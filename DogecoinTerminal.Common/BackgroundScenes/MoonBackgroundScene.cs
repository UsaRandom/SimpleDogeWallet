using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Common.BackgroundScenes
{
	public class MoonBackgroundScene : IBackgroundScene
	{
		private Texture2D _floatingObject;
		private Vector2 _objectPosition;
		private float _floatingObjectSpeed;

		private int _width;
		private int _height;

		private bool _rotate = false;

		private float _rotation = 0;
		private float _rotationSpeed = 0.01f;

		private float __floatingObjectScale = 0;


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




		private TimeSpan _idleTime = TimeSpan.Zero;
		private MouseState _lastMouseState = Mouse.GetState();



		public MoonBackgroundScene(IServiceProvider services, int width, int height)
		{
			_displayWidth = _width = width;
			_displayHeight = _height = height;

			// Set the initial object and star positions and speeds
			_objectPosition = new Vector2(-100, 0.52f * height);

			
			_rotationSpeed = (float)(0.02f * (new Random().NextDouble() - 0.5));
			_rotate = true;
			_floatingObjectSpeed = 0.04f;
			_moveXPerMS = 0.02f;

			var imageService = services.GetService<Images>();

			_tileTexture = imageService.GetImage("Content/stars.png");

			
			_textureWidth = _tileTexture.Width;
			_textureHeight = _tileTexture.Height;

			_floatingObject = imageService.GetImage("Content/ogfreshdoge.png");
			__floatingObjectScale = (0.14f * Math.Min(width, height))/ ((float)_floatingObject.Bounds.Width);

		}


		public void Draw(GameTime gameTime, SpriteBatch spriteBatch, IServiceProvider services)
		{

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



			spriteBatch.Draw(_floatingObject, _objectPosition, null, Color.White, _rotation, _floatingObject.Bounds.Center.ToVector2(), __floatingObjectScale, SpriteEffects.None, 0);
		}

		public void Update(GameTime gameTime, IServiceProvider services)
		{
			//var newMouseState = Mouse.GetState();

			//if(newMouseState == _lastMouseState)
			//{
			//	_idleTime = _idleTime.Add(gameTime.ElapsedGameTime);
			//}
			//else
			//{
			//	services.GetService<VirtualScreen>().Opacity = 255;
			//	_idleTime = TimeSpan.Zero;
			//}

			//if(_idleTime.TotalSeconds > 5)
			//{
			//	services.GetService<VirtualScreen>().Opacity = 0;
			//}

			//_lastMouseState = newMouseState;



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


			if(_rotate)
			{

				_rotation += _rotationSpeed;

				if (_rotation >= Math.PI * 2)
				{
					_rotation = 0;
				}
			}
			else
			{
				_rotation = 0;
			}
			_objectPosition.X -= _floatingObjectSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

			if (_objectPosition.X < -(_floatingObject.Bounds.Width * __floatingObjectScale) - 250)
			{

				var imageService = services.GetService<Images>();

				var scale = 0.2f;
				var randomInt = new Random().Next(0, 100);

				//the moon can make text hard to read, so we reduce it's screen precense compared to others.
				if(randomInt < 5)
				{
					_floatingObject = imageService.GetImage("Content/dogemoon.png");
					scale = 0.25f;
					_rotationSpeed = 0;
					_rotate = false;
				}
				else if(randomInt < 55)
				{
					_floatingObject = imageService.GetImage("Content/ogfreshdoge.png");
					scale = 0.14f;
					_rotationSpeed = (float)(0.02f * (new Random().NextDouble() - 0.5));
					_rotate = true;

				}
				else
				{
					_floatingObject = imageService.GetImage("Content/cheems.png");
					scale = 0.14f;
					_rotationSpeed = (float)(0.06f * (new Random().NextDouble() - 0.5));
					_rotate = true;
				}

				__floatingObjectScale = (scale * Math.Min(_displayWidth, _displayHeight)) / ((float)Math.Max(_floatingObject.Bounds.Width, _floatingObject.Bounds.Height));

				_objectPosition.X = _width + (_floatingObject.Bounds.Width * __floatingObjectScale);

				_objectPosition.Y = (int)(new Random().NextDouble() * _displayHeight);


			}


		}
	}
}
