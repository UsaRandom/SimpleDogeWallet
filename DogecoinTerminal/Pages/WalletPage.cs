using DogecoinTerminal.Common;
using DogecoinTerminal.Common.Pages;
using Lib.Dogecoin;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using ZXing.QrCode;
using ZXing;
using Microsoft.Xna.Framework;

namespace DogecoinTerminal.Pages
{
    [PageDef("Pages/Xml/WalletPage.xml")]
    internal class WalletPage : Page
	{
		private const string SETTINGS_BUTTON_NAME = "SettingsButton";
		private const string LOCK_BUTTON_NAME = "LockButton";

		private Texture2D _qrCodeImage;

		public WalletPage(IPageOptions options, Navigation navigation, Strings strings, GraphicsDevice graphicsDevice) : base(options)
        {
            var addressTextControl = GetControl<TextControl>("AddressText");
			addressTextControl.Text = options.GetOption<string>("address");// slot.Address;


            var balanceTextControl = GetControl<TextControl>("BalanceText");
			balanceTextControl.Text = "Đ 0.000";// slot.CalculateBalance();


			_qrCodeImage = new Texture2D(graphicsDevice, 480, 480);

			var barcodeWriter = new BarcodeWriterPixelData()
			{
				Format = BarcodeFormat.QR_CODE,
				Options = new QrCodeEncodingOptions
				{
					Width = 480,
					Height = 480, Margin = 0
				}
			};

			var pixels = barcodeWriter.Write(addressTextControl.Text).Pixels;

			_qrCodeImage.SetData(pixels);


			OnClick("SettingsButton", async _ =>
			{
				await navigation.PushAsync<SettingsPage>();
			});
			OnClick("LockButton", async _ =>
			{
				await navigation.TryInsertBeforeAsync<UnlockTerminalPage, WalletPage>();
				await navigation.PopToPage<UnlockTerminalPage>();
			});
		}

		public override void Draw(GameTime gameTime, IServiceProvider services)
		{
			var screen = services.GetService<VirtualScreen>();
			screen.DrawImage(_qrCodeImage, new Point(50, 60), new Point(45, 45));
			base.Draw(gameTime, services);
		}
	}
}
