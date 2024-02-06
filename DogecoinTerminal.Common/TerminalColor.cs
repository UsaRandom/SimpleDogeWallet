using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DogecoinTerminal.Common
{
    public class TerminalColor
    {
        private Texture2D _texture;
        private Color _color;

        private TerminalColor(string name, GraphicsDevice graphicsDevice, byte r, byte g, byte b, byte a = byte.MaxValue)
        {
            Name = name;
            _color = new Color(r, g, b, a);
            _texture = new Texture2D(graphicsDevice, 1, 1);
            _texture.SetData(new[] { _color });
        }


        public Texture2D Texture { get { return _texture; } }

        public Color Color { get { return _color; } }

        public string Name { get; private set; }

        private static TerminalColor _white;
        private static TerminalColor _blue;
        private static TerminalColor _green;
        private static TerminalColor _grey;
        private static TerminalColor _red;
        private static TerminalColor _darkGrey;
        private static TerminalColor _lightGrey;

        public static void Init(GraphicsDevice graphicsDevice)
        {
            _white = new TerminalColor("White", graphicsDevice, 0xff, 0xff, 0xff);
            _blue = new TerminalColor("Blue", graphicsDevice, 0x00, 0x85, 0xff); 
			_green = new TerminalColor("Green", graphicsDevice, 0x2C, 0x93, 0x30);
			_darkGrey = new TerminalColor("DarkGrey", graphicsDevice, 0x22, 0x22, 0x22);
            _red = new TerminalColor("Red", graphicsDevice, 0xff, 0x28, 0x28);
            _grey = new TerminalColor("Grey", graphicsDevice, 0x33, 0x33, 0x33);
            _lightGrey = new TerminalColor("LightGrey", graphicsDevice, 0x67, 0x67, 0x67);
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
