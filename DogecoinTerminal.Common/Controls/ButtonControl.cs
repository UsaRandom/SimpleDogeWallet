
using Microsoft.Xna.Framework;
using System;
using System.Xml.Linq;

namespace DogecoinTerminal.Common
{
    public class ButtonControl : PageControl
	{

		public ButtonControl(XElement element)
			: base(element)
		{
			StartPosition = GetPoint(element.Attribute(nameof(StartPosition)));
			EndPosition = GetPoint(element.Attribute(nameof(EndPosition)));
			BackgroundColor = GetTerminalColor(element.Attribute(nameof(BackgroundColor)));
			ForegroundColor = GetTerminalColor(element.Attribute(nameof(ForegroundColor)));
			TextSize = int.Parse(element.Attribute(nameof(TextSize)).Value);
			StringDef = element.Attribute(nameof(StringDef))?.Value;
			Text = StringDef ?? element.Attribute(nameof(Text))?.Value;

			IsSelected = false;
		}
	
		public bool IsSelected { get; set; }

		public string Text { get; set; }
		public Point StartPosition { get; set; }
		public Point EndPosition { get; set; }
		public TerminalColor BackgroundColor { get; set; }
		public TerminalColor ForegroundColor { get; set; }
		public int TextSize { get; set; }


		public string StringDef { get; set; }

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

			screen.DrawRectangle(BackgroundColor, StartPosition, EndPosition);

			screen.DrawText(Text, ForegroundColor, TextSize,
				new Point(StartPosition.X + ((EndPosition.X - StartPosition.X) / 2),
						  StartPosition.Y + ((EndPosition.Y - StartPosition.Y) / 2)));

			if(IsSelected)
			{
				screen.DrawRectangleBorder(TerminalColor.Blue, StartPosition, EndPosition);
			}
		}

		public override void Update(GameTime time, IServiceProvider services)
		{
			var strings = services.GetService<Strings>();

			if(!string.IsNullOrEmpty(StringDef)
				&& Text != strings[StringDef])
			{
				Text = strings[StringDef];
			}
		}

		public override void AcceptVisitor(IControlVisitor visitor)
		{
			visitor.VisitButton(this);
		}
	}
}
