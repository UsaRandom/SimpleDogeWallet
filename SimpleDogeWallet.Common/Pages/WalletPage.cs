using SimpleDogeWallet.Common;
using SimpleDogeWallet.Common.Pages;
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
using System.Diagnostics;

namespace SimpleDogeWallet.Pages
{
    [PageDef("Pages/Xml/WalletPage.xml")]
    public class WalletPage : Page, IReceiver<SPVNodeBlockInfo>, IReceiver<SPVUpdatedWalletMessage>, IReceiver<UpdateSendButtonMessage>, IReceiver<UpdateSPVTextMessage>
	{

		private Texture2D _qrCodeImage;

		private SimpleSPVNodeService _spvNode;
		private FeeEstimator _feeEstimator;

		private ButtonControl _sendButton;
		private IServiceProvider _services;

		public WalletPage(IPageOptions options, IServiceProvider services,  IClipboardService clipboard, ITerminalSettings settings, Navigation navigation, Strings strings, GraphicsDevice graphicsDevice, SimpleSPVNodeService spvNode, FeeEstimator feeEstimator) : base(options)
        {
			_feeEstimator = feeEstimator;
			_services = services;

			var isNew = options.GetOption<bool>("is-new", false);

			_spvNode = spvNode;

			Messenger.Default.Register<SPVNodeBlockInfo>(this);
			Messenger.Default.Register<SPVUpdatedWalletMessage>(this);
			Messenger.Default.Register<UpdateSendButtonMessage>(this);
			Messenger.Default.Register<UpdateSPVTextMessage>(this);


			_spvNode.Start(isNew);

			_sendButton = GetControl<ButtonControl>("SendButton");
			
			UpdateSendButton();


			var addressTextControl = GetControl<TextControl>("AddressText");
			addressTextControl.Text = SimpleDogeWallet.Instance.Address;


            var balanceTextControl = GetControl<TextControl>("BalanceText");
			balanceTextControl.Text = $"Đ {(SimpleDogeWallet.Instance.GetBalance() ):#,0.000}";

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
				await navigation.PushAsync<SettingsPage>(("wallet", options.GetOption<SimpleDogeWallet>("wallet")));
			});
			OnClick("CopyButton", async _ =>
			{
				clipboard.SetClipboardContents(SimpleDogeWallet.Instance.Address);
			});

			OnClick("LockButton", async _ =>
			{
			//	await navigation.TryInsertBeforeAsync<UnlockTerminalPage, WalletPage>();
				//await navigation.PopToPage<UnlockTerminalPage>();
			});

			OnClick("ContactsButton", async _ =>
			{
				await navigation.PushAsync<ContactsPage>(("edit-mode", true));
			});


			OnClick("SendButton", async _ =>
			{
				if(!_spvNode.SyncCompleted || SimpleDogeWallet.Instance.PendingAmount != 0)
				{
					return;
				}

				PromptResult destinationResult = default;
				await navigation.PushAsync<LoadingPage>();

				destinationResult = await navigation.PromptAsync<ContactsPage>(("edit-mode", false));

				if(destinationResult.Response != PromptResponse.YesConfirm)
				{
					navigation.Pop();
					return;
				}

				var target = (Contact)destinationResult.Value;


				var maxSpend = SimpleDogeWallet.Instance.GetBalance();


				var rate = _services.GetService<SimpleSPVNodeService>().EstimatedRate;
				var feeCoeff = _services.GetService<ITerminalSettings>().GetDecimal("fee-coeff");



				var ratePerByte = rate * feeCoeff;



				maxSpend -= ratePerByte * (225 +  (SimpleDogeWallet.Instance.UTXOs.Count - 1) * 148);


				maxSpend = Math.Round(maxSpend, (int)Math.Ceiling(Math.Log10(1 / (double)settings.GetDecimal("dust-limit"))), MidpointRounding.ToZero);

				var amountResult = await navigation.PromptAsync<NumPadPage>(("value-mode", true),
																			("title", strings.GetString("terminal-sendamount-title")),
																			("hint", $"(Đ {maxSpend:#,0.000})"));


				

				if(amountResult.Response != PromptResponse.YesConfirm ||
					!decimal.TryParse(amountResult?.Value.ToString(), out decimal amountToSend) ||
					amountToSend < settings.GetDecimal("dust-limit") ||
					amountToSend > maxSpend)
				{
					navigation.Pop();
					return;
				}

				var transaction = new DogecoinTransaction(services, SimpleDogeWallet.Instance);

				if (!transaction.Send(target.Address, amountToSend))
				{
					await navigation.PromptAsync<ShortMessagePage>(("message", "Error creating transaction."));
					navigation.Pop();
					return;
				}



				var sendYesNo = await navigation.PromptAsync<TransactionConfirmPage>(("tx", transaction), ("target", target));

				if(sendYesNo.Response != PromptResponse.YesConfirm)
				{
					navigation.Pop();
					return;
				}


				//var numPadResponse = await navigation.PromptAsync<NumPadPage>(("title", strings["terminal-send-confirmpin"]));

				//if (numPadResponse.Response != PromptResponse.YesConfirm ||
				//	!SimpleDogeWallet.TryOpen(numPadResponse.Value.ToString()))
				//{
				//	navigation.Pop();
				//	return;
				//}

				//


				//force garbage collection after signing
				if (!transaction.Sign())
				{
					await navigation.PromptAsync<ShortMessagePage>(("message", "Error signing transaction."));
					navigation.Pop();
					return;
				}

				//if(navigation.CurrentPage is LoadingPage)
				//{
				//	((LoadingPage)navigation.CurrentPage).StringDef = "terminal-loading-text-broadcasting";
				//}
				await Task.Run(async () =>
				{
					_spvNode.Pause();
					Debug.WriteLine("Stopped SPV Node");

					var rawTx = transaction.GetRawTransaction();
					var txId = Crypto.GetTransactionIdFromRaw(rawTx);

					Debug.WriteLine($"Raw Transaction: {rawTx}");
					Debug.WriteLine($"Transaction Id (hash): {txId}");
					Debug.WriteLine("Attempting to Broadcast Transaction!");



					SimpleDogeWallet.Instance.PendingTxHash = txId;
					SimpleDogeWallet.Instance.PendingAmount = transaction.Total;
                    SimpleDogeWallet.Instance.PendingTxTime = DateTime.Now;

                    if (navigation.CurrentPage is LoadingPage)
					{
						navigation.Pop();
					}

					if(!await transaction.BroadcastAsync())
					{
						SimpleDogeWallet.Instance.PendingTxHash = string.Empty;
                        SimpleDogeWallet.Instance.PendingAmount = 0;
                        SimpleDogeWallet.Instance.PendingTxTime = DateTime.MaxValue;

                    }





                    _spvNode.Resume();
					Debug.WriteLine("Starting SPV Node");

				});

			});



