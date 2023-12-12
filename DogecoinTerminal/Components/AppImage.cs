using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DogecoinTerminal.Components
{
	internal class AppImage : Interactable
	{
		private Texture2D _image;

		//desired dim
		private (int width, int height) _imgDim;

		//original dim
		private (int width, int height) _imgOgDim;

		public AppImage(Texture2D image,
						 (int x, int y) start,
						 (int width, int height) imgDim,
						 (int width, int height) imgOgDim)
			: base(start, start, (a,b) => { })
		{
			_image = image;
			_imgDim = imgDim;
			_imgOgDim = imgOgDim;
		}


		public override void Draw(VirtualScreen screen)
		{
			screen.DrawImage(_image, Start, _imgDim, _imgOgDim);
		}

		public string Text
		{
			get; set;
		}
	}
}
