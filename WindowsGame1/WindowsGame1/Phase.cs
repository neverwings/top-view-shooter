using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
	public abstract class Phase
	{
		public bool Cursor;

		public Phase()
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
		/// <param name="left">left mousebutton state.</param>
		/// <param name="right">right mousebutton state.</param>
		/// <param name="keysPressed">keybinding states.</param>
		public virtual void Update(GameTime gameTime, Vector2 mousePos, int left, int right, int[] keysPressed)
		{
		}
	}
}
