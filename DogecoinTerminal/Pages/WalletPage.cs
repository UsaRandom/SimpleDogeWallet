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
using OpenCvSharp;

namespace DogecoinTerminal.Pages
{
    [PageDef("Pages/Xml/WalletPage.xml")]
    internal class WalletPage : Page, IReceiver<SPVNodeBlockInfo>, IReceiver<SPVUpdatedWalletMessage>, IReceiver<SPVSyncCompletedMessage>
	{

		private Texture2D _qrCodeImage;

		private SimpleDogeWallet _wallet;
		private SimpleSPVNodeService _spvNode;

		private ButtonControl _sendButton;

		public WalletPage(IPageOptions options, IServiceProvider services,  IClipboardService clipboard, ITerminalSettings settings, Navigation navigation, Strings strings, GraphicsDevice graphicsDevice, SimpleSPVNodeService spvNode) : base(options)
        {
			_wallet = options.GetOption<SimpleDogeWallet>("wallet");

			_spvNode = spvNode;

			Messenger.Default.Register<SPVNodeBlockInfo>(this);
			Messenger.Default.Register<SPVUpdatedWalletMessage>(this);
			Messenger.Default.Register<SPVSyncCompletedMessage>(this);


			_spvNode.SetWallet(_wallet);
			_spvNode.Start();

			_sendButton = GetControl<ButtonControl>("SendButton");

			if(!_spvNode.SyncCompleted)
			{
				_sendButton.BackgroundColor = TerminalColor.Grey;
				_sendButton.StringDef = "terminal-wallet-syncing";
			}
			else
			{
				_sendButton.BackgroundColor = TerminalColor.Green;
				_sendButton.StringDef = "terminal-wallet-send";
			}
			
			var addressTextControl = GetControl<TextControl>("AddressText");
			addressTextControl.Text = _wallet.Address;


            var balanceTextControl = GetControl<TextControl>("BalanceText");
			balanceTextControl.Text = $"Đ {(_wallet.GetBalance() ):#,0.000}";
			GetControl<TextControl>("PendingBalanceText").Text = $"(Đ {(_wallet.GetPendingBalance()):#,0.000})";


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
			OnClick("CopyButton", async _ =>
			{
				clipboard.SetClipboardContents(_wallet.Address);
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


			OnClick("SendButton", async _ =>
			{
				if(!_spvNode.SyncCompleted)
				{
					return;
				}

				await navigation.PushAsync<LoadingPage>();

				var destinationResult = await navigation.PromptAsync<ContactsPage>(("edit-mode", false));

				if(destinationResult.Response != PromptResponse.YesConfirm)
				{
					navigation.Pop();
					return;
				}

				var target = (Contact)destinationResult.Value;

				var maxSpend = _wallet.GetBalance() - _wallet.GetPendingBalance();

				maxSpend -= (_wallet.UTXOs.Count - _wallet.PendingSpentUTXOs.Count) * settings.GetDecimal("fee-per-utxo");

				var amountResult = await navigation.PromptAsync<NumPadPage>(("value-mode", true),
																			("title", "terminal-sendamount-title"),
																			("hint", $"(Đ {maxSpend:#,0.000})"));


				

				if(amountResult.Response != PromptResponse.YesConfirm ||
					!decimal.TryParse(amountResult?.Value.ToString(), out decimal amountToSend) ||
					amountToSend < settings.GetDecimal("dust-limit") ||
					amountToSend > maxSpend)
				{
					navigation.Pop();
					return;
				}

				var sendYesNo = await navigation.PromptAsync<YesNoPage>(("message", $"Đ {amountToSend:#,0.000}  -> {target.Name} ({target.ShortAddress})"));

				if(sendYesNo.Response != PromptResponse.YesConfirm)
				{
					navigation.Pop();
					return;
				}


				var numPadResponse = await navigation.PromptAsync<NumPadPage>(("title", strings["terminal-send-confirmpin"]));

				if (numPadResponse.Response != PromptResponse.YesConfirm ||
					!SimpleDogeWallet.TryOpen(numPadResponse.Value.ToString(),
												services,
												out SimpleDogeWallet simpleWallet))
				{
					navigation.Pop();
					return;
				}

				//

				var transaction = new DogecoinTransaction(services, _wallet);

				if(!transaction.Send(target.Address, amountToSend))
				{
					await navigation.PromptAsync<ShortMessagePage>(("message", "Error creating transaction."));
					navigation.Pop();
					return;
				}
				if (!transaction.Sign())
				{
					await navigation.PromptAsync<ShortMessagePage>(("message", "Error signing transaction."));
					navigation.Pop();
					return;
				}


				



				var rawTx = transaction.GetRawTransaction();

				if (!LibDogecoinContext.Instance.BroadcastRawTransaction(rawTx))
				{
					await navigation.PromptAsync<ShortMessagePage>(("message", "Error broadcasting transaction."));
					navigation.Pop();
				}
				else
				{
					transaction.Commit();
					GetControl<TextControl>("PendingBalanceText").Text = $"(Đ {(_wallet.GetPendingBalance()):#,0.000})";

					await navigation.PromptAsync<ShortMessagePage>(("message", "Transaction Broadcast!"));
					navigation.Pop();
				}


			});



			UpdateSPVText();
		}

		~WalletPage()
		{
			_qrCodeImage?.Dispose();

			Messenger.Default.Deregister<SPVNodeBlockInfo>(this);
			Messenger.Default.Deregister<SPVUpdatedWalletMessage>(this);
			Messenger.Default.Deregister<SPVSyncCompletedMessage>(this);
		}

		private void UpdateSPVText()
		{

			GetControl<TextControl>("SPVNodeInfo").Text = $"{_spvNode.CurrentBlock.BlockHeight} @ {_spvNode.CurrentBlock.Timestamp.ToLocalTime()}";

		}


		public override void Draw(GameTime gameTime, IServiceProvider services)
		{
			var screen = services.GetService<VirtualScreen>();
			screen.DrawImage(_qrCodeImage, new Microsoft.Xna.Framework.Point(50, 60), new Microsoft.Xna.Framework.Point(40, 40));
			base.Draw(gameTime, services);
		}

		public void Receive(SPVUpdatedWalletMessage message)
		{
			var balanceTextControl = GetControl<TextControl>("BalanceText");
			balanceTextControl.Text = $"Đ {_wallet.GetBalance():#.###}";
			GetControl<TextControl>("PendingBalanceText").Text = $"(Đ {(_wallet.GetPendingBalance()):#,0.000})";

		}

		public void Receive(SPVNodeBlockInfo message)
		{
			UpdateSPVText();
		}

		public void Receive(SPVSyncCompletedMessage message)
		{
			_sendButton.BackgroundColor = TerminalColor.Green;
			_sendButton.StringDef = "terminal-wallet-send";
		}
	}
}
