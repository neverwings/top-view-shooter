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
	public delegate void BulletTrigger(Bullet b);
	public class Player: WorldObject
	{
		#region Variables

		public BulletTrigger BulletEvent;
		public int id;
		public int Team; // index of Team.
		public int Lifes;
		public int AmmoCount;
		public int BulletDelay;
		private float _maxHealth;
		private float _currentHealth;
		public int MaxHealth
		{
			get
			{
				var returner = _maxHealth;
				foreach (var i in Powerups)
				{
					returner += i.Health;
				}
				return (int)Math.Round(returner);
			}
		}
		public float CurrentHealth
		{
			get
			{
				return _currentHealth;
			}
			set
			{
				_currentHealth = value;
			}
		}
		public Item Ammo;
		public List<Item> Powerups;
		private float _damage;
		public float Damage
		{
			get
			{
				return Ammo.Damage + _damage; // add powerup damage.
			}
			set
			{
				_damage = value;
			}
		}
		private List<Other.Damage> _takenDamage = new List<Other.Damage>();
		private float _maxSpeed;
		public float Velocity = 0;
		public bool Dead;
		public bool SemiDead;
		public bool WaitingForRespawn;
		private float _deathPenalty;
		public float CurrentDeathPenalty;
		public override float Rotation
		{
			get
			{
				return _rotation;
			}
			set
			{
				_rotation = value;
			}
		}
		private int _frameCount = 5;
		private int _frame = 2;
		private int _standardFrame = 2;
		private float frameTime;
		private bool up;
		private float _frameDelay
		{
			get
			{
				return 0.06f;
			}
		}
		public Rectangle Frame
		{
			get
			{
				return new Rectangle(0, (Textures[0].Height / _frameCount) * _frame, Textures[0].Width, Textures[0].Height / _frameCount);
			}
		}
		public Vector2 ScreenPos;

		#endregion

		public Player(World world, Vector2 pos, int team, bool collision) : base(world, pos, collision)
		{
			Clr = Color.White;
			Team = team;
			if (team == 1)
			{
				Textures.Add(Loader.LoadGameTexture("WorldObjects/character 1"));
			}
			else
			{
				Textures.Add(Loader.LoadGameTexture("WorldObjects/character 2"));
			}
			Textures.Add(Loader.LoadGameTexture("WorldObjects/OtherHPContainer"));
			Textures.Add(Loader.LoadGameTexture("WorldObjects/OtherHPBar"));
			_size = new Point(Textures[0].Width, Textures[0].Height / _frameCount);
			if (collision)
			{
				MyBody = BodyFactory.CreateCircle(world, _size.Y / 2, 1f, pos);
				MyBody.IsStatic = false;
				MyBody.BodyType = BodyType.Dynamic;
				MyBody.Restitution = 0.0f;
				MyBody.CollisionCategories = Category.Cat1;
				MyBody.CollidesWith = Category.Cat2;
			}
			Position = pos;
			_currentHealth = Config.MaxPlayerHealth;
			_maxHealth = Config.MaxPlayerHealth;
			Lifes = Config.MaxLifes;
			BulletDelay = Config.BulletDelay;
			Ammo = new Item(world, new Vector2(-100, -100), false, 5, null);
			_deathPenalty = Config.PlayerDeathPenalty;
			_maxSpeed = Config.MaxPlayerSpeed;
			AmmoCount = Config.StartAmmo;
			_damage = Config.BasePlayerDamage;
		}

		/// <summary>
		/// Update of other clients' players
		/// </summary>
		/// <param name="position">position of player</param>
		/// <param name="rotation">rotation of player</param>
		public void Update(Vector2 position, float rotation, GameTime gameTime)
		{
			UpdateLifeStatus(gameTime);
		}

		/// <summary>
		/// update player
		/// </summary>
		/// <param name="direction">directional vector</param>
		public void Update(Vector2 direction, Vector2 mousePos, int left, GameTime gameTime)
		{
			if (Dead)
			{
				SemiDead = false;
			}
			if (!SemiDead)
			{
				if (direction.Length() > 0)
				{
					MyBody.ApplyLinearImpulse(direction * 3000);

				}
				else
				{
					MyBody.LinearVelocity = Vector2.Zero;
				}
				Rotation = Config.VectorToAngle(mousePos);
				if (Dead && _hasBody)
				{
					MyBody.CollidesWith = Category.Cat3;
				}
				if (!Dead && left % BulletDelay == 1 && left > 0)
				{
					if (AmmoCount > 0)
					{
						BulletEvent.Invoke(new Bullet(id, Position + (Config.AngleToVector(Rotation)), Rotation, Damage, Ammo.DType)); //TODO: speed dependent
						AmmoCount--;
					}
				}
			}
			else
			{
				MyBody.LinearVelocity = Vector2.Zero;
			}
			UpdateLifeStatus(gameTime);
			UpdateFrame(gameTime, direction);
		}

		/// <summary>
		/// Updates everything which has to do with life: damage taken, 
		/// </summary>
		/// <param name="gameTime"></param>
		private void UpdateLifeStatus(GameTime gameTime)
		{
			var temp = 0;
			if (Clr.R < 255)
			{
				temp = Clr.R + Config.DamageColorChange;
				Clr.R = (temp > 255) ? (byte)255 : (byte)temp;
			}
			if (Clr.G < 255)
			{
				temp = Clr.G + Config.DamageColorChange;
				Clr.G = (temp > 255) ? (byte)255 : (byte)temp;
			}
			if (Clr.B < 255)
			{
				temp = Clr.B + Config.DamageColorChange;
				Clr.B = (temp > 255) ? (byte)255 : (byte)temp;
			}
			for (var i = _takenDamage.Count - 1; i >= 0; i--)
			{
				if (_takenDamage[i].CurrentTick != _takenDamage[i].Ticks)
				{
					_takenDamage[i].CurrentInterval += (float)gameTime.ElapsedGameTime.TotalSeconds;
					if (_takenDamage[i].CurrentInterval > _takenDamage[i].Interval)
					{
						_takenDamage[i].CurrentTick++;
						_currentHealth -= _takenDamage[i].TickDamage;
						Clr = _takenDamage[i].Clr;
						_takenDamage[i].CurrentInterval = 0;
					}
				}
				else
				{
					_takenDamage.RemoveAt(i);
				}
			}
			if (WaitingForRespawn)
			{
				CurrentDeathPenalty = 0;
			}
			if (!Dead && !SemiDead)
			{
				if (_currentHealth <= 0.0f)
				{
					Lifes--;
					Clr = Color.White;
					_takenDamage.Clear();
					SemiDead = true;
				}
				if (Lifes == 0)
				{
					Dead = true;
				}
			}
			if (SemiDead)
			{
				CurrentDeathPenalty += (float)gameTime.ElapsedGameTime.TotalSeconds;
				if (CurrentDeathPenalty > _deathPenalty)
				{
					_currentHealth = Config.MaxPlayerHealth;
					SemiDead = false;
					WaitingForRespawn = true;
				}
			}
		}

		private void UpdateFrame(GameTime gameTime, Vector2 direction)
		{
			if (direction.Length() > 0)
			{
				frameTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
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
				_frame = _standardFrame;
			}
		}

		public void AddDamage(Other.Damage damage)
		{
			if (damage.Type != DamageType.Normal)
			{
				for (var i = _takenDamage.Count - 1; i >= 0; i--)
				{
					if (_takenDamage[i].Type == damage.Type)
					{
						if (_takenDamage[i].CurrentTick > 0)
						{
							_takenDamage[i] = damage;
						}
						return;
					}
				}
			}
			_takenDamage.Add(damage);
		}
	}
}