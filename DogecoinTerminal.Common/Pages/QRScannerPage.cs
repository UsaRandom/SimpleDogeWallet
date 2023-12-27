
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using ZXing.OpenCV;
using static System.Net.Mime.MediaTypeNames;
using DogecoinTerminal.Common.Components;
using DogecoinTerminal.Common;
using ZXing.Common;
using ZXing.QrCode;

namespace DogecoinTerminal.Common
{
	public class QRScannerPage : AppPage
	{
		public AppImage cameraImg;
		private GraphicsDevice _graphicsDevice;
		public Texture2D cameraTexture;
		private VideoCapture capture;
		private AppText titleText;

		private const int VIDEO_CAPTURE_HEIGHT = 480;
		private const int VIDEO_CAPTURE_WIDTH = 640;


		public QRScannerPage(Game game)
			:base(game, true)
		{
			_graphicsDevice = Game.GraphicsDevice;


			cameraTexture = new Texture2D(Game.GraphicsDevice, VIDEO_CAPTURE_WIDTH, VIDEO_CAPTURE_HEIGHT);

			titleText = new AppText("Scan QR", TerminalColor.White, 3, (50, 10));

			Interactables.Add(titleText);
		}

		private int updateNumb = 0;

		public override void Update()
		{

		}



		public override void Draw(VirtualScreen screen)
		{
			using (Mat frame = new Mat(VIDEO_CAPTURE_WIDTH, VIDEO_CAPTURE_HEIGHT, MatType.CV_32SC1))
			{
				if (capture.Read(frame))
				{
					var next = frame.CvtColor(ColorConversionCodes.BGR2RGBA);

					next.GetArray<Vec4b>(out Vec4b[] vec4bArray);

					int[] intArray = new int[vec4bArray.Length];
					for (int i = 0; i < vec4bArray.Length; i++)
					{
						intArray[i] = (vec4bArray[i][3] << 24) | (vec4bArray[i][2] << 16) | (vec4bArray[i][1] << 8) | vec4bArray[i][0];
					}
					cameraTexture.Dispose();

					cameraTexture = new Texture2D(_graphicsDevice, VIDEO_CAPTURE_WIDTH, VIDEO_CAPTURE_HEIGHT);


					cameraTexture.SetData(intArray);

					//byte[] byteArray = new byte[intArray.Length * sizeof(int)];
					//Buffer.BlockCopy(intArray, 0, byteArray, 0, byteArray.Length);

					//RGBLuminanceSource source = new RGBLuminanceSource(byteArray, VIDEO_CAPTURE_WIDTH, VIDEO_CAPTURE_HEIGHT);
					//BinaryBitmap bitmap = new BinaryBitmap(new HybridBinarizer(source));
					//Dictionary<DecodeHintType, object> hints = new Dictionary<DecodeHintType, object>();
					//hints.Add(DecodeHintType.TRY_HARDER, true);

					//Result result = new QRCodeReader().decode(bitmap, hints);

					// create a barcode reader instance
					var reader = new BarcodeReader();
					// load a bitmap
						var result = reader.Decode(frame);

					// do something with the result
					if (result != null)
					{
						Game.Services.GetService<Router>().Return(result.Text);		
					}


					screen.DrawImage(cameraTexture, (50, 50), (50,60), (VIDEO_CAPTURE_WIDTH, VIDEO_CAPTURE_HEIGHT));

					next.Dispose();
				}
			}
		}




		public override void Cleanup()
		{

			capture.Dispose();
		}

		public override void OnBack()
		{
			Game.Services.GetService<Router>().Back();
		}

		protected override void OnNav(dynamic value, bool backable)
		{
			if(!string.IsNullOrEmpty(value as string))
			{
				titleText.Text = value as string;
			}
			else
			{
				titleText.Text = "Scan QR";
			}


			if(Environment.OSVersion.Platform == PlatformID.Win32NT)
			{
				capture = new VideoCapture(0, VideoCaptureAPIs.DSHOW);
			}
			else
			{
				//TODO: this can take a long time if you don't specify the video capture api.
				//
				//we also are relying on the first camera, there might be more.
				capture = new VideoCapture(0, VideoCaptureAPIs.ANY);
			}


			capture.Set(VideoCaptureProperties.FrameWidth, VIDEO_CAPTURE_WIDTH);
			capture.Set(VideoCaptureProperties.FrameHeight, VIDEO_CAPTURE_HEIGHT);
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
	}
}
