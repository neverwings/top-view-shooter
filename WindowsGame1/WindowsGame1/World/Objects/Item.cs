using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
	public class Item : WorldObject
	{
		public ItemType Type;
		public DamageType DType;
		public float Damage;
		public int Amount;
		public int Health;
		public float AbsorbDamage; // for shield
		private float frameTime;
		public float RespawnTime;
		public float Color;
		private int _frame = 0;
		private float _frameDelay = 0.2f;
		private int _frameCount = 5;
		private bool up;
		public bool PickedUp;
		public override Rectangle BBox
		{
			get
			{
				return new Rectangle(0, (Textures[0].Height / _frameCount) * _frame, Textures[0].Width, Textures[0].Height / _frameCount);
			}
		}

		public Item(World world, Vector2 pos, bool hasBody) : base(world, pos, hasBody)
		{
		}

		/// <summary>
		/// Ammo Constructor
		/// </summary>
		/// <param name="world"></param>
		/// <param name="pos">world position</param>
		/// <param name="hasBody">farseer physics effected or not</param>
		/// <param name="amount">Amount of ammo</param>
		/// <param name="texture">texture</param>
		public Item(World world, Vector2 pos, bool hasBody, int amount, Texture2D texture, DamageType dmgType)
			: base(world, pos, hasBody)
		{
			Type = ItemType.Ammo;
			DType = dmgType;
			Amount = amount;
			Textures.Add(texture);
			Position = pos;
		}

		/// <summary>
		/// Ammo Constructor
		/// </summary>
		/// <param name="world"></param>
		/// <param name="pos">world position</param>
		/// <param name="hasBody">farseer physics effected or not</param>
		/// <param name="damage">Amount of damage</param>
		/// <param name="texture">texture</param>
		public Item(World world, Vector2 pos, bool hasBody, float damage, Texture2D texture)
			: base(world, pos, hasBody)
		{
			Type = ItemType.Ammo;
			Damage = damage;
		}

		/// <summary>
		/// Powerup Constructor
		/// </summary>
		/// <param name="world"></param>
		/// <param name="pos">world position</param>
		/// <param name="hasBody">farseer physics effected or not</param>
		/// <param name="damage">Amount of damage</param>
		/// <param name="health">Amount of health</param>
		/// <param name="texture">texture</param>
		public Item(World world, Vector2 pos, bool hasBody, float damage, int health, Texture2D texture) : base(world, pos, hasBody)
		{
			Type = ItemType.Powerup;
			Damage = damage;
			Health = health;
		}

		/// <summary>
		/// Firstaid kit Constructor
		/// </summary>
		/// <param name="world"></param>
		/// <param name="pos">world position</param>
		/// <param name="hasBody">farseer physics effected or not</param>
		/// <param name="health">Amount of health</param>
		/// <param name="texture">texture</param>
		public Item(World world, Vector2 pos, bool hasBody, Texture2D texture, int health)
			: base(world, pos, hasBody)
		{
			Type = ItemType.Firstaid;
			_frameCount = 6;
			Textures.Add(texture);
			Health = health;
			Position = pos;
			_frameDelay = 0.1f;
		}

		public void Update(GameTime gameTime)
		{
			if (RespawnTime > 0 && PickedUp)
			{
				RespawnTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
			}
			PickedUp = (RespawnTime > 0) ? true : false;
			frameTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (Type == ItemType.Ammo)
			{
				if (frameTime > _frameDelay)
				{
					if (up && _frame == 0)
					{
						up = false;
					}
					else if (!up && _frame == _frameCount - 1)
					{
						up = true;
					}
					_frame = (up) ? _frame - 1 : _frame + 1;
					frameTime = 0;
				}
			}
			else
			{
				if (frameTime > _frameDelay)
				{
					_frame = (_frame++ < _frameCount - 1) ? _frame : 0;
					frameTime = 0;
				}
			}
		}
	}
}
