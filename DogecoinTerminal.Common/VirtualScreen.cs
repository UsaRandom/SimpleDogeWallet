using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace DogecoinTerminal.Common
{
    /// <summary>
    /// basically a 0-100 grid that is projected to the physical screen.
    /// also provides a simplified interface for drawing to the screen.
    /// </summary>
    public class VirtualScreen
	{
		private SpriteBatch _spriteBatch;
		private GraphicsDevice _graphicsDevice;

		private int _width;
		private int _height;

		private decimal _heightScale;
		private decimal _widthScale;

		private FontSystem _fontSystem;

		public void Init(GraphicsDevice graphicsDevice, int height, int width)
		{
			_graphicsDevice = graphicsDevice;

			_height = height;
			_width = width;

			_heightScale = _height / 100M;
			_widthScale = _width / 100M;
		}

		public void Load(SpriteBatch spriteBatch)
		{
			_spriteBatch = spriteBatch;
			_fontSystem = new FontSystem();
			_fontSystem.AddFont(File.ReadAllBytes(@"Content\ComicNeue-Bold.ttf"));
		}


		public Point WindowCoordToVirtualCoord(Point screenCoord)
		{
			return new Point((int)(((float)screenCoord.X/(float)_width)*100.0), (int)(((float)screenCoord.Y/(float)_height) * 100.0));
		}


		public void DrawRectangle(TerminalColor color,
								  Point start, Point end)
		{
			_spriteBatch.Draw(color.Texture,
				new Rectangle(
					(int)Math.Round(start.X * _widthScale),
					(int)Math.Round(start.Y * _heightScale),
					(int)Math.Round((end.X - start.X) * _widthScale),
					(int)Math.Round((end.Y - start.Y) * _heightScale)),
				Color.White);
		}


		public void DrawText(string text, TerminalColor color, int scale, Point pos)
		{
			SpriteFontBase font = _fontSystem.GetFont((float)scale * (float)Math.Min(_widthScale, _heightScale));

			var textSize = font.MeasureString(text);

			_spriteBatch.DrawString(font, text,
				new Vector2((int)(pos.X * _widthScale) - (textSize.X) / 2,
					        (int)(pos.Y * _heightScale) - (textSize.Y) / 2),
				Color.White);

		}


		internal void DrawImage(
			Texture2D image,
			Point start,
			Point imgDim,
			Point imgOgDim)
		{

			//determine target size
			var target = (width: imgDim.X * _widthScale,
						 height: imgDim.Y * _heightScale);



			_spriteBatch.Draw(image,
					new Vector2((int)( (start.X - (imgDim.X / 2)) * _widthScale),
					(int)((start.Y - (imgDim.Y / 2)) * _heightScale)),
					null, Color.White, 0, Vector2.Zero, 
					new Vector2((float)target.width/imgOgDim.X,
								(float)target.width / imgOgDim.X), 
					
					SpriteEffects.None, 0
					);
		}
	}
}
