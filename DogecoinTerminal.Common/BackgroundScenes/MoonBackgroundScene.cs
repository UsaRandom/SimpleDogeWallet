using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Common.BackgroundScenes
{
	public class MoonBackgroundScene : IBackgroundScene
	{
		private Texture2D _moonTexture;
		private Vector2 _moonPosition;
		private float _moonSpeed;

		private int _width;
		private int _height;

		private float _moonScale = 0;


		private int _displayWidth;
		private int _displayHeight;
		private int _textureWidth;
		private int _textureHeight;

		private Texture2D _tileTexture;

		//how much to move in direction per ms passed
		private float _moveXPerMS = 0;
		private float _moveYPerMS = 0;

		//current offset, for x and y for tiling, resets when at display limits
		private float _xOffset = 0;
		private float _yOffset = 0;

		public MoonBackgroundScene(IServiceProvider services, int width, int height)
		{
			_displayWidth = _width = width;
			_displayHeight = _height = height;

			// Set the initial moon and star positions and speeds
			_moonPosition = new Vector2(width, 0.12f * height);
	
			_moonSpeed = 0.04f;
			_moveXPerMS = 0.02f;

			var imageService = services.GetService<Images>();

			_moonTexture = imageService.GetImage("Content/dogemoon.png");
			_tileTexture = imageService.GetImage("Content/stars.png");

			
			_textureWidth = _tileTexture.Width;
			_textureHeight = _tileTexture.Height;

			_moonScale = (0.2f * Math.Min(width, height))/ ((float)_moonTexture.Bounds.Width);

		}


		public void Draw(GameTime gameTime, SpriteBatch spriteBatch, IServiceProvider services)
		{
			// Draw the moon and stars



			// Calculate the number of tiles to draw in each direction
			int numTilesX = (int)Math.Ceiling(_displayWidth / (float)_textureWidth);
			int numTilesY = (int)Math.Ceiling(_displayHeight / (float)_textureHeight);

			// Draw the background tiles
			for (int y = 0; y <= numTilesY; y++)
			{
				for (int x = 0; x <= numTilesX; x++)
				{
					spriteBatch.Draw(_tileTexture, new Vector2(x * _textureWidth - _xOffset, y * _textureHeight - _yOffset), Color.White);
				}
			}



			spriteBatch.Draw(_moonTexture, _moonPosition, null, Color.White, 0f, Vector2.Zero, _moonScale, SpriteEffects.None, 0);
		}

		public void Update(GameTime gameTime, IServiceProvider services)
		{


			float elapsedMS = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

			// Update the offsets for the background
			_xOffset += _moveXPerMS * elapsedMS;
			_yOffset += _moveYPerMS * elapsedMS;

			// Reset the offsets when they reach the edge of the screen
			if (_xOffset > _textureWidth)
			{
				_xOffset = 0;
			}
			if (_yOffset > _textureHeight)
			{
				_yOffset = 0;
			}
			if (_xOffset < 0)
			{
				_xOffset = _textureWidth;
			}
			if (_yOffset < 0)
			{
				_yOffset = _textureHeight;
			}


			// Update the moon and star positions
			_moonPosition.X -= _moonSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

			// Wrap the moon and star positions around the screen
			if (_moonPosition.X < -(_moonTexture.Bounds.Width * _moonScale) - 10)
			{
				_moonPosition.X = _width + (_moonTexture.Bounds.Width * _moonScale);

				_moonPosition.Y = (int)(new Random().NextDouble() * _displayHeight);
			}


		}
	}
}
