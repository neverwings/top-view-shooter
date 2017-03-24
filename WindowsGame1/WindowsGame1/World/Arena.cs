using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System.Threading;
using System.IO;

using Lidgren.Network;
using Lidgren.Network.Xna;
namespace WindowsGame1
{
	public class Arena
	{
		NetClient client;
		private Vector2  lastMousePos = new Vector2();
		//static ReaderWriterLock rwl = new ReaderWriterLock(); //so that arenaClient's worker thread does not screw up our parsing of raw snapshot.
		public Tile[,] Walls;
		public Tile[,] Floors;
		public int Size;
		public int Team1Lifes
		{
			get
			{
				var lifes = 0;
				foreach (var p in Team1)
				{
					lifes += p.Lifes;
				}
				return lifes;
			}
		}
		public int Team2Lifes
		{
			get
			{
				var lifes = 0;
				foreach (var p in Team2)
				{
					lifes += p.Lifes;
				}
				return lifes;
			}
		}
		public List<Player> Team1;
		public List<Player> Team2;
		
		private float[] _timers;
		public Player Me 
		{
			get
			{
				return Team1[0];
			}
		}
		public Item[] Items;
		public World _world;
		public List<Bullet> ListOfMyBullets;
		public List<Bullet> ListOfOtherBullets;
		public Light light;

		public Arena(World world, Tile[,] walls, Tile[,] floors, Item[] items, int size)
		{
			Walls = walls;
			Floors = floors;
			Size = size;
			Items = items;
			_world = world;
			ListOfMyBullets = new List<Bullet>();
			ListOfOtherBullets = new List<Bullet>();
			Random rnd = new Random();
			int id = rnd.Next(0, 1000);
			Player me = new Player(_world, new Vector2(100, 100), 0, true);
			me.id = id;

			Team1 = new List<Player>();
			Team2 = new List<Player>();
			Team1.Add(me);
			Me.BulletEvent += AddBullet;
			_timers = new float[items.Length];

			light = new Light(Me.Position);

			#region networking


			NetPeerConfiguration config = new NetPeerConfiguration("xnaapp");
			config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);

			client = new NetClient(config);
			client.Start();
			client.Connect("127.0.0.1", 14242);
			
			#endregion
		}

		public void Update(GameTime gameTime, Vector2 mousePos, int leftClick, int rightClick, int[] keysPressed)
		{
			var x = (keysPressed[2] > 0 ? -1 : 0) + (keysPressed[3] > 0 ? 1 : 0);
			var y = (keysPressed[0] > 0 ? -1 : 0) + (keysPressed[1] > 0 ? 1 : 0);
			Vector2 direction = new Vector2(x, y);
			if (direction.Length() > 0 || mousePos.X != lastMousePos.X || mousePos.Y != lastMousePos.Y)
			{
				//
				// If there's input; send it to server
				//
				NetOutgoingMessage om = client.CreateMessage();
				om.Write(0);
				om.Write(Me.id); // write positions as int
				om.Write(Me.Position); // write positions as floats
				om.Write(Me.Rotation); // write positions as int
				client.SendMessage(om, NetDeliveryMethod.Unreliable);
			}
			lastMousePos = mousePos;


			// read messages
			NetIncomingMessage msg;
			while ((msg = client.ReadMessage()) != null)
			{
				switch (msg.MessageType)
				{
					case NetIncomingMessageType.DiscoveryResponse:
						// just connect to first server discovered
						client.Connect(msg.SenderEndpoint);
						break;
					case NetIncomingMessageType.Data:
						var gameMessageType = msg.ReadInt32();
						switch (gameMessageType)
						{
							case 0: //playerstate                           
								this.DispatchRemotePlayerStatePacket(msg);
								break;
							case 1: //shot
								this.DispatchRemoteShot(msg);
								break;
						}

						break;
				}
			}
			Me.Update(new Vector2(x,y), mousePos, leftClick, gameTime);

			// TEST
			if (keysPressed[4] > 0)
			{
				if (Me.CurrentHealth > 0)
				{
					Me.CurrentHealth--;
				}
			}
			foreach (var p in Team1)
			{
				if (p != Me)
				{
					p.Update(p.Position, p.Rotation, gameTime);
				}
				if (p.WaitingForRespawn && p == Me)
				{
					p.Position = GetNewSpawnSpot(Team1);
					p.WaitingForRespawn = false;
					p.SemiDead = false;
					p.CurrentDeathPenalty = 0;
				}
			}
			foreach (var p in Team2)
			{
				p.Update(p.Position, p.Rotation, gameTime);
			}

			//MAIN LOOP
			//CAUTION, BE GENTLE WITH THE CODE BELOW

			UpdateBullet();
			UpdateBullet(ListOfOtherBullets);
			UpdateListOfWalls();
			foreach (var i in Items)
			{
				i.Update(gameTime);
				if (!Me.SemiDead && !Me.Dead && !i.PickedUp && Me.BBox.Intersects(new Rectangle((int)i.Position.X + Config.TileSize / 2, (int)i.Position.Y + Config.TileSize / 2, i.BBox.Width, i.BBox.Height)))
				{
					if (i.Type == ItemType.Ammo && Me.AmmoCount < Config.MaxAmmo)
					{
						Me.AmmoCount = (Me.AmmoCount + Config.AdditionalAmmo > Config.MaxAmmo) ? Config.MaxAmmo : Me.AmmoCount + Config.AdditionalAmmo;
						Me.Ammo.DType = i.DType;
						i.PickedUp = true;
						i.RespawnTime = Config.AmmoRespawnTime;
					}
					else if(i.Type == ItemType.Firstaid && Me.CurrentHealth < Config.MaxPlayerHealth)
					{
						Me.CurrentHealth = (Me.CurrentHealth + i.Health < Config.MaxPlayerHealth) ? Me.CurrentHealth + i.Health : Config.MaxPlayerHealth;
						i.PickedUp = true;
						i.RespawnTime = Config.AmmoRespawnTime;
					}
				}
			}
			_world.Step(1);
			light.Update(Me.Position, gameTime);
			//END MAIN LOOP
		}

