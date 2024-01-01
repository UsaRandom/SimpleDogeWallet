using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Common.old
{
	public class Images
	{
		public static Texture2D DogeImage;
		public static Texture2D ArrowImage;

		public static void Load(GraphicsDevice device)
		{
			var fileStream = new FileStream("Content/dogedrawn.png", FileMode.Open);
			DogeImage = Texture2D.FromStream(device, fileStream);
			fileStream.Dispose();

			fileStream = new FileStream("Content/arrow.png", FileMode.Open);
			ArrowImage = Texture2D.FromStream(device, fileStream);
			fileStream.Dispose();
		}

		public static (int width, int height) DogeImageDim = (2477, 2477);
		public static (int width, int height) ArrowImageData = (231, 148);
	}
}
