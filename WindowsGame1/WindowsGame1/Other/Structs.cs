using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
	public struct Input
	{
		public Vector2 MousePos;
		public int Left, Right;
		public int[] GameKeys, HudKeys;
		public Input(Vector2 mPos, int left, int right, int[] gameKeys, int[] hudKeys)
		{
			MousePos = mPos;
			Left = left;
			Right = right;
			GameKeys = gameKeys;
			HudKeys = hudKeys;
		}
	}

	public struct Binding
	{
		public Keys key;
		public string keyName
		{
			get
			{
				if (key.ToString().Length > 3)
				{
					return key.ToString().Substring(0, 3);
				}
				else
				{
					return key.ToString();
				}
			}
		}
		public string Description;
		public Binding(Keys k, string d)
		{
			key = k;
			Description = d;
		}
	}
}