		private void UpdateListOfWalls()
		{
			var min = new Point((int)(Me.Position.X - (TVSGame.Resolution.X * 0.6f)), (int)(Me.Position.Y - (TVSGame.Resolution.Y * 0.6f)));
			var max = new Point((int)(Me.Position.X + (TVSGame.Resolution.X * 0.6f)), (int)(Me.Position.Y + (TVSGame.Resolution.Y * 0.6f)));
			var indices = new List<int>();
			for (var y = 0; y < Size; y++)
			{
			   for (var x = 0; x < Size; x++)
			   {
				   if(null != Walls[x,y])
				   {
					   Walls[x, y].Update(min, max);     //check for null value
				   }                 
			   }
			}
		}

		#region networking



		#endregion

		/// <summary>
		/// add the shooting bullets into a list
		/// </summary>
		/// <param name="bullet">bullet that has already been created by trigging the event in Player</param>
		public void AddBullet(Bullet bullet)
		{
			NetOutgoingMessage om = client.CreateMessage();
			om.Write(1);
			om.Write(bullet.who);
			om.Write(bullet.Position); // write positions as floats
			om.Write(bullet.Rotation); // write positions as int
			om.Write(Config.BasePlayerDamage); // write positions as int
			om.Write((int)bullet.Damage.Type); // write positions as int
			client.SendMessage(om, NetDeliveryMethod.Unreliable);

			light.CreateNewLightSource(bullet);
			ListOfMyBullets.Add(bullet);
		}

		/// <summary>
		/// Update bullet current position as well as deleting the bullet from list if intersect with the walls or not
		/// </summary>
		public void UpdateBullet()
		{
		   if (ListOfMyBullets.Count > 0)
			{
				for (int i = ListOfMyBullets.Count - 1; i >= 0; i--)
				{
					var minY = ListOfMyBullets[i].Position.Y / Config.TileSize - 2;
					minY = (minY < 0) ? 0 : minY;
					var maxY= minY + 4;
					maxY = (maxY > Size) ? Size : maxY;

					var minX = ListOfMyBullets[i].Position.X / Config.TileSize - 2;
					minX = (minX < 0) ? 0 : minX;
					var maxX = minX + 4;
					maxX = (maxX > Size) ? Size : maxX;

					ListOfMyBullets[i].Update();
					for (var y = minY; y < maxY; y++)
					{
						for (var x = minX; x < maxX ; x++)
						{
							if (Walls[(int)x, (int)y] != null)
							{
								if (ListOfMyBullets.Count > 0 && i < ListOfMyBullets.Count)
								{
									if (ListOfMyBullets[i].BoundingBox.Intersects(Walls[(int)x, (int)y].BBox))
									{
										ListOfMyBullets[i].DisposeBullet();
										ListOfMyBullets.RemoveAt(i);
									}
								}
							}
						}
					}
				}
				for (int i = ListOfMyBullets.Count - 1; i >= 0; i--)
				{
					foreach (var p in Team2)
					{
						if ( !p.SemiDead && !p.Dead && ListOfMyBullets.Count > 0 && i < ListOfMyBullets.Count)
						{
							if ( Vector2.Distance(ListOfMyBullets[i].Position, p.Position) < (p.GetSize().X / 2.2f + ListOfMyBullets[i].GetSize().X / 2.2f))
							{
								p.AddDamage(ListOfMyBullets[i].Damage);
								ListOfMyBullets[i].DisposeBullet();
								ListOfMyBullets.RemoveAt(i);
							}
						}
					}
					foreach (var p in Team1)
					{
						if ( !p.SemiDead && !p.Dead && ListOfMyBullets.Count > 0 && i < ListOfMyBullets.Count)
						{
							if (p != Me && ListOfMyBullets[i].BoundingBox.Intersects(new Rectangle(p.BBox.X - (p.GetSize().X / 2), p.BBox.Y - (p.GetSize().Y / 2), p.GetSize().X, p.GetSize().Y)))
							{
								p.AddDamage(ListOfMyBullets[i].Damage);
								ListOfMyBullets[i].DisposeBullet();
								ListOfMyBullets.RemoveAt(i);
							}
						}
					}
				}
			}
		}

