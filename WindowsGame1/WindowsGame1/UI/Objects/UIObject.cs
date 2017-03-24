using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
	public abstract class UIObject
	{
		public Vector2 Position;
		public Vector2 Size;
		public Color Clr;
		public Texture2D Texture;

		public virtual Rectangle BoundingBox
		{
			get
			{
				return new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
			}
		}

		public UIObject()
		{
		}

		public virtual void Update(Vector2 position)
		{
			Position = position;
		}
	}
}
