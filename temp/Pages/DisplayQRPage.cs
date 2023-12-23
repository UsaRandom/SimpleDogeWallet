using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace DogecoinTerminal.Common
{
	public class DisplayQRPage : AppPage
	{
		private GraphicsDevice _graphicsDevice;

		private Texture2D _image;

		public DisplayQRPage(GraphicsDevice graphicsDevice)
			: base(true)
		{
			_graphicsDevice = graphicsDevice;
		}

		public override void OnBack()
		{

		}

		public override void Draw(VirtualScreen screen)
		{
			screen.DrawImage(_image, (50, 40), (40, 40), (480, 480));
		}


		protected override void OnNav(dynamic value, bool backable)
		{
			_image = new Texture2D(_graphicsDevice, 480, 480);
			var barcodeWriter = new BarcodeWriterPixelData()
			{
				Format = BarcodeFormat.QR_CODE,
				Options = new QrCodeEncodingOptions
				{
					Width = 480,
					Height = 480
				}
			};

			var pixels = barcodeWriter.Write(value).Pixels;

			_image.SetData(pixels);
		}
	}
}
