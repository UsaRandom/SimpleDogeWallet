
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

namespace DogecoinTerminal.Common
{
	public class QRScannerPage : AppPage
	{
		public AppImage cameraImg;
		private GraphicsDevice _graphicsDevice;
		public Texture2D cameraTexture;
		private VideoCapture capture;
		private AppText updateText;


		public QRScannerPage(GraphicsDevice graphicsDevice)
			:base(true)
		{
			_graphicsDevice = graphicsDevice;


			cameraTexture = new Texture2D(graphicsDevice, 640, 480);

			updateText = new AppText("Scan QR", TerminalColor.White, 3, (50, 10));

			Interactables.Add(updateText);
		}

		private int updateNumb = 0;

		public override void Update()
		{

		}



		public override void Draw(VirtualScreen screen)
		{
			using (Mat frame = new Mat(640, 480, MatType.CV_32SC1))
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

					cameraTexture = new Texture2D(_graphicsDevice, 640, 480);


					cameraTexture.SetData(intArray);



					// create a barcode reader instance
					var reader = new BarcodeReader();
					// load a bitmap
					var result = reader.Decode(frame);
					
					// do something with the result
					if (result != null && result.BarcodeFormat == BarcodeFormat.QR_CODE
							&& result.Text.ToLower().StartsWith("dogecoin:"))
					{
						Router.Instance.Return(result.Text.Split(":")[1]);		
					}


					screen.DrawImage(cameraTexture, (50, 50), (50,60), (640, 480));

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
			Router.Instance.Back();
		}

		protected override void OnNav(dynamic value, bool backable)
		{

			capture = new VideoCapture(0, VideoCaptureAPIs.DSHOW);


			capture.Set(VideoCaptureProperties.FrameWidth, 640);
			capture.Set(VideoCaptureProperties.FrameHeight, 480);
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
