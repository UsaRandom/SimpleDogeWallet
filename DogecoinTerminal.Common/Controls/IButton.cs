using Microsoft.Xna.Framework;

namespace DogecoinTerminal.Common
{
    interface IButton : IPageControl
	{
		string StringDef { get; }
		string Text { get; set; }
		Point StartPosition { get; set; }
		Point EndPosition { get; set; }
		TerminalColor BackgroundColor { get; set; }
		TerminalColor ForegroundColor { get; set; }
		int TextSize { get; set; }
	}

}
