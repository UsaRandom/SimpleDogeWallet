using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Components
{
	internal class AppButton : Interactable
	{

		public  TerminalColor BackgroundColor;
		public TerminalColor ForegroundColor;
		private float _textScale;

		public AppButton(string text,
						 (int x, int y) start,
							(int x, int y) end,
							TerminalColor backgroundColor,
							TerminalColor foregroundColor,
							float textScale,
							Action<bool, Interactable> onInteract)
			: base(start, end, onInteract)
		{
			Text = text;
			BackgroundColor = backgroundColor;
			ForegroundColor = foregroundColor;
			_textScale = textScale;
		}



		public override void Draw(VirtualScreen screen)
		{
			screen.DrawRectangle(BackgroundColor, Start, End);

			screen.DrawText(Text, ForegroundColor, _textScale, 
				(Start.x + ((End.x - Start.x)/2), Start.y + ((End.y - Start.y)/2)));
		}

		public string Text
		{
			get;set;
		}
	}
}
