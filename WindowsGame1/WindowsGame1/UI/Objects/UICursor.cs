using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
	public class UICursor: UIObject
	{
		private int _frame = 0;
		private int _frameDelay = 5;
		private int _frameCount = 2;
		public override Rectangle BoundingBox
		{
			get
			{
				return new Rectangle(0, (int)Size.Y * (_frame / _frameDelay), (int)Size.X, (int)Size.Y);
			}
		}
		public UICursor(int size, int delay, int count)
		{
			Texture = Loader.LoadGameTexture("HUD/cursor");
			Size = new Vector2(size, size);
			_frameDelay = delay;
			_frameCount = count;
		}

		public override void Update(Vector2 position)
		{
			Position = position;
			FrameUpdate();
		}

		private void FrameUpdate()
		{
			_frame++;
			_frame %= (_frameDelay * _frameCount) - 1;
		}
	}
}
