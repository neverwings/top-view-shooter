using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;

namespace WindowsGame1
{
	public abstract class WorldObject
	{
		public List<Texture2D> Textures;
		public Body MyBody;
		public Color Clr;
		public virtual Rectangle BBox
		{
			get
			{
				return new Rectangle((int)Position.X, (int)Position.Y, _size.X, _size.Y);
			}
		}
		public bool _hasBody = true;
		protected float _rotation;
		public virtual float Rotation
		{
			get
			{
				return _hasBody ? MyBody.Rotation : _rotation;
			}
			set
			{
				if (_hasBody)
				{
					MyBody.Rotation = value;
				}
				else
				{
					_rotation = value;
				}
			}
		}
		public float Scale;
		protected Vector2 _position;
		public Vector2 Position
		{
			get
			{
				return _hasBody ? MyBody.Position : _position;
			}
			set
			{
				if (_hasBody)
				{
					MyBody.Position = value;
				}
				else
				{
					_position = value;
				}
			}
		}
		protected Point _size;
		public bool Visible;

		~WorldObject()
		{
			if (_hasBody)
			{
				MyBody.Dispose();
			}
		}

		public WorldObject(World world, Vector2 pos, bool hasBody)
		{
			_hasBody = hasBody;
			Textures = new List<Texture2D>();
		}

		public virtual Point GetSize()
		{
			return _size;
		}

		public virtual void SetSize(Point size)
		{
			_size = size;
		}
	}
}
