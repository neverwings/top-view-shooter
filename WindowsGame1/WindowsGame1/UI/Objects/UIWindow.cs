using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
	public class UIWindow : UIObject
	{ 
		public List<UIText> Text = new List<UIText>();
		public bool HasTexture, HasText;
		public Color Clr;

		public UIWindow(Vector2 position, Color color) : base()
		{
			Position = position;
			HasText = true;
			HasTexture = false;
			Clr = color;
		}
		public UIWindow(Vector2 position, string texture)
			: base()
		{
			Position = position;
			if (texture != "")
			{
				Texture = Loader.LoadGameTexture(texture);
				HasTexture = true;
			}
			HasText = false;
		}

		public void Update()
		{
			// TODO: implement how a UIWindow receives updates... custom style!
		}
	}
}