using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
	public class HUDScreen: ScreenTask
	{
		private UIWindow _window;
		private UIWindow _debugWindow;
		#region HealthBar

		private UIWindow _healthContainer;
		private UIWindow _healthBar;
		private UIWindow _HPBar;
		private UIWindow _HPContainer;


		#endregion
		#region TeamBar

		private UIWindow _teamContainer;
		private UIWindow _usBar;
		private UIWindow _themBar;

		#endregion
		private UIWindow _ammoWindow;

		private float _frameRate;
		private UICursor _cursor;
		private int _inputTest;
		private Arena _arena;

		public HUDScreen(Arena arena)
		{
			_HPContainer = new UIWindow(new Vector2(TVSGame.Resolution.X - 125/* / 2 - (125.0f / 2)*/, /*TVSGame.Resolution.Y - 221*/0), "HUD/HPContainer"); // 125, 221 are the dimensions of the texture.
			_HPBar = new UIWindow(new Vector2(TVSGame.Resolution.X - 125/* / 2 - (125.0f / 2)*/, /*TVSGame.Resolution.Y - 221*/0), "HUD/HP");
			_window = new UIWindow(new Vector2(40, 40), Color.SandyBrown);
			_ammoWindow = new UIWindow(new Vector2(40, 75), Color.SandyBrown);
			_healthContainer = new UIWindow(new Vector2((TVSGame.Resolution.X / 2) - 50, 30), "HUD/Container");
			_healthBar = new UIWindow(new Vector2(_healthContainer.Position.X + 1, _healthContainer.Position.Y + 1), "HUD/HPBar");
			_healthBar.Size.X = _healthContainer.Texture.Width - 2;
			_teamContainer = new UIWindow(new Vector2((TVSGame.Resolution.X / 2) - 50, 45), "HUD/Container");
			_usBar = new UIWindow(new Vector2(_teamContainer.Position.X + 1, _teamContainer.Position.Y + 1), "HUD/TeamOneBar");
			_usBar.Size.X = (_teamContainer.Texture.Width - 1) / 2;
			_themBar = new UIWindow(new Vector2(_teamContainer.Position.X + (_teamContainer.Texture.Width) / 2, _teamContainer.Position.Y + 1), "HUD/TeamTwoBar");
			_themBar.Size.X = (_teamContainer.Texture.Width - 1) / 2;
#if DEBUG
			_debugWindow = new UIWindow(new Vector2(0, 0), Color.Yellow);
#endif
			Font1 = Loader.Font1;
			Font2 = Loader.Font2;
			Font3 = Loader.Font3;
			_cursor = new UICursor(20, 3, 3);
			_arena = arena;
			InitializeUI();
		}

		public override void Draw(GameTime gameTime, SpriteBatch sb)
		{
			_frameRate = (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (!_arena.Me.Dead)
			{
				base.Draw(gameTime, sb);
				var hpbarY = _HPBar.Size.Y * (_arena.Me.CurrentHealth / Config.MaxPlayerHealth);
				if (float.IsInfinity(hpbarY))
				{
					hpbarY = 0;
				}
				#region Healthbar Coloring

				var healthColor = Color.DarkOliveGreen;
				if (hpbarY < 221 * 0.3f)
				{
					healthColor = Color.Red;
				}
				else if (hpbarY < 221 * 0.6f)
				{
					healthColor = Color.Orange;
				}
				else if (hpbarY < 221 * 0.8f)
				{
					healthColor = Color.Peru;
				}

				#endregion
				sb.Draw(_HPContainer.Texture, new Rectangle((int)_HPContainer.Position.X, (int)_HPContainer.Position.Y, _HPContainer.Texture.Width, _HPContainer.Texture.Height), Color.White);
				sb.Draw(_HPBar.Texture, new Rectangle((int)_HPBar.Position.X, (int)_HPBar.Position.Y + (_HPBar.Texture.Height - (int)hpbarY), _HPBar.Texture.Width, (int)hpbarY), new Rectangle(0, 221 - (int)hpbarY, _HPBar.Texture.Width, (int)hpbarY), healthColor);
				sb.Draw(_teamContainer.Texture, new Rectangle((int)_teamContainer.Position.X, (int)_teamContainer.Position.Y, _teamContainer.Texture.Width, _teamContainer.Texture.Height), Color.White);
				sb.Draw(_usBar.Texture, new Rectangle((int)_usBar.Position.X, (int)_usBar.Position.Y, (int)_usBar.Size.X, _usBar.Texture.Height), Color.Blue);
				sb.Draw(_themBar.Texture, new Rectangle((int)_themBar.Position.X, (int)_themBar.Position.Y, (int)_themBar.Size.X, _themBar.Texture.Height), Color.Green);
				sb.Draw(_cursor.Texture, new Rectangle((int)_cursor.Position.X, (int)_cursor.Position.Y, (int)_cursor.Size.X, (int)_cursor.Size.Y), _cursor.BoundingBox, Color.White, 0.0f, new Vector2(_cursor.Size.X / 2, _cursor.Size.Y / 2), SpriteEffects.None, 0.0f);
				
				foreach (var t in _window.Text)
				{
					sb.DrawString(Font2, t.Text, t.Position, t.MyColor);
				}
#if DEBUG
				foreach (var t in _debugWindow.Text)
				{
					sb.DrawString(Font1, t.Text, t.Position, t.MyColor);
				}
#endif
				if (_arena.Me.CurrentDeathPenalty > 0)
				{
					sb.DrawString(Font1, ((int)(5 - _arena.Me.CurrentDeathPenalty)).ToString(), new Vector2(TVSGame.Resolution.X / 2, TVSGame.Resolution.Y / 2), Color.White);
				}
			}
		}

		public override void Update(GameTime gameTime, Vector2 mousePos, int left, int right, int[] keysPressed)
		{
			_cursor.Update(mousePos);
			// TEST
			_inputTest = keysPressed[1];
			if (_inputTest == 1)
			{
				_arena.Team2[0].Lifes--;
			}
			// TEST
			base.Update(gameTime, mousePos, left, right, keysPressed);
			UpdateWindow();
#if DEBUG
			UpdateDebugWindow();
#endif
		}

		private void InitializeUI()
		{
			_HPBar.Size = new Vector2(_HPBar.Texture.Width, _HPBar.Texture.Height);
			_window.Text.Add(new UIText("Lifes: " + _arena.Me.Lifes.ToString(), new Vector2(40, 40), _window.Clr));
			var s = "Health";
			_window.Text.Add(new UIText(s, new Vector2(TVSGame.Resolution.X / 2 - Font2.MeasureString(s).X / 2, 3), _window.Clr));
			_window.Text.Add(new UIText("Ammo: " + _arena.Me.AmmoCount.ToString(), new Vector2(40, 75), _ammoWindow.Clr));
#if DEBUG
			_debugWindow.Text.Add(new UIText("Escape Button timer: " + _inputTest.ToString(), new Vector2(700, 40), _debugWindow.Clr));
			_debugWindow.Text.Add(new UIText("Framerate: " + (1 / _frameRate), new Vector2(700, 60), _debugWindow.Clr));
#endif
		}

		private void UpdateWindow()
		{
			//_healthBar.Size.X = (_healthContainer.Texture.Width - 2) * (_arena.Me.CurrentHealth / Config.MaxPlayerHealth);
			_usBar.Size.X = ((_teamContainer.Texture.Width - 2) / 2) * (_arena.Team1Lifes / ((_arena.Team1.Count) * Config.MaxLifes));
			_themBar.Size.X = (_arena.Team2.Count > 0) ? ((_teamContainer.Texture.Width - 2) / 2) * (_arena.Team2Lifes / ((_arena.Team2.Count) * Config.MaxLifes)) : 0;
			_themBar.Position.X = _teamContainer.Position.X + (_teamContainer.Texture.Width - _themBar.Size.X) - 0.1f;
			_window.Text[0].Text = "Lifes: " + _arena.Me.Lifes.ToString();
			_window.Text[2].Text = "Ammo: " + _arena.Me.AmmoCount.ToString();
		}

		private void UpdateDebugWindow()
		{
			_debugWindow.Text[0].Text = "Escape Button timer: " + _inputTest.ToString();
			_debugWindow.Text[1].Text = "Framerate: " + (1 / _frameRate).ToString("0.0");
		}
	}

	public class UIText
	{
		public string Text;
		public Vector2 Position;
		public Color MyColor;
		public UIText(string text, Vector2 pos, Color clr)
		{
			Text = text;
			Position = pos;
			MyColor = clr;
		}
	}
}