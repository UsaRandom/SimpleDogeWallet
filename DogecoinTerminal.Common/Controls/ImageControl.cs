using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DogecoinTerminal.Common
{
	public class ImageControl : PageControl
	{
		private Point _imgDim;
		private Point _center;

		public ImageControl(XElement element)
			: base(element)
		{
			StartPosition = GetPoint(element.Attribute(nameof(StartPosition)));
			EndPosition = GetPoint(element.Attribute(nameof(EndPosition)));
			ImageSource = element.Attribute(nameof(ImageSource)).Value;

			var backgroundAttr = element.Attribute(nameof(BackgroundColor));

			if(backgroundAttr != null )
			{
				BackgroundColor = GetTerminalColor(backgroundAttr);
			}
			

			_imgDim = new Point(EndPosition.X - StartPosition.X, EndPosition.Y - StartPosition.Y);
			_center = new Point(StartPosition.X + (_imgDim.X / 2), StartPosition.Y + (_imgDim.Y / 2));
		}


		public Point StartPosition { get; set; }
		public Point EndPosition { get; set; }
		public Point SourceImageDim { get; set; }

		public string ImageSource { get; set; }

		public TerminalColor BackgroundColor { get; set; }

		public override bool ContainsPoint(Point point)
		{
			if (point.X >= StartPosition.X && point.X < EndPosition.X &&
				point.Y >= StartPosition.Y && point.Y < EndPosition.Y)
			{
				return true;
			}
			return false;
		}

		public override void Draw(GameTime time, IServiceProvider services)
		{
			var screen = services.GetService<VirtualScreen>();
			var images = services.GetService<Images>();

			var imageTexture = images.GetImage(ImageSource);


			if(BackgroundColor != null)
			{
				screen.DrawRectangle(BackgroundColor, StartPosition, EndPosition);
			}

			screen.DrawImage(imageTexture, _center, _imgDim);

		}

		public override void Update(GameTime time, IServiceProvider services)
		{
			
			
		}
	}
}
