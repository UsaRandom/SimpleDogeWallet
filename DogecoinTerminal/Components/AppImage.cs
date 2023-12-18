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
		public Texture2D Image;

		//desired dim
		private (int width, int height) _imgDim;

		//original dim
		private (int width, int height) _imgOgDim;

		private (int x, int y) _center;

		public AppImage(Texture2D image,
						 (int x, int y) start,
						 (int x, int y) end,
						 (int width, int height) imgOgDim,
						 Action<bool, Interactable> onInteract = null)
			: base(start, end, onInteract)
		{
			Image = image;
			_imgDim = (end.x-start.x, end.y-start.y);
			_imgOgDim = imgOgDim;
			_center = (start.x + (_imgDim.width / 2), start.y + (_imgDim.height / 2));
		}


		public override void Draw(VirtualScreen screen)
		{
			screen.DrawImage(Image, _center, _imgDim, _imgOgDim);
		}

		public string Text
		{
			get; set;
		}
	}
}
