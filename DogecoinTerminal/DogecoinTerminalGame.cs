
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DogecoinTerminal.Common;
using Microsoft.Xna.Framework.Input;
using DogecoinTerminal.Common.BackgroundScenes;
using System.Xml.Linq;
using DogecoinTerminal.Common.Controls;
using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using DogecoinTerminal.Common.Interop;
using MonoGame.Framework.Utilities;
using DogecoinTerminal.Common.Pages;

namespace DogecoinTerminal
{
    public class DogecoinTerminalGame : Game
    {
		private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
		public FontSystem _fontSystem;

        public VirtualScreen _screen;

        private Navigation _nav;

		private ButtonControl _devButton;

		private ITerminalSettings _settings;

		private SelectedControlVisitor _textInputSelector;

		//dev tools
		private ButtonState lastButtonState = ButtonState.Released;
		private MoveHandlesControlVisitor _moveHandler;

		private IClipboardService _clipboardService;
		private IUserInputService _userInputService;

		public DogecoinTerminalGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _screen = new VirtualScreen();
            _fontSystem = new FontSystem();

		}

        protected override void Initialize()
        {

			_settings = new TerminalSettings();

			_settings.Set("terminal-devmode", false);
			_settings.Set("terminal-background", true);

			Strings.Current.SelectLanguage(
				Language.Languages[_settings.GetString("language", "eng")]);
			
			
			TerminalColor.Init(_graphics.GraphicsDevice);


            _screen.Init(_graphics, useFullScreen: _settings.GetBool("terminal-fullscreen", false));

			_nav = new Navigation(Services);

			_userInputService = new UserInputService(this);
			_clipboardService = new TextCopyClipboardService();


			Services.AddService(Strings.Current);
            Services.AddService(_nav);
			Services.AddService(_screen);
			Services.AddService(new ContactService());
			Services.AddService(new Images(GraphicsDevice));
			Services.AddService(_settings);
			Services.AddService(GraphicsDevice);
			Services.AddService<Game>(this);
			Services.AddService<IServiceProvider>(Services);
			Services.AddService(_userInputService);


			Services.AddService<IClipboardService>(_clipboardService);


			//text input selector
			_textInputSelector = new SelectedControlVisitor(_screen);

			//background
			_background = new MoonBackgroundScene(Services, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);


			//dev tools
			var devButtonEl = new XElement("button");
			devButtonEl.Add(new XAttribute("Name", "DevButton"));
			devButtonEl.Add(new XAttribute("StartPosition", "0,97"));
			devButtonEl.Add(new XAttribute("EndPosition", "4,99"));
			devButtonEl.Add(new XAttribute("BackgroundColor", "Blue"));
			devButtonEl.Add(new XAttribute("ForegroundColor", "White"));
			devButtonEl.Add(new XAttribute("TextSize", "2"));
			devButtonEl.Add(new XAttribute("Text", "copy"));
			_devButton = new ButtonControl(devButtonEl);
			_moveHandler = new MoveHandlesControlVisitor(_screen);

			base.Initialize();
        }

        protected override void LoadContent()
        {

			_spriteBatch = new SpriteBatch(GraphicsDevice);

            _screen.Load(_spriteBatch);

			//Just for testing wallet creation.

			SimpleDogeWallet.ClearWallet();

			_nav.PushAsync<ContactsPage>();
        }

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			_textInputSelector.UpdateMouse();
			foreach (var control in _nav.CurrentPage.Controls)
			{
				control.AcceptVisitor(_textInputSelector);
			}


			_nav.CurrentPage.Update(gameTime, Services);

            if (IsActive)
            {
				var mouseState = Mouse.GetState(Window);

				if (mouseState.LeftButton == ButtonState.Pressed &&
					lastButtonState == ButtonState.Released)
				{
					Messenger.Default.Send(
						new UserClickMessage(
							_screen.WindowCoordToVirtualCoord(
								new Point(mouseState.X, mouseState.Y))));

					if (_settings.GetBool("terminal-devmode", false))
					{
						if (_devButton.ContainsPoint(_screen.WindowCoordToVirtualCoord(
								new Point(mouseState.X, mouseState.Y))))
						{

							var xmlExporter = new XmlExporterControlVisitor();

							foreach(var control in _nav.CurrentPage.Controls)
							{
								control.AcceptVisitor(xmlExporter);
							}

							_clipboardService.SetClipboardContents(xmlExporter.PageElement.ToString());
						}
					}

				}

				lastButtonState = mouseState.LeftButton;



				if (_settings.GetBool("terminal-devmode", false))
				{
					_moveHandler.UpdateMouse();

					foreach (var control in _nav.CurrentPage.Controls)
					{
						control.AcceptVisitor(_moveHandler);
					}

				}
			}


            if (_settings.GetBool("terminal-background", false))
            {
				_background.Update(gameTime, Services);
			}


			base.Update(gameTime);
        }


        IBackgroundScene _background;
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(TerminalColor.Grey.Color);


			_spriteBatch.Begin();


            if (_settings.GetBool("terminal-background", false))
            {
				_background.Draw(gameTime, _spriteBatch, Services);
			}

			_nav.CurrentPage.Draw(gameTime, Services);

            if(_settings.GetBool("terminal-devmode", false))
            {
				var handles = new DrawHandlesControlVisitor(_spriteBatch, _screen);

				foreach (var control in _nav.CurrentPage.Controls)
				{
					control.AcceptVisitor(handles);
				}

				_devButton.Draw(gameTime, Services);
				_screen.DrawText(_screen.WindowCoordToVirtualCoord(Mouse.GetState().Position).ToString(), TerminalColor.White, 2, new Point(50, 98));
			}



			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}