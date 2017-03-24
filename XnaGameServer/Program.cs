using System;
using System.Threading;
using System.Net;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
namespace XnaGameServer
{
	public enum DamageType
	{
		Normal,
		Poison,
		Water,
	}
	class Player
	{
		public int id;
		public Vector2 pos;
		public float rot;

	}
	class Bullet
	{
		public int who;
		public Vector2 pos;
		public float rot;
		public float dmg;
		public DamageType dt;
	}
	class Program
	{

		static void Main(string[] args)
		{
			
			List<Player> players = new List<Player>();
			List<Bullet> bullets = new List<Bullet>();
			

			NetPeerConfiguration config = new NetPeerConfiguration("xnaapp");
			config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
			config.Port = 14242;

			// create and start server
			NetServer server = new NetServer(config);
			server.Start();

			// schedule initial sending of position updates
			double nextSendUpdates = NetTime.Now;

			// run until escape is pressed
			while (!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Escape)
			{
				NetIncomingMessage msg;
				Player player = null;

				while ((msg = server.ReadMessage()) != null)
				{
					switch (msg.MessageType)
					{
						case NetIncomingMessageType.DiscoveryRequest:
							//
							// Server received a discovery request from a client; send a discovery response (with no extra data attached)
							//
							server.SendDiscoveryResponse(null, msg.SenderEndpoint);
							break;
						case NetIncomingMessageType.VerboseDebugMessage:
						case NetIncomingMessageType.DebugMessage:
						case NetIncomingMessageType.WarningMessage:
						case NetIncomingMessageType.ErrorMessage:
							//
							// Just print diagnostic messages to console
							//
							Console.WriteLine(msg.ReadString());
							break;
						case NetIncomingMessageType.StatusChanged:
							NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
							if (status == NetConnectionStatus.Connected)
							{
								//
								// A new player just connected!
	
								Console.WriteLine(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " connected!");
							}

							break;
						case NetIncomingMessageType.Data:
							//
							// The client sent input to the server
							//
							var gameMessageType = msg.ReadInt32();
							switch (gameMessageType)
							{
								case 0:
									player = new Player();
									player.id = msg.ReadInt32();
									player.pos = msg.ReadVector2();
									player.rot = msg.ReadFloat();
									players.Add(player);
									break;
								case 1:
									Bullet bul = new Bullet();
									bul.who = msg.ReadInt32();
									bul.pos = msg.ReadVector2();
									bul.rot = msg.ReadFloat();
									bul.dmg = msg.ReadFloat();
									bul.dt = (DamageType)msg.ReadByte();
									bullets.Add(bul);
									break;
							}

							break;
					}

					//
					// send position updates 30 times per second
					//
					double now = NetTime.Now;
					if (now > nextSendUpdates)
					{
						// Yes, it's time to send position updates

						// for each player...
						foreach (NetConnection con in server.Connections)
						{

							// ... send information about every other player (actually including self)
							foreach (Player p in players)
							{
								if (p.id != 0)
								{
									// send position update about 'otherPlayer' to 'player'
									NetOutgoingMessage om = server.CreateMessage();

									// write who this position is for
									om.Write(0);
									om.Write(p.id);
									om.Write(p.pos);
									om.Write(p.rot);
									// send message
									server.SendMessage(om, con, NetDeliveryMethod.Unreliable);
								}
							}
							for (int i = 0; i < bullets.Count; i++)
							{
								// send position update about 'otherPlayer' to 'player'
								NetOutgoingMessage om = server.CreateMessage();

								// write who this position is for
								om.Write(1);
								om.Write(bullets[i].who);
								om.Write(bullets[i].pos);
								om.Write(bullets[i].rot);
								om.Write(bullets[i].dmg);
								om.Write((int)bullets[i].dt);
								server.SendMessage(om, con, NetDeliveryMethod.Unreliable);
							}
							

						}
						bullets.Clear();
						players.Clear();
						// schedule next update
						nextSendUpdates += (1.0 / 30.0);
					}
				}

				// sleep to allow other processes to run smoothly
				Thread.Sleep(1);
			}

			server.Shutdown("app exiting");
		}
	}
}