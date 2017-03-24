using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace WindowsGame1
{
	public class Bullet: WorldObject
	{
		public delegate void DisposeTrigger(Bullet b);
		public Vector2 Direction;
		public DisposeTrigger DisposeEvent;
		public Other.Damage Damage
		{
			get;
			protected set;
		}
		public bool isSent;
		private int _frame = 0;
		private int _delay = 0;
		private int _frameCount = 3;
        public int who;
		/// <summary>
		/// Creating a rectangle for each bullet to check the intersection with another object
		/// </summary>
		public Rectangle BoundingBox
		{
			get
			{
				return new Rectangle((int)Position.X - (Textures[0].Width / 2), (int)Position.Y - (Textures[0].Height / 2 / _frameCount), Textures[0].Width, Textures[0].Height / _frameCount);
			}
		}

		public Rectangle Frame
		{
			get
			{
				return new Rectangle(0, BoundingBox.Height * _frame, Textures[0].Width, BoundingBox.Height);
			}
		}

		public Bullet(int who, Vector2 pos, float rot, float dmg, DamageType type)
			: base(null, pos, false)
		{
			isSent = false;
			switch (type)
			{
				case DamageType.Normal:
				Textures.Add(Loader.LoadGameTexture("WorldObjects/Bullets/NormalBullet"));
				Damage = new Other.Damage(dmg);
				Clr = Color.White;
				break;
				case DamageType.Poison:
				Textures.Add(Loader.LoadGameTexture("WorldObjects/Bullets/PoisonBullet"));
				Damage = new Other.Damage(dmg, 4, 1, type);
				Clr = Color.Green;
				break;
				case DamageType.Water:
				Textures.Add(Loader.LoadGameTexture("WorldObjects/Bullets/WaterBullet"));
				Damage = new Other.Damage(dmg, 2, 5, type);
				Clr = Color.Blue;
				break;
			}
            this.who = who;
			_size = new Point(Textures[0].Width, Textures[0].Height / 3);
			Direction = Config.AngleToVector(rot);
			Rotation = rot;
			Position = pos;
		}
		
		public void DisposeBullet()
		{
			if (DisposeEvent != null)
			{
				DisposeEvent.Invoke(this);
			}
		}

		/// <summary>
		/// Update bullet
		/// </summary>
		/// <param name="direction">the direction of the bullet where it goes to</param>
		public void Update()
		{
			Position += Direction * Config.BulletSpeed;
			if (_delay++ > 10)
			{
				_frame = (_frame++ < 2) ? _frame : 0;
				_delay = 0;
			}
		}
	}
}
