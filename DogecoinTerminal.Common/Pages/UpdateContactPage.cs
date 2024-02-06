using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing.QrCode;
using ZXing;
using System.Runtime.CompilerServices;

namespace DogecoinTerminal.Common.Pages
{
	[PageDef("Pages/Xml/UpdateContactPage.xml")]
	public class UpdateContactPage : PromptPage
	{
		private Contact _contact;

		private string _previousLabel;
		private string _previousAddress;
		private string _label;
		private string _address;

		private Texture2D _qrCodeImage;

		private GraphicsDevice _graphicsDevice;

		private TextInputControl _labelControl;
		private TextInputControl _addressControl;


		public UpdateContactPage(IPageOptions options, GraphicsDevice graphicsDevice, Navigation navigation, IClipboardService clipboardService) : base(options)
		{
			_contact = options.GetOption<Contact>("contact");

			_graphicsDevice = graphicsDevice;
			_address = _contact.Address;
			_label = _contact.Name;

			GetControl<TextControl>("TitleText").Text = _label;

			_labelControl = GetControl<TextInputControl>("LabelText");
			_addressControl = GetControl<TextInputControl>("AddressText");

			_labelControl.Text = _label;
			_addressControl.Text = _address;

			OnClick("BackButton", _ =>
			{
				Cancel();
			});

			OnClick("LabelPasteButton", _ =>
			{
				_label = _labelControl.Text = clipboardService.GetClipboardContents();
			});

			OnClick("AddressPasteButton", _ =>
			{
				_address = _addressControl.Text = clipboardService.GetClipboardContents();
			});


			OnClick("QRButton", async _ =>
			{
				await navigation.PushAsync<LoadingPage>();

				var qrScanResult = await navigation.PromptAsync<QRScannerPage>();

				if (qrScanResult.Response == PromptResponse.YesConfirm)
				{
					_address = _addressControl.Text = (string)qrScanResult.Value;
				}

				navigation.Pop();
			});


			OnClick("UpdateButton", _ =>
			{
				var newContact = new Contact();
				newContact.Address = _address;
				newContact.Name = _label;

				Submit(newContact);
			});

			UpdateQR();

		}

		~UpdateContactPage()
		{
			_qrCodeImage?.Dispose();
		}

		private void UpdateQR()
		{
			_qrCodeImage?.Dispose();
			_qrCodeImage = new Texture2D(_graphicsDevice, 480, 480);

			var barcodeWriter = new BarcodeWriterPixelData()
			{
				Format = BarcodeFormat.QR_CODE,
				Options = new QrCodeEncodingOptions
				{
					Width = 480,
					Height = 480,
					Margin = 0
				}
			};

			var pixels = barcodeWriter.Write(_address).Pixels;

			_qrCodeImage.SetData(pixels);
		}


		public override void Draw(GameTime gameTime, IServiceProvider services)
		{
			var screen = services.GetService<VirtualScreen>();

			screen.DrawImage(_qrCodeImage, new Point(50, 69), new Point(40, 40));

			base.Draw(gameTime, services);
		}

		public override void Update(GameTime gameTime, IServiceProvider services)
		{
			_address = GetControl<TextInputControl>("AddressText").Text?.Trim() ?? string.Empty;
			_label = GetControl<TextInputControl>("LabelText").Text?.Trim() ?? string.Empty;

			bool allValid = true;

			if (_previousAddress != _address)
			{
				var isValidP2pkh = Crypto.VerifyP2PKHAddress(_address);

				if(!isValidP2pkh)
				{
					GetControl<ButtonControl>("UpdateButton").Enabled = false;
					allValid = false;
				}
			}

			if(_previousLabel != _label)
			{
				if (string.IsNullOrWhiteSpace(_label))
				{
					GetControl<ButtonControl>("UpdateButton").Enabled = false;
					allValid = false;
				}
			}

			if((_previousLabel != _label ||
				_previousAddress != _address) && allValid)
			{
				GetControl<ButtonControl>("UpdateButton").Enabled = true;
			}


			_previousAddress = _address;
			_previousLabel = _label;
			base.Update(gameTime, services);
		}

	}
}
