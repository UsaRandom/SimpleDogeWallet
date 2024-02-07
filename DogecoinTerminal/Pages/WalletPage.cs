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
    internal class WalletPage : Page, IReceiver<SPVNodeBlockInfo>, IReceiver<SPVUpdatedWalletMessage>
	{
		private const string SETTINGS_BUTTON_NAME = "SettingsButton";
		private const string LOCK_BUTTON_NAME = "LockButton";

		private Texture2D _qrCodeImage;

		private SimpleDogeWallet _wallet;
		private SimpleSPVNodeService _spvNode;

		public WalletPage(IPageOptions options, Navigation navigation, Strings strings, GraphicsDevice graphicsDevice, SimpleSPVNodeService spvNode) : base(options)
        {
			_wallet = options.GetOption<SimpleDogeWallet>("wallet");

			_spvNode = spvNode;

			Messenger.Default.Register<SPVNodeBlockInfo>(this);
			Messenger.Default.Register<SPVUpdatedWalletMessage>(this);


			_spvNode.SetWallet(_wallet);
			_spvNode.Start();

			
			var addressTextControl = GetControl<TextControl>("AddressText");
			addressTextControl.Text = _wallet.Address;


            var balanceTextControl = GetControl<TextControl>("BalanceText");
			balanceTextControl.Text = $"Đ {_wallet.GetBalance():#.###}";


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

			OnClick("ContactsButton", async _ =>
			{
				await navigation.PushAsync<ContactsPage>(("edit-mode", true));
			});

			UpdateSPVText();
		}

		~WalletPage()
		{
			_qrCodeImage?.Dispose();

			Messenger.Default.Deregister<SPVNodeBlockInfo>(this);
			Messenger.Default.Deregister<SPVUpdatedWalletMessage>(this);
		}

		private void UpdateSPVText()
		{

			GetControl<TextControl>("SPVNodeInfo").Text = $"{_spvNode.CurrentBlock.BlockHeight} @ {_spvNode.CurrentBlock.Timestamp.ToLocalTime()}";

		}


		public override void Draw(GameTime gameTime, IServiceProvider services)
		{
			var screen = services.GetService<VirtualScreen>();
			screen.DrawImage(_qrCodeImage, new Point(50, 60), new Point(40, 40));
			base.Draw(gameTime, services);
		}

		public void Receive(SPVUpdatedWalletMessage message)
		{
			var balanceTextControl = GetControl<TextControl>("BalanceText");
			balanceTextControl.Text = $"Đ {_wallet.GetBalance():#.###}";
		}

		public void Receive(SPVNodeBlockInfo message)
		{
			UpdateSPVText();
		}
	}
}
