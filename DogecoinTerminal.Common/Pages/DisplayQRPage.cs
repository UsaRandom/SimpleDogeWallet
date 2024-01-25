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

namespace DogecoinTerminal.Common.Pages
{
	[PageDef("Pages/Xml/DisplayQRPage.xml")]
	public class DisplayQRPage : PromptPage
	{

		private Texture2D _image;

		public DisplayQRPage(IPageOptions options, GraphicsDevice graphicsDevice) : base(options)
		{
			var qr = options.GetOption<string>("qr");
			var msg = options.GetOption<string>("message");


			var msgControl = GetControl<TextControl>("MessageText");
			msgControl.Text = msg;

			OnClick("NextButton", _ =>
			{

				//PromptPages support the submit/cancel functionality
				Submit();
			});

			OnClick("BackButton", _ =>
			{
				Cancel();
			});

			_image = new Texture2D(graphicsDevice, 480, 480);
			var barcodeWriter = new BarcodeWriterPixelData()
			{
				Format = BarcodeFormat.QR_CODE,
				Options = new QrCodeEncodingOptions
				{
					Width = 480,
					Height = 480
				}
			};

			var pixels = barcodeWriter.Write(qr).Pixels;

			_image.SetData(pixels);
		}

		~DisplayQRPage()
		{
			_image?.Dispose();
		}

		public override void Draw(GameTime gameTime, IServiceProvider services)
		{
			var screen = services.GetService<VirtualScreen>();

			screen.DrawImage(_image, new Point(50,50), new Point(65, 65));

			base.Draw(gameTime, services);
		}

	}
}
