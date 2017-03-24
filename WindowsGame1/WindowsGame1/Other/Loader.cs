using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace WindowsGame1
{
	public static class Loader
	{
		private static ContentManager _content;
		public static SpriteFont Font1
		{
			get
			{
				return _content.Load<SpriteFont>(@"Font\Font1");
			}
		}
		public static SpriteFont Font2
		{
			get
			{
				return _content.Load<SpriteFont>(@"Font\Font2");
			}
		}
		public static SpriteFont Font3
		{
			get
			{
				return _content.Load<SpriteFont>(@"Font\Font3");
			}
		}

		/// <summary>
		/// Initializes the loader class.
		/// </summary>
		/// <param name="content">contentManager of XNA</param>
		public static void Initialize(ContentManager content)
		{
			_content = content;
		}

		/// <summary>
		/// This Method Loads Textures for created objects.
		/// </summary>
		/// <param _name="TextureName">name of objectTexture (include the subdirectory inside game when needed) NO EXTENSIONS</param>
		/// <returns>texture of object</returns>
		public static Texture2D LoadGameTexture(string TextureName)
		{
			return _content.Load<Texture2D>(@"Textures\" + TextureName);
		}
		public static Effect LoadEffect(string EffectName)
		{
			return _content.Load<Effect>(@"Effects\" + EffectName);
		}
	}
}