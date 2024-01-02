using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DogecoinTerminal.Common
{
	public class ImageControl : PageControl
	{

		public ImageControl(XElement element)
			: base(element)
		{
			StartPosition = GetPoint(element.Attribute(nameof(StartPosition)));
			EndPosition = GetPoint(element.Attribute(nameof(EndPosition)));
			SourceImageDim = GetPoint(element.Attribute(nameof(SourceImageDim)));
			ImageSource = element.Attribute(nameof(ImageSource)).Value;
		}


		public Point StartPosition { get; set; }
		public Point EndPosition { get; set; }
		public Point SourceImageDim { get; set; }

		public string ImageSource { get; set; }

		public override bool ContainsPoint(Point point)
		{
			if (point.X >= StartPosition.X && point.X <= EndPosition.X &&
				point.Y >= StartPosition.Y && point.Y <= EndPosition.Y)
			{
				return true;
			}
			return false;
		}

		public override void Draw(GameTime time, IServiceProvider services)
		{
			var screen = services.GetService<VirtualScreen>();


		}

		public override void Update(GameTime time, IServiceProvider services)
		{

		}
	}
}
