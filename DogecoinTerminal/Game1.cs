using DogecoinTerminal.Components;
using DogecoinTerminal.Pages;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

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
			_router = new Router(new[]
			{
				("home", (AppPage)new StartPage()),
				("test", new TestPage())
			}) ;
		}

		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			_screen.Init(GraphicsDevice, 720, 1280);

			_router.Route("home", null, false);

			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			_screen.Load(this);

			// TODO: use this.Content to load your game content here
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			// TODO: Add your update logic here

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