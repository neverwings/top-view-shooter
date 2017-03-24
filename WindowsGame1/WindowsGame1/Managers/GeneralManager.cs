using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
	public static class GeneralManager
	{
		private static HUDScreen _hud;
		private static GameScreen _game;
		private static InputManager _inputManager;
		private static Input _input;

		/// <summary>
		/// Initializes the General Manager.
		/// </summary>
		public static void Initialize()
		{
			_inputManager = new InputManager();
			_game = new GameScreen();
			_hud = new HUDScreen(_game.World);
		}

		/// <summary>
		/// Updates the phases when needed.
		/// </summary>
		/// <param name="gameTime">Information about the game time.</param>
		public static void Update(GameTime gameTime)
		{
			_input = _inputManager.Update(gameTime);
			_game.Update(gameTime, _input.MousePos, _input.Left, _input.Right, _input.GameKeys);
			_hud.Update(gameTime, _input.MousePos, _input.Left, _input.Right, _input.HudKeys);
		}

		/// <summary>
		/// Draws the different phases.
		/// </summary>
		/// <param name="gameTime">Information about the game time.</param>
		/// <param name="sb">Spritebatch, needed to draw.</param>
		public static void Draw(GameTime gameTime, SpriteBatch sb)
		{
			_game.Draw(gameTime, sb);
            sb.Begin();
			_hud.Draw(gameTime, sb);
            sb.End();
		}
	}
}