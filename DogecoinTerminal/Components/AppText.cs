using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Components
{
	internal class AppText : Interactable
	{

		private TerminalColor _color;

		private double _scale;

		public AppText(string text,
					   TerminalColor color,
					   double scale,
					   (int x, int y) start)
			: base(start, start, (isFirst) => {})
		{
			Text = text;
			_color = color;
			_scale = scale;
		}


		public override void Draw(VirtualScreen screen)
		{
			screen.DrawText(Text, _color, _scale, Start);
		}

		public string Text
		{
			get; set;
		}
	}
}
