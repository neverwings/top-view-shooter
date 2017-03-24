using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace WindowsGame1
{
	public class InputManager
	{
		private MouseState _mouseState;
		private int _left, _right;
		private KeyboardState _keyboardState;
		int[] hudKeys = new int[KeyBinding.Size.Y], gameKeys = new int[KeyBinding.Size.Y];

		public Input Update(GameTime gameTime)
		{
			_mouseState = Mouse.GetState();
			_left = (_mouseState.LeftButton == ButtonState.Pressed) ? _left + 1 : 0;
			_right = (_mouseState.RightButton == ButtonState.Pressed) ? _right + 1 : 0;
			_keyboardState = Keyboard.GetState();
			for (var i = 0; i < KeyBinding.Size.Y; i++)
			{
				if (_keyboardState.IsKeyDown(KeyBinding.GameBindings[0, i].key))
				{
					gameKeys[i]++;
				}
				else
				{
					gameKeys[i] = 0;
				}
				if (_keyboardState.IsKeyDown(KeyBinding.GameBindings[1, i].key))
				{
					hudKeys[i]++;
				}
				else
				{
					hudKeys[i] = 0;
				}
			}
			return new Input(new Vector2(_mouseState.X, _mouseState.Y), _left, _right, gameKeys, hudKeys);
		}
	}
}