using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WindowsGame1
{
	public static class Config
	{
		public static int xReso = 400;
		public static int yReso = 400;
		#region World

		public static int TileSize = 30; // 30
		public static int Players = 3; // 4
		public static int WorldTilesPerPlayer = 10; // 25
		public static int AmmoPerPlayer = 3;
		public static int Seed = 5968407; // number which influences the outcome of the random number generator.
		public static int TileQuantifier = 3; // total tile amount will be divided by this number.
		public static float AmmoRespawnTime = 40;
		public static int AdditionalAmmo = 20;
		public static int AdditionalHealth = 25;

		#endregion
		#region PlayerInfo

		public static int MaxPlayerHealth = 100;
		public static int MaxLifes = 2;
		public static int RespawnDistance = 4;
		public static float MaxPlayerSpeed = 200; // 5
		public static float PlayerDeathPenalty = 3.1f;
		public static Color PlayerLightColor = Color.LightGoldenrodYellow;
		public static int PlayerLightRadius = 400;
		public static float PlayerAcceleration = 50.05f;
		public static byte DamageColorChange = 5;
		#region Bullet info

		public static int StartAmmo = 100;
		public static int BulletSpeed = 10;
		public static int MaxAmmo = 100;
		public static int BulletDelay = 10;
		public static float BulletLightDecay = 2.0f;
		public static float BasePlayerDamage = 10f;

		#endregion

		#endregion
		#region GameInfo

		public static string Version = "0.0.1";
		public static bool IsFullscreen = false;
		public static int MaxPlayers = 20;

		#endregion
		#region Methods
		
		public static Vector2 AngleToVector(float angle)
		{
			return new Vector2((float)Math.Sin(angle), -(float)Math.Cos(angle));
		}

		public static float VectorToAngle(Vector2 vector)
		{
			return (float)Math.Atan2(vector.X, -vector.Y);
		}

		#endregion
	}
}
