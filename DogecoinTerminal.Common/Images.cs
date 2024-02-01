using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Common
{
	public class Images
	{
		private Dictionary<string, Texture2D> _imageDictionary = new();
		private GraphicsDevice _graphicsDevice;

		public Images(GraphicsDevice graphicsDevice)
		{
			_graphicsDevice = graphicsDevice;
		}


		public Texture2D GetImage(string path)
		{
			if (!_imageDictionary.ContainsKey(path))
			{
				_imageDictionary[path] = LoadImage(path);

			}

			return _imageDictionary[path];
		}

		private Texture2D LoadImage(string path)
		{
			using (var fileStream = new FileStream(path, FileMode.Open))
			{
				return Texture2D.FromStream(_graphicsDevice, fileStream);
			}
		}
	}
}
