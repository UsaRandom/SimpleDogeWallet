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


		public void SetWindowDim(GraphicsDeviceManager graphicsDeviceManager, bool useFullScreen, int width, int height)
		{
			graphicsDeviceManager.PreferredBackBufferWidth = width;
			graphicsDeviceManager.PreferredBackBufferHeight = height;

			graphicsDeviceManager.IsFullScreen = useFullScreen;

			_xPad = Math.Max(0, (width - height) / 2);
			_yPad = Math.Max(0, (height - width) / 2);
			graphicsDeviceManager.ApplyChanges();


			_graphicsDevice = graphicsDeviceManager.GraphicsDevice;

			_renderDim = Math.Min(graphicsDeviceManager.PreferredBackBufferHeight, graphicsDeviceManager.PreferredBackBufferWidth);


			_renderScale = _renderDim / 100M;
		}


		public void Init(GraphicsDeviceManager graphicsDeviceManager, bool useFullScreen)
		{
			SetWindowDim(graphicsDeviceManager,
							useFullScreen,
							graphicsDeviceManager.GraphicsDevice.Adapter.CurrentDisplayMode.Width - 200,
							graphicsDeviceManager.GraphicsDevice.Adapter.CurrentDisplayMode.Height - 200);
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
		} = 255;

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
			const float thickness = 3f;

			_spriteBatch.DrawLine(_xPad + (int)Math.Round(start.X * _renderScale), _yPad + (int)Math.Round(start.Y * _renderScale), 
				_xPad + (int)Math.Round(end.X * _renderScale), _yPad + (int)Math.Round(start.Y * _renderScale), color.Color, thickness);
			_spriteBatch.DrawLine(_xPad + (int)Math.Round(start.X * _renderScale), _yPad + (int)Math.Round(start.Y * _renderScale),
				_xPad + (int)Math.Round(start.X * _renderScale), _yPad + (int)Math.Round(end.Y * _renderScale), color.Color, thickness);

			//bottom line
			_spriteBatch.DrawLine(_xPad + (int)Math.Round(end.X * _renderScale)+1, _yPad + (int)Math.Round(end.Y * _renderScale)+1,
				_xPad + (int)Math.Round(start.X * _renderScale)-1, _yPad + (int)Math.Round(end.Y * _renderScale) + 1, color.Color, thickness);



			_spriteBatch.DrawLine(_xPad + (int)Math.Round(end.X * _renderScale), _yPad + (int)Math.Round(end.Y * _renderScale),
				_xPad + (int)Math.Round(end.X * _renderScale), _yPad + (int)Math.Round(start.Y * _renderScale), color.Color, thickness);
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


		public void DrawText(string text, TerminalColor color, float scale, Point pos, TextAnchor anchor = TextAnchor.Center)
		{
			if(color == null)
			{
				color = TerminalColor.White;
			}

			SpriteFontBase font = _fontSystem.GetFont((float)scale * (float)Math.Min(_renderScale, _renderScale));
			
			var textSize = font.MeasureString(text);

			Vector2 position;
			if(anchor == TextAnchor.TopLeft)
			{
				position = new Vector2(_xPad + (int)(pos.X * _renderScale) ,
							_yPad + (int)(pos.Y * _renderScale));
			}
			else
			{
				position = new Vector2(_xPad + (int)(pos.X * _renderScale) - (textSize.X) / 2,
							_yPad + (int)(pos.Y * _renderScale) - (textSize.Y) / 2);
			}

			_spriteBatch.DrawString(font, text, position, color.Color);

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
