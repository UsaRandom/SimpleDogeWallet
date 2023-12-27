
using DogecoinTerminal.Pages;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Linq;
using DogecoinTerminal.Common;
using DogecoinTerminal.Common.Components;
using System.Diagnostics.Metrics;
using DogecoinTerminal.QRDoge;

namespace DogecoinTerminal
{
	public class DogecoinTerminalGame : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		private VirtualScreen _screen;
		private AppButton _button;

		private Router _router;
		public FontSystem _fontSystem;


		public const int HEIGHT = 720;
		public const int WIDTH = 1280;

		public DogecoinTerminalGame()
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


			_router = new Router();

			Services.AddService(_router);
			Services.AddService<ITerminalService>(new TerminalService(this));
			Services.AddService<ITerminalSettingsService>(new TerminalSettingsService());


			Services.AddService<IDogecoinService>(new QRDogecoinService(this));


			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			_fontSystem.AddFont(File.ReadAllBytes(@"Content\ComicNeue-Bold.ttf"));

			Images.Load(GraphicsDevice);

			_screen.Load(this, _fontSystem);


			_router.AddRoute("home", new UnlockTerminalPage(this));
			_router.AddRoute("pin", new PinCodePage(this));
			_router.AddRoute("msg", new MessagePage(this));
			_router.AddRoute("wallets", new WalletListPage(this));
			_router.AddRoute("wallet", new WalletPage(this));
			_router.AddRoute("scanqr", new QRScannerPage(this));
			_router.AddRoute("displayqr", new DisplayQRPage(this));
			_router.AddRoute("codes", new BackupCodePage(this));
			_router.AddRoute("settings", new TerminalSettingsPage(this));
			_router.AddRoute("transactionsettings", new TransactionSettingsPage(this));

			_router.Route("home", null, false);

		}

		protected override void Update(GameTime gameTime)
		{
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