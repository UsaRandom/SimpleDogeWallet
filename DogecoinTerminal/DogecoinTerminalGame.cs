
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DogecoinTerminal.Common;
using Microsoft.Xna.Framework.Input;
using System;
using DogecoinTerminal.Pages;
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

            Strings.Current.SelectLanguage(Language.Languages["en"]);
			TerminalColor.Init(_graphics.GraphicsDevice);


			_screen.Init(_graphics, useFullScreen: false);

			_nav = new Navigation(Services);


			Services.AddService(Strings.Current);
            Services.AddService(_nav);
            Services.AddService(_screen);
            Services.AddService(new Images(GraphicsDevice));
			Services.AddService<ITerminalSettingsService>(new TerminalSettingsService());
			Services.AddService<ITerminalService>(new TerminalService(Services));


//            Services.AddService<IDogecoinService>(new QRDogecoinService(this));


            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _screen.Load(_spriteBatch);

            _nav.PushAsync<UnlockTerminalPage>();
        }

        private ButtonState lastButtonState = ButtonState.Released;

        protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			_nav.CurrentPage.Update(gameTime, Services);


			var mouseState = Mouse.GetState(Window);

            if(mouseState.LeftButton == ButtonState.Pressed &&
                lastButtonState == ButtonState.Released)
            {
                Messenger.Default.Send(
                    new UserClickMessage(
                        _screen.WindowCoordToVirtualCoord(
                            new Point(mouseState.X, mouseState.Y))));
            }

			lastButtonState = mouseState.LeftButton;


			base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(TerminalColor.Grey.Color);


            _spriteBatch.Begin();

            _nav.CurrentPage.Draw(gameTime, Services);

			_spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}