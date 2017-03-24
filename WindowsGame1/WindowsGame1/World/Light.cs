using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Ziggyware;
using Shadows2D;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace WindowsGame1
{
	public class Light
	{
		public Vector2 LightPosition;
		public LightsFX LightsFX;
		public ShadowMapResolver ShadowMapResolver;
		public ShadowMapResolver ShadowMapResolverBullets;
		public LightSource LightSource;
		public List<LightBullet> Bullets;
		public ShadowCasterMap ShadowMap;
		public RenderTarget2D ScreenLights;
		public RenderTarget2D ScreenGround;
		public TVSGame Game;
		public SpriteBatch sb;
		private Vector2 MyPosition;

		public Light(Vector2 PlayerPos)
		{
			MyPosition = Vector2.Zero;
			Game = TVSGame.Instance;
			sb = new SpriteBatch(Game.GraphicsDevice);
			LightsFX = new LightsFX(Loader.LoadEffect("resolveShadowsEffect"), Loader.LoadEffect("reductionEffect"), Loader.LoadEffect("2xMultiBlend"));
			ShadowMapResolver = new ShadowMapResolver(Game.GraphicsDevice, LightsFX, 128);
			LightSource = new LightSource(Game.graphics, Config.PlayerLightRadius, LightAreaQuality.High, Config.PlayerLightColor);
			Bullets = new List<LightBullet>();
			ShadowMap = new ShadowCasterMap(PrecisionSettings.VeryHigh, Game.graphics, sb);
			LightPosition = new Vector2(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2); //PlayerPos;
			ScreenLights = new RenderTarget2D(Game.GraphicsDevice, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height);
			ScreenGround = new RenderTarget2D(Game.GraphicsDevice, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height);
			ShadowMapResolverBullets = new ShadowMapResolver(Game.GraphicsDevice, LightsFX, 128);
		}

		public void Update(Vector2 myPos, GameTime gameTime)
		{
			MyPosition = myPos;
			foreach (var lb in Bullets)
			{
				if (lb.Remove)
				{
					lb.Timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
					lb.Light.Radius -= (int)(lb.Light.Radius * (0.2f / Config.BulletLightDecay));
					if (lb.Timer < 0.1)
					{
						Bullets.Remove(lb);
						return;
					}
				}
			}
		}

		public void CreateNewLightSource(Bullet b)
		{
			var newLight = new LightSource(Game.graphics, 50, LightAreaQuality.High, b.Clr);
			Bullets.Add(new LightBullet(newLight, b));
			b.DisposeEvent += DisposeLight;
		}

		public void Draw(Vector2 WorldPos)
		{
		   // Game.GraphicsDevice.Clear(Color.CornflowerBlue);
			ShadowMapResolver.ResolveShadows(ShadowMap, LightSource, PostEffect.LinearAttenuation_BlurHigh, 1.0f, LightPosition);//CurveAttenuation_BlurHigh
			foreach (var lb in Bullets)
			{
				ShadowMapResolverBullets.ResolveShadows(ShadowMap, lb.Light, PostEffect.CurveAttenuation_BlurHigh, 1.0f, GetMyScreenPos(lb.MyBullet.Position));
			}
			Game.GraphicsDevice.SetRenderTarget(ScreenLights);
			{
				Game.GraphicsDevice.Clear(Color.Black);
				sb.Begin(SpriteSortMode.Deferred, BlendState.Additive);
				{
					LightSource.Draw(sb);
					foreach (var lb in Bullets)
					{
						lb.Light.Draw(sb);
					}
				}
				sb.End();
			}
			Game.GraphicsDevice.SetRenderTarget(ScreenGround);
			Game.GraphicsDevice.Clear(Color.Black);
		}

		private void DisposeLight(Bullet b)
		{
			foreach (var lb in Bullets)
			{
				if (lb.MyBullet == b)
				{
					lb.Remove = true;
					lb.Timer = Config.BulletLightDecay;
				}
			}
		}

		private Vector2 GetMyScreenPos(Vector2 myPos)
		{
			var xHalf = TVSGame.Resolution.X / 2;
			var yHalf = TVSGame.Resolution.Y / 2;

			var temp = Vector2.Zero;
			temp.X = xHalf - (MyPosition.X - (myPos.X));
			temp.Y = yHalf - (MyPosition.Y - (myPos.Y));
			return temp;
		}
	}

	public class LightBullet
	{
		public LightSource Light;
		public Bullet MyBullet;
		public float Timer = 0.0f;
		public bool Remove = false;

		public LightBullet(LightSource ls, Bullet b)
		{
			Light = ls;
			MyBullet = b;
		}
	}
}
