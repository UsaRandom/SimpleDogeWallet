using DogecoinTerminal.Components;
using DogecoinTerminal.Pages;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
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
		public FontSystem _fontSystem;


		public const int HEIGHT = 720;
		public const int WIDTH = 1280;

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;

			_graphics.PreferredBackBufferHeight = HEIGHT;
			_graphics.PreferredBackBufferWidth = WIDTH;
			_screen = new VirtualScreen();
			_fontSystem = new FontSystem();
		}

		protected override void Initialize()
		{
			_screen.Init(GraphicsDevice, HEIGHT, WIDTH);


			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			_fontSystem.AddFont(File.ReadAllBytes(@"Content\ComicNeue-Bold.ttf"));
		

			Images.DogeImage = Content.Load<Texture2D>("dogedrawn");

			_screen.Load(this);

			_router = new Router(new[]
			{
				("home", (AppPage)new UnlockTerminalPage()),
				("test", new TestPage()),
				("pin", new PinCodePage()),
				("msg", new MessagePage()),
				("wallets", new WalletListPage()),
				("wallet", new WalletPage())
			});

			_router.Route("home", null, false);

		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();


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