		public void UpdateBullet(List<Bullet> buls)
		{
			if (buls.Count > 0)
			{
				for (int i = buls.Count - 1; i >= 0; i--)
				{
					buls[i].Update();
				}
				for (int i = buls.Count - 1; i >= 0; i--)
				{
					foreach (var p in Team2)
					{
						if (buls.Count > 0 && i < buls.Count)
						{
							if (p.id != buls[i].who && buls[i].BoundingBox.Intersects(new Rectangle(p.BBox.X - (p.GetSize().X / 2), p.BBox.Y - (p.GetSize().Y / 2), p.GetSize().X, p.GetSize().Y)))
							{
								p.AddDamage(buls[i].Damage);
								buls[i].DisposeBullet();
								buls.RemoveAt(i);
							}
						}
					}

					foreach (var p in Team1)
					{
						if (buls.Count > 0 && i < buls.Count)
						{
							if (p.id != buls[i].who && buls[i].BoundingBox.Intersects(new Rectangle(p.BBox.X - (p.GetSize().X / 2), p.BBox.Y - (p.GetSize().Y / 2), p.GetSize().X, p.GetSize().Y)))
							{
								p.AddDamage(buls[i].Damage);
								buls[i].DisposeBullet();
								buls.RemoveAt(i);
							}
						}
					}
				}
			}
		}

		private Vector2 GetNewSpawnSpot(List<Player> team)
		{
			var r = new Random();
			Vector2 returner = Vector2.Zero;
			var respawn = false;
			var indices = new List<int>();
			for (var i = 0; i < team.Count; i++)
			{
				if (!team[i].Dead && !team[i].SemiDead && team[i] != Me)
				{
					indices.Add(i);
				}
			}
			while (!respawn)
			{
				respawn = true;
				if (indices.Count > 0)
				{
					var index = indices[r.Next(indices.Count)];
					var position = team[index].Position;
					var offset = new Vector2(Config.TileSize * r.Next(Config.RespawnDistance), Config.TileSize * r.Next(Config.RespawnDistance));
					returner = (r.Next(2) < 1) ? position - offset : position + offset;
					returner.X -= returner.X % Config.TileSize;
					returner.Y -= returner.Y % Config.TileSize;
				}
				else
				{
					var min = Config.TileSize * 3;
					returner = new Vector2(r.Next(min, (Size * Config.TileSize) - min), r.Next(min, (Size * Config.TileSize) - min));
				}
				if (this.Walls[(int)returner.X / Config.TileSize, (int)returner.Y / Config.TileSize] != null)
				{
					respawn = false;
				}
			}
			return returner;
		}

		internal void DispatchRemotePlayerStatePacket(NetIncomingMessage msg)
		{
			int id = msg.ReadInt32();
			Vector2 position = msg.ReadVector2();
			float rotation = msg.ReadFloat();
			Player player = null;
			foreach (Player p in Team2)
			{
				if (p.id == id)
				{
					player = p;
					p.Position = position;
					p.Rotation = rotation;
					break;
				}
			}

			//Add other player to local FFAPArticipants
			if (player == null && id != 0 && Me.id != id)
			{
				player = new Player(_world, position, 0, false);
				player.id = id;
				player.Rotation = rotation;
				Team2.Add(player);
			}
		}

		internal void DispatchRemoteShot(NetIncomingMessage msg)
		{
			int who = msg.ReadInt32();
			if (who != Me.id)
			{
				Vector2 position = msg.ReadVector2();
				float rotation = msg.ReadFloat();
				float dmg = msg.ReadFloat();
				DamageType dt = (DamageType)msg.ReadByte();
				Bullet bul = new Bullet(who, position, rotation, dmg, dt);
				ListOfOtherBullets.Add(bul);
				light.CreateNewLightSource(bul);
			}

		}
	}
}