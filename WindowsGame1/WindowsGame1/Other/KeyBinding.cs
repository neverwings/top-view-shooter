using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame1
{
	public static class KeyBinding
	{
		public static Point Size = new Point(2, 5);
		public static Binding[,] GameBindings = new Binding[Size.X, Size.Y];

		public static void Initialize()
		{
			GameBindings[1, 0] = new Binding(Keys.Escape, "Menu / Back");
			GameBindings[1, 1] = new Binding(Keys.OemTilde, "Menu / Back");
			GameBindings[0, 0] = new Binding(Keys.W, "Walk up");
			GameBindings[0, 1] = new Binding(Keys.S, "Walk down");
			GameBindings[0, 2] = new Binding(Keys.A, "Walk left");
			GameBindings[0, 3] = new Binding(Keys.D, "Walk right");
			GameBindings[0, 4] = new Binding(Keys.E, "Health drain");
		}
	}
}
