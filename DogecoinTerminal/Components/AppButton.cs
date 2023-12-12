using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Components
{
	internal class AppButton : Interactable
	{

		private  TerminalColor _backgroundColor;
		private TerminalColor _foregroundColor;
		private double _textScale;

		public AppButton(string text,
						 (int x, int y) start,
							(int x, int y) end,
							TerminalColor backgroundColor,
							TerminalColor foregroundColor,
							double textScale,
							Action<bool> onInteract)
			: base(start, end, onInteract)
		{
			Text = text;
			_backgroundColor = backgroundColor;
			_foregroundColor = foregroundColor;
			_textScale = textScale;
		}


		public override void Draw(VirtualScreen screen)
		{
			screen.DrawRectangle(_backgroundColor, Start, End);

			screen.DrawText(Text, _foregroundColor, _textScale, 
				(Start.x + ((End.x - Start.x)/2), Start.y + ((End.y - Start.y)/2)));
		}

		public string Text
		{
			get;set;
		}
	}
}
