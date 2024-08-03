using Microsoft.Xna.Framework.Graphics;
using XNA = Microsoft.Xna.Framework;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing.OpenCV;
using Microsoft.Xna.Framework;



namespace SimpleDogeWallet.Common.Pages
{
	[PageDef("Pages/Xml/QRScannerPage.xml")]
	public class QRScannerPage : PromptPage
	{
		private GraphicsDevice _graphicsDevice;
		public Texture2D _cameraTexture;
		private VideoCapture _capture;

		private const int VIDEO_CAPTURE_HEIGHT = 480;
		private const int VIDEO_CAPTURE_WIDTH = 640;



		private Strings _strings;

		public QRScannerPage(IPageOptions options, Navigation navigation, GraphicsDevice graphicsDevice, Strings strings) : base(options)
		{
			_strings = strings;

			if (Environment.OSVersion.Platform == PlatformID.Win32NT)
			{
				_capture = new VideoCapture(0, VideoCaptureAPIs.DSHOW);
			}
			else
			{
				//TODO: this can take a long time if you don't specify the video capture api.
				//
				//we also are relying on the first camera, there might be more.
				_capture = new VideoCapture(0, VideoCaptureAPIs.ANY);
			}


			_capture.Set(VideoCaptureProperties.FrameWidth, VIDEO_CAPTURE_WIDTH);
			_capture.Set(VideoCaptureProperties.FrameHeight, VIDEO_CAPTURE_HEIGHT);

			_graphicsDevice = graphicsDevice;

			_cameraTexture = new Texture2D(_graphicsDevice, VIDEO_CAPTURE_WIDTH, VIDEO_CAPTURE_HEIGHT);



			OnClick("BackButton", _ =>
			{
				Cancel();
			});

		}

		public override void Cleanup()
		{
			_capture.Dispose();
			base.Cleanup();
		}






		public override void Draw(GameTime gameTime, IServiceProvider services)
		{
			var screen = services.GetService<VirtualScreen>();


			using (Mat frame = new Mat(VIDEO_CAPTURE_WIDTH, VIDEO_CAPTURE_HEIGHT, MatType.CV_32SC1))
			{
				if (!_capture.IsDisposed && _capture.Read(frame))
				{
					try
					{
						var next = frame.CvtColor(ColorConversionCodes.BGR2RGBA);

						next.GetArray<Vec4b>(out Vec4b[] vec4bArray);

						int[] intArray = new int[vec4bArray.Length];
						for (int i = 0; i < vec4bArray.Length; i++)
						{
							intArray[i] = (vec4bArray[i][3] << 24) | (vec4bArray[i][2] << 16) | (vec4bArray[i][1] << 8) | vec4bArray[i][0];
						}
						_cameraTexture.Dispose();

						_cameraTexture = new Texture2D(_graphicsDevice, VIDEO_CAPTURE_WIDTH, VIDEO_CAPTURE_HEIGHT);


						_cameraTexture.SetData(intArray);

						var reader = new BarcodeReader();
						var result = reader.Decode(frame);

						screen.DrawImage(_cameraTexture, new XNA.Point(50, 57), new XNA.Point(78, 60));

						next.Dispose();


						if (result != default && !string.IsNullOrEmpty(result.Text))
						{
							string address = result.Text;
							if(result.Text.StartsWith("dogecoin:"))
							{
								address = result.Text.Split(':')[1];
							}

							if(!Crypto.VerifyP2PKHAddress(address))
							{
								GetControl<TextControl>("HintText").Text = $"{_strings.GetString("terminal-qrscanner-hint")}\n{result.Text}";
							}
							else
							{
								_capture.Dispose();
								Submit(address);
							}

						}

					}
					catch (Exception ex) { }

				}
			}

			base.Draw(gameTime, services);
		}


		private Texture2D ConvertMatToTexture2D(Mat mat)
		{
			Texture2D texture = null;

			using (MemoryStream stream = new MemoryStream(mat.ToBytes()))
			{
				texture = Texture2D.FromStream(_graphicsDevice, stream);
			}

			return texture;
		}


		~QRScannerPage()
		{
			_cameraTexture?.Dispose();
		}

	}
}
