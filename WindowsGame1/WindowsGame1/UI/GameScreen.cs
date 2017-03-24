using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace WindowsGame1
{
	public class GameScreen: ScreenTask
	{
		public Arena World;
        
        
		/// <summary>
		/// World representation. (players, walls, background, items, bullets, etc.)
		/// User interface is found in HUDScreen.
		/// </summary>
		public GameScreen()
		{
			World = WorldGenerator.GenerateWorld(Config.Players);

		}

		public override void Draw(GameTime gameTime, SpriteBatch sb)
		{
			base.Draw(gameTime, sb);
			DrawLight(sb);
			World.light.Draw(-1 * GetMyScreenPos(World.Walls[0, 0].Position));
			sb.Begin();
			DrawFloors(sb);
			DrawItems(sb);
			DrawCharacters(sb);
			DrawBullets(sb);
			sb.End();
			World.light.LightsFX.PrintLightsOverTexture(null, sb, World.light.Game.graphics, World.light.ScreenLights, World.light.ScreenGround, 0.95f);
			sb.Begin();
			DrawMe(sb);
			// draw world tiles that are visible.
			//DrawWalls(sb);
			sb.End();
		}

		public override void Update(GameTime gameTime, Vector2 mousePos, int left, int right, int[] keysPressed)
		{


			base.Update(gameTime, mousePos, left, right, keysPressed);

			World.Update(gameTime, MousePosToRotation(mousePos), left, right, keysPressed);
		}

		/// <summary>
		/// Yet to be implemented method for all objects that are not the client's player.
		/// </summary>
		/// <param name="myPos">object's position</param>
		/// <param name="mySize">object's size</param>
		/// <param name="resolution">current game resolution</param>
		/// <returns>Vector2 which represents the center of the screenPosition of the object.</returns>
		private Vector2 GetMyScreenPos(Vector2 myPos)
		{
			var xHalf = TVSGame.Resolution.X / 2;
			var yHalf = TVSGame.Resolution.Y / 2;

			var temp = Vector2.Zero;
			temp.X = xHalf - (World.Me.Position.X - (myPos.X));
			temp.Y = yHalf - (World.Me.Position.Y - (myPos.Y));
			return temp;
		}

		private Vector2 MousePosToRotation(Vector2 myPos)
		{
			var temp = Vector2.Zero;
			temp.X = (myPos.X - (TVSGame.Resolution.X / 2));
			temp.Y = (myPos.Y - (TVSGame.Resolution.Y / 2));
			return temp;
		}

		private void DrawItems(SpriteBatch sb)
		{
			foreach (var i in World.Items)
			{
				if (!i.PickedUp)
				{
					var screenPos = GetMyScreenPos(i.Position);
					if (screenPos.X > -Config.TileSize && screenPos.Y > -Config.TileSize && screenPos.X < TVSGame.Resolution.X + Config.TileSize && screenPos.Y < TVSGame.Resolution.Y + Config.TileSize)
					{
						sb.Draw(i.Textures[0], new Rectangle((int)screenPos.X, (int)screenPos.Y, i.BBox.Width, i.BBox.Height), i.BBox, Color.White, i.Rotation, new Vector2(i.GetSize().X / 2, i.GetSize().Y / 2), SpriteEffects.None, 0);
					}
				}
			}
		}

		private void DrawCharacters(SpriteBatch sb)
		{
			foreach (var p in World.Team1)
			{
				if (!p.Dead && !p.SemiDead && p != World.Me)
				{

					p.ScreenPos = GetMyScreenPos(p.Position);
					sb.Draw(p.Textures[0], new Rectangle((int)p.ScreenPos.X, (int)p.ScreenPos.Y, p.GetSize().X, p.GetSize().Y), p.Frame, p.Clr, p.Rotation, new Vector2(p.Textures[0].Bounds.Center.X, p.Textures[0].Bounds.Center.Y), SpriteEffects.None, 0);
					sb.Draw(p.Textures[1], new Rectangle((int)p.ScreenPos.X - (p.Textures[1].Width / 2), (int)p.ScreenPos.Y - 25, p.Textures[1].Width, p.Textures[1].Height), Color.White);
					sb.Draw(p.Textures[2], new Rectangle((int)p.ScreenPos.X - (p.Textures[2].Width / 2), (int)p.ScreenPos.Y - 25, (int)(p.Textures[2].Width * (p.CurrentHealth / Config.MaxPlayerHealth)), p.Textures[2].Height), new Color(0, 255, 0, 150));

				}

			}
			foreach (var p in World.Team2)
			{
				if (!p.Dead && !p.SemiDead)
				{
					p.ScreenPos = GetMyScreenPos(p.Position);

					sb.Draw(p.Textures[0], new Rectangle((int)p.ScreenPos.X, (int)p.ScreenPos.Y, p.GetSize().X, p.GetSize().Y), p.Frame, p.Clr, p.Rotation, new Vector2(p.GetSize().X / 2, p.GetSize().Y / 2), SpriteEffects.None, 0);
					sb.Draw(p.Textures[1], new Rectangle((int)p.ScreenPos.X - (p.Textures[1].Width / 2), (int)p.ScreenPos.Y - 25, p.Textures[1].Width, p.Textures[1].Height), Color.White);
					sb.Draw(p.Textures[2], new Rectangle((int)p.ScreenPos.X - (p.Textures[2].Width / 2), (int)p.ScreenPos.Y - 25, (int)(p.Textures[2].Width * (p.CurrentHealth / Config.MaxPlayerHealth)), p.Textures[2].Height), new Color(0,255,0, 150));
				}
			}
		}

		private void DrawBullets(SpriteBatch sb)
		{
			foreach (var b in World.ListOfMyBullets)
			{
				var screenPos = GetMyScreenPos(b.Position);
				sb.Draw(b.Textures[0], new Rectangle((int)screenPos.X, (int)screenPos.Y, b.GetSize().X, b.BoundingBox.Height), b.Frame, Color.White, b.Rotation, new Vector2(b.Textures[0].Bounds.Center.X, b.Textures[0].Bounds.Center.Y), SpriteEffects.None, 0.0f);
			}
			foreach (var b in World.ListOfOtherBullets)
			{
				var screenPos = GetMyScreenPos(b.Position);
				sb.Draw(b.Textures[0], new Rectangle((int)screenPos.X, (int)screenPos.Y, b.GetSize().X, b.BoundingBox.Height), b.Frame, Color.White, b.Rotation, new Vector2(b.Textures[0].Bounds.Center.X, b.Textures[0].Bounds.Center.Y), SpriteEffects.None, 0.0f);
			}
		}
	
		private void DrawMe(SpriteBatch sb)
		{
			if (!World.Me.Dead && !World.Me.SemiDead)
			{
				sb.Draw(World.Me.Textures[0], new Rectangle((TVSGame.Resolution.X / 2), (TVSGame.Resolution.Y / 2), World.Me.GetSize().X, World.Me.GetSize().Y), World.Me.Frame, World.Me.Clr, World.Me.Rotation, new Vector2(World.Me.GetSize().X / 2, World.Me.GetSize().Y / 2), SpriteEffects.None, 0);
			}
		}

		private void DrawWalls(SpriteBatch sb)
		{
			for (var y = 0; y < World.Size; y++)
			{
				for (var x = 0; x < World.Size; x++)
				{
					if (null != World.Walls[x, y] && World.Walls[x, y].Visible)
					{
						var screenPos = GetMyScreenPos(World.Walls[x, y].Position);
						sb.Draw(World.Walls[x, y].Textures[0], new Rectangle((int)screenPos.X, (int)screenPos.Y, Config.TileSize, Config.TileSize), World.Walls[x, y].Frame, Color.White, 0.0f, new Vector2(Config.TileSize / 2, Config.TileSize / 2), SpriteEffects.None, 0.0f);
					}
				}
			}
		}

		private void DrawFloors(SpriteBatch sb)
		{
			for (var y = 0; y < World.Size; y++)
			{
				for (var x = 0; x < World.Size; x++)
				{
					if (null != World.Floors[x, y])
					{
						var screenPos = GetMyScreenPos(World.Floors[x, y].Position);
						sb.Draw(World.Floors[x, y].Textures[0], new Rectangle((int)screenPos.X, (int)screenPos.Y, Config.TileSize, Config.TileSize), World.Floors[x, y].Frame, Color.White, 0.0f, new Vector2(Config.TileSize / 2, Config.TileSize / 2), SpriteEffects.None, 0.0f);
					}
				}
			}
		}

		public void DrawLight(SpriteBatch sb)
		{
			var lightRange = 6;
			World.light.ShadowMap.StartGeneratingShadowCasteMap(false);
			{
				var minY = World.Me.Position.Y / Config.TileSize - lightRange;
				minY = (minY < 0) ? 0 : minY;
				var maxY = minY + lightRange * 2;
				maxY = (maxY > World.Size) ? World.Size : maxY;

				var minX = World.Me.Position.X / Config.TileSize - lightRange;
				minX = (minX < 0) ? 0 : minX;
				//

				var maxX = minX + lightRange * 2;
				maxX = (maxX > World.Size) ? World.Size : maxX;
				for (var y = minY; y < maxY; y++)
				{
					for (var x = minX; x < maxX; x++)
					{
						if (null != World.Walls[(int)x, (int)y] && World.Walls[(int)x, (int)y].Visible)
						{
							var screenPos = GetMyScreenPos(World.Walls[(int)x, (int)y].Position);
							screenPos.X -= Config.TileSize / 2;
							screenPos.Y -= Config.TileSize / 2;
							var screenPos1 = GetMyScreenPos(World.Walls[0, 0].Position);
							var newPos = new Vector2(World.Walls[(int)x, (int)y].Position.X, World.Walls[(int)x, (int)y].Position.Y);
							World.light.ShadowMap.AddShadowCaster(World.Walls[(int)x, (int)y].Textures[0], screenPos, Config.TileSize, Config.TileSize);
						}
					}
				}
			}
			World.light.ShadowMap.EndGeneratingShadowCasterMap();
		}
	}
}