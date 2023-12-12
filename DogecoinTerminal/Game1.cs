using DogecoinTerminal.Components;
using DogecoinTerminal.Pages;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace DogecoinTerminal
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		private VirtualScreen _screen;
		private AppButton _button;

		private Router _router;


		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;

			_graphics.PreferredBackBufferHeight = 720;
			_graphics.PreferredBackBufferWidth = 1280;
			_screen = new VirtualScreen();
		}

		protected override void Initialize()
		{
			_screen.Init(GraphicsDevice, 720, 1280);


			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			_screen.Load(this);

			_router = new Router(new[]
			{
				("home", (AppPage)new UnlockTerminalPage()),
				("test", new TestPage())
			});

			_router.Route("home", null, false);

		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();


			_router.GetPage().Update();
			_screen.Update(_router.GetPage());

			
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(TerminalColor.Grey.Color);


			_spriteBatch.Begin();

			_screen.Draw(_spriteBatch, _router.GetPage());


			_spriteBatch.End();


			base.Draw(gameTime);
		}
	}
}