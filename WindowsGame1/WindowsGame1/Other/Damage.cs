using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WindowsGame1.Other
{
	public class Damage
	{
		public int Ticks;
		public int CurrentTick = 0;
		public float CurrentInterval;
		public float Interval;
		public Color Clr;
		public DamageType Type;
		private float TotalDamage;
		public float TickDamage
		{
			get
			{
				return TotalDamage / Ticks;
			}
		}

		public Damage(float damage)
		{
			Clr = Color.Red;
			Ticks = 1;
			Interval = 0;
			Type = DamageType.Normal;
			TotalDamage = damage;
		}

		public Damage(float damage, int ticks, float interval, DamageType type)
		{
			TotalDamage = damage;
			Ticks = ticks;
			Interval = interval;
			CurrentInterval = Interval;
			switch (type)
			{
				case DamageType.Poison:
				Clr = Color.Green;
				break;
				case DamageType.Water:
				Clr = Color.Blue;
				break;
			}
			Type = type;
		}
	}
}
