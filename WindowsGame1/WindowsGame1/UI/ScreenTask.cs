using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
	public abstract class ScreenTask
	{
		public bool Cursor;
		public SpriteFont Font1;
		public SpriteFont Font2;
		public SpriteFont Font3;

		public ScreenTask()
		{
		}

		public virtual void Initialize()
		{
		}

		/// <summary>
		/// Draws the phase.
		/// </summary>
		/// <param name="gameTime">Information about the game time.</param>
		/// <param name="sb">Spritebatch, needed to draw.</param>
		public virtual void Draw(GameTime gameTime, SpriteBatch sb)
		{
		}

		/// <summary>
		/// This method updates the world elements.
		/// </summary>
		/// <param name="gameTime">Information about the game time.</param>
		/// <param name="mousePos">Mouse position of the player.</param>
		/// <param name="leftClick">leftClick mousebutton state.</param>
		/// <param name="rightClick">rightClick mousebutton state.</param>
		/// <param name="keysPressed">keybinding states.</param>
		public virtual void Update(GameTime gameTime, Vector2 mousePos, int left, int right, int[] keysPressed)
		{
		}
	}
}
