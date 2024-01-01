using DogecoinTerminal.Common.Controls;
using Microsoft.Xna.Framework;
using System;
using System.Xml.Linq;

namespace DogecoinTerminal.Common
{
	public class Button : PageControl, IButton
	{

		internal Button(XElement element)
		{
			StartPosition = GetPoint(element.Attribute(nameof(StartPosition)));
			EndPosition = GetPoint(element.Attribute(nameof(EndPosition)));
			BackgroundColor = GetTerminalColor(element.Attribute(nameof(BackgroundColor)));
			ForegroundColor = GetTerminalColor(element.Attribute(nameof(ForegroundColor)));
			TextSize = int.Parse(element.Attribute(nameof(TextSize)).Value);
			Name = element.Attribute(nameof(Name)).Value;
			StringDef = element.Attribute(nameof(StringDef)).Value;
			Text = StringDef;
		}

		public string Text { get; set; }
		public Point StartPosition { get; set; }
		public Point EndPosition { get; set; }
		public TerminalColor BackgroundColor { get; set; }
		public TerminalColor ForegroundColor { get; set; }
		public int TextSize { get; set; }

		public string Name { get; private set; }

		public string StringDef { get; set; }

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

			screen.DrawRectangle(BackgroundColor, StartPosition, EndPosition);

			screen.DrawText(Text, ForegroundColor, TextSize,
				new Point(StartPosition.X + ((EndPosition.X - StartPosition.X) / 2),
						  StartPosition.Y + ((EndPosition.Y - StartPosition.Y) / 2)));
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
	}
}
