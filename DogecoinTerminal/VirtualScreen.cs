using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal
{
	/// <summary>
	/// basically a 0-100 grid that is projected to the physical screen.
	/// also provides a simplified interface for drawing to the screen.
	/// </summary>
	internal class VirtualScreen
	{
		private SpriteBatch _spriteBatch;
		private GraphicsDevice _graphicsDevice;

		private SpriteFont _font;

		private int _width;
		private int _height;

		private decimal _heightScale;
		private decimal _widthScale;

		public void Init(GraphicsDevice graphicsDevice, int height, int width)
		{
			_graphicsDevice = graphicsDevice;

			TerminalColor.Init(_graphicsDevice);
			
			_height = height;
			_width = width;


			_heightScale = _height / 100M;
			_widthScale = _width / 100M;
		}

		public void Load(Game1 game)
		{
			_font = game.Content.Load<SpriteFont>("File"); // Use the name of your sprite font file here instead of 'Score'.

		}

		public void Update(AppPage page)
		{

		}



		public void Draw(SpriteBatch spriteBatch, AppPage page)
		{
			_spriteBatch = spriteBatch;

			page.Draw(this);
			
		}



		public void DrawRectangle(TerminalColor color,
								  (int x, int y) start, (int x, int y) end)
		{
			_spriteBatch.Draw(color.Texture,
				new Rectangle(
					(int)Math.Round(start.x * _widthScale),
					(int)Math.Round(start.y * _heightScale),
					(int)Math.Round((end.x - start.x) * _widthScale),
					(int)Math.Round((end.y - start.y) * _heightScale)),
				color.Color);
		}


		public void DrawText(string text, TerminalColor color, (int x, int y) pos)
		{
			_spriteBatch.DrawString(_font, text,
				new Vector2(
					(int)(pos.x * _widthScale),
					(int)(pos.y * _heightScale)	
					),
				
				color.Color);

		}




	}
}
