using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame;
using System;
using System.IO;

namespace DogecoinTerminal.Common
{
    /// <summary>
    /// basically a 0-100 grid that is projected to the physical screen.
    /// provides a simplified interface for drawing to the screen.
    /// </summary>
    public class VirtualScreen
	{
		private SpriteBatch _spriteBatch;
		private GraphicsDevice _graphicsDevice;

		private int _renderDim;

		private decimal _renderScale;

		private int _xPad;
		private int _yPad;

		private FontSystem _fontSystem;

		public void Init(GraphicsDeviceManager graphicsDeviceManager, bool useFullScreen)
		{
			var displayWidth = graphicsDeviceManager.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
			var displayHeight = graphicsDeviceManager.GraphicsDevice.Adapter.CurrentDisplayMode.Height;


			if (useFullScreen)
			{
				graphicsDeviceManager.PreferredBackBufferWidth = displayWidth;
				graphicsDeviceManager.PreferredBackBufferHeight = displayHeight;
				graphicsDeviceManager.IsFullScreen = true;

				_xPad = Math.Max(0, (displayWidth - displayHeight) / 2);
				_yPad = Math.Max(0, (displayHeight - displayWidth) / 2);
			}
			else
			{
				//if not fullscreen, we render as a box, 100 pixels less than the smallest dim on the screen
				var renderDim = Math.Min(displayWidth, displayHeight) - 200;

				graphicsDeviceManager.PreferredBackBufferHeight = renderDim;
				graphicsDeviceManager.PreferredBackBufferWidth = renderDim;
				_xPad = 0;
				_yPad = 0;
			}

			graphicsDeviceManager.ApplyChanges();


			_graphicsDevice = graphicsDeviceManager.GraphicsDevice;

			_renderDim = Math.Min(graphicsDeviceManager.PreferredBackBufferHeight, graphicsDeviceManager.PreferredBackBufferWidth);


			_renderScale = _renderDim / 100M;
			Opacity = 255;
		}

		public float RenderScale
		{
			get
			{
				return (float)_renderScale;
			}
		}

		public int Opacity
		{
			get;
			set;
		}

		private Color GetOpacityColor()
		{
			return new Color(Opacity, Opacity, Opacity, Opacity);
		}

		public void Load(SpriteBatch spriteBatch)
		{
			_spriteBatch = spriteBatch;
			_fontSystem = new FontSystem();
			_fontSystem.AddFont(File.ReadAllBytes(@"Content\ComicNeue-Bold.ttf"));
			_fontSystem.AddFont(File.ReadAllBytes(@"Content\NotoSansJP-VariableFont_wght.ttf"));
			_fontSystem.AddFont(File.ReadAllBytes(@"Content\NotoSansKR-VariableFont_wght.ttf"));
			_fontSystem.AddFont(File.ReadAllBytes(@"Content\NotoSansSC-VariableFont_wght.ttf"));
			_fontSystem.AddFont(File.ReadAllBytes(@"Content\NotoSansTC-VariableFont_wght.ttf"));
		}


		public Point WindowCoordToVirtualCoord(Point screenCoord)
		{

			return new Point((int)(((float)(screenCoord.X-_xPad)/(float)_renderDim)*100.0), (int)(((float)(screenCoord.Y-_yPad)/(float)_renderDim) * 100.0));
		}

		public Point VirtualCoordToWindowCoord(Point virtualCoord)
		{
			return new Point(_xPad + (int)Math.Round(virtualCoord.X * _renderScale),
							 _yPad + (int)Math.Round(virtualCoord.Y * _renderScale));
		}

		public void DrawRectangleBorder(TerminalColor color, Point start, Point end)
		{

			_spriteBatch.DrawLine(_xPad + (int)Math.Round(start.X * _renderScale), _yPad + (int)Math.Round(start.Y * _renderScale), 
				_xPad + (int)Math.Round(end.X * _renderScale), _yPad + (int)Math.Round(start.Y * _renderScale), color.Color);
			_spriteBatch.DrawLine(_xPad + (int)Math.Round(start.X * _renderScale), _yPad + (int)Math.Round(start.Y * _renderScale),
				_xPad + (int)Math.Round(start.X * _renderScale), _yPad + (int)Math.Round(end.Y * _renderScale), color.Color);
			_spriteBatch.DrawLine(_xPad + (int)Math.Round(end.X * _renderScale), _yPad + (int)Math.Round(end.Y * _renderScale),
				_xPad + (int)Math.Round(start.X * _renderScale), _yPad + (int)Math.Round(end.Y * _renderScale), color.Color);
			_spriteBatch.DrawLine(_xPad + (int)Math.Round(end.X * _renderScale), _yPad + (int)Math.Round(end.Y * _renderScale),
				_xPad + (int)Math.Round(end.X * _renderScale), _yPad + (int)Math.Round(start.Y * _renderScale), color.Color);
		}

		public void DrawRectangle(TerminalColor color,
								  Point start, Point end)
		{
			_spriteBatch.Draw(color.Texture,
				new Rectangle(
					_xPad + (int)Math.Round(start.X * _renderScale),
					_yPad + (int)Math.Round(start.Y * _renderScale),
					(int)Math.Round((end.X - start.X) * _renderScale),
					(int)Math.Round((end.Y - start.Y) * _renderScale)),
				GetOpacityColor());
		}


		public void DrawText(string text, TerminalColor color, int scale, Point pos)
		{
			SpriteFontBase font = _fontSystem.GetFont((float)scale * (float)Math.Min(_renderScale, _renderScale));

			var textSize = font.MeasureString(text);

			_spriteBatch.DrawString(font, text,
				new Vector2(_xPad + (int)(pos.X * _renderScale) - (textSize.X) / 2,
					        _yPad + (int)(pos.Y * _renderScale) - (textSize.Y) / 2),
				GetOpacityColor());

		}

		public void DrawSprite(
			Texture2D image,
			Rectangle sourceRectangle,
			Point start,
			Point imgDim)
		{

			//determine target size
			var target = (width: imgDim.X * _renderScale,
						 height: imgDim.Y * _renderScale);



			_spriteBatch.Draw(image,
					new Vector2(
						_xPad + (int)((start.X - (imgDim.X / 2)) * _renderScale),
					_yPad + (int)((start.Y - (imgDim.Y / 2)) * _renderScale)),
					sourceRectangle, GetOpacityColor(), 0, Vector2.Zero,
					new Vector2((float)target.width / sourceRectangle.Width,
								(float)target.height / sourceRectangle.Height),

					SpriteEffects.None, 0
					);
		}

		public void DrawImage(
			Texture2D image,
			Point start,
			Point imgDim)
		{

			//determine target size
			var target = (width: imgDim.X * _renderScale,
						 height: imgDim.Y * _renderScale);



			_spriteBatch.Draw(image,
					new Vector2(
						_xPad + (int)( (start.X - (imgDim.X / 2)) * _renderScale),
					_yPad + (int)((start.Y - (imgDim.Y / 2)) * _renderScale)),
					null, GetOpacityColor(), 0, Vector2.Zero, 
					new Vector2((float)target.width/ image.Bounds.Width,
								(float)target.height / image.Bounds.Height), 
					
					SpriteEffects.None, 0
					);
		}
	}
}
