using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace WindowsGame1
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	/// 
	public class TVSGame: Microsoft.Xna.Framework.Game
	{
		public GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		public static Point Resolution = new Point(1024, 768);
		public static TVSGame Instance;
		public TVSGame()
		{
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = Resolution.X;
			graphics.PreferredBackBufferHeight = Resolution.Y;
			graphics.IsFullScreen = Config.IsFullscreen;
			Content.RootDirectory = "Content";
			KeyBinding.Initialize();
			Instance = this;
		}

		protected override void Initialize()
		{
			base.Initialize();
			Loader.Initialize(Content);
			spriteBatch = new SpriteBatch(GraphicsDevice);
			GeneralManager.Initialize();


		}

		protected override void LoadContent()
		{
		}

		protected override void UnloadContent()
		{
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			GeneralManager.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
			GraphicsDevice.Clear(Color.Black);
			GeneralManager.Draw(gameTime, spriteBatch);
		}
	}
}