			UpdateSPVText();
		}

		~WalletPage()
		{
			_qrCodeImage?.Dispose();

			Messenger.Default.Deregister<SPVNodeBlockInfo>(this);
			Messenger.Default.Deregister<SPVUpdatedWalletMessage>(this);
			Messenger.Default.Deregister<UpdateSendButtonMessage>(this);
			Messenger.Default.Deregister<UpdateSPVTextMessage>(this);
		}

		private void UpdateSPVText()
		{
			GetControl<TextControl>("SPVNodeInfo").Text = $"{_spvNode.CurrentBlock.BlockHeight}/~{_spvNode.EstimatedHeight} @ {_spvNode.CurrentBlock.Timestamp.ToLocalTime()} - {_feeEstimator.EstimatedFee}";

		}

        public override void Update(GameTime gameTime, IServiceProvider services)
        {
			SimpleDogeWallet.Instance.UpdatePending();
            UpdateSendButton();
            UpdateSPVText();

            base.Update(gameTime, services);
        }


        public override void Draw(GameTime gameTime, IServiceProvider services)
		{
			var screen = services.GetService<VirtualScreen>();
			screen.DrawImage(_qrCodeImage, new Microsoft.Xna.Framework.Point(50, 60), new Microsoft.Xna.Framework.Point(40, 40));
			base.Draw(gameTime, services);
		}

		public void Receive(UpdateSPVTextMessage message)
		{
			UpdateSPVText();
		}

		public void Receive(SPVUpdatedWalletMessage message)
		{
			UpdateSendButton();
		}

		public void Receive(SPVNodeBlockInfo message)
		{
			UpdateSPVText();
		}

		public void Receive(UpdateSendButtonMessage message)
		{
			UpdateSendButton();
		}

		private void UpdateSendButton()
		{
			GetControl<TextControl>("BalanceText").Text = $"Đ {SimpleDogeWallet.Instance.GetBalance():#,0.000}";

			if(SimpleDogeWallet.Instance.PendingAmount == 0)
			{
				GetControl<TextControl>("PendingText").Text = string.Empty;
			}
			else
			{
				GetControl<TextControl>("PendingText").Text = $"(Đ -{SimpleDogeWallet.Instance.PendingAmount:#,0.000})";
			}

			if (!_spvNode.SyncCompleted || !_spvNode.IsRunning)
			{
				_sendButton.BackgroundColor = TerminalColor.Grey;
				_sendButton.StringDef = "terminal-wallet-syncing";
			}
			else if(SimpleDogeWallet.Instance.PendingAmount != 0)
			{
				_sendButton.BackgroundColor = TerminalColor.Red;
				_sendButton.StringDef = "terminal-wallet-pending";
			}
			else
			{
				_sendButton.BackgroundColor = TerminalColor.Green;
				_sendButton.StringDef = "terminal-wallet-send";
			}
			
		}
	}
}
