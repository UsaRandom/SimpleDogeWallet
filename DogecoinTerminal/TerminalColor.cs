using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DogecoinTerminal
{
	internal class TerminalColor
	{
		private Texture2D _texture;
		private Color _color;

		private TerminalColor(GraphicsDevice graphicsDevice, byte r, byte g, byte b)
		{
			_color = new Color(r, g, b);
			_texture = new Texture2D(graphicsDevice, 1, 1);
			_texture.SetData(new[] { _color });
		}

		public Texture2D Texture { get { return _texture; } }

		public Color Color { get { return _color; } }


		private static TerminalColor _white;
		private static TerminalColor _blue;
		private static TerminalColor _green;
		private static TerminalColor _grey;
		private static TerminalColor _red;
		private static TerminalColor _darkGrey;
		private static TerminalColor _lightGrey;

		public static void Init(GraphicsDevice graphicsDevice)
		{
			_white = new TerminalColor(graphicsDevice, 0xff, 0xff, 0xff);
			_blue = new TerminalColor(graphicsDevice, 0x42, 0xa4, 0xff);
			_green = new TerminalColor(graphicsDevice, 0x2c, 0x93, 0x30);
			_grey = new TerminalColor(graphicsDevice, 0x22, 0x22, 0x22);
			_red = new TerminalColor(graphicsDevice, 0xff, 0x28, 0x28);
			_darkGrey = new TerminalColor(graphicsDevice, 0x33, 0x33, 0x33);
			_lightGrey = new TerminalColor(graphicsDevice, 0x67, 0x67, 0x67);
		}


		public static TerminalColor White
		{
			get
			{
				return _white;
			}
		}

		public static TerminalColor Blue
		{
			get
			{
				return _blue;
			}
		}

		public static TerminalColor Green
		{
			get
			{
				return _green;
			}
		}

		public static TerminalColor Grey
		{
			get
			{
				return _grey;
			}
		}

		public static TerminalColor Red
		{
			get
			{
				return _red;
			}
		}

		public static TerminalColor DarkGrey
		{
			get
			{
				return _darkGrey;
			}
		}

		public static TerminalColor LightGrey
		{
			get
			{
				return _lightGrey;
			}
		}

	}
}
