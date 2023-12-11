using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Components
{
	internal class AppButton : Interactable
	{


		public AppButton(string text,
						 (int x, int y) start,
							(int x, int y) end,
							Action<bool> onInteract)
			: base(start, end, onInteract)
		{
			Text = text;
		}


		public override void Draw(VirtualScreen screen)
		{

			screen.DrawRectangle(TerminalColor.Red, Start, End);

			screen.DrawText(Text, TerminalColor.White, Start);
		}

		public string Text
		{
			get;set;
		}
	}
}
