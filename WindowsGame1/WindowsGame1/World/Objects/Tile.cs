using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace WindowsGame1
{
	public class Tile: WorldObject
	{
		public override Rectangle BBox
		{
			get
			{
				return new Rectangle((int)Position.X - (Config.TileSize / 2), (int)Position.Y - (Config.TileSize / 2), Config.TileSize, Config.TileSize);
			}
		}
		private int _frameCount = 4;
		private int _frame = 0;
		public Rectangle Frame
		{
			get
			{
				return new Rectangle(0, (Textures[0].Height / _frameCount) * _frame, Textures[0].Width, Textures[0].Height / _frameCount);
			}
		}
		public Tile(World world, Vector2 pos, bool outerWall, int frame, bool hasBody) : base(world, pos, hasBody)
		{
			if (hasBody)
			{
				Textures.Add(Loader.LoadGameTexture("WorldObjects/wall"));
				MyBody = BodyFactory.CreateRectangle(world, Config.TileSize, Config.TileSize, 1f, pos);
				MyBody.IsStatic = true;
				MyBody.BodyType = BodyType.Static;
				MyBody.Restitution = 0.0f;
				MyBody.CollisionCategories = outerWall ? Category.Cat3 : Category.Cat2;
				MyBody.CollidesWith = Category.Cat1;
			}
			else
			{
				Textures.Add(Loader.LoadGameTexture("WorldObjects/wall"));
				_position = pos;
			}
			_frame = frame;
		}

		public void Update(Point min, Point max)
		{
			if (min.X < Position.X && max.X > Position.X)
			{
				if (min.Y < Position.Y && max.Y > Position.Y)
				{
					Visible = true;
				}
				else
				{
					Visible = false;
				}
			}
			else
			{
				Visible = false;
			}
		}

		public override Point GetSize()
		{
			return new Point(Config.TileSize, Config.TileSize);
		}
	}
}
