
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SimpleDogeWallet.Common
{

	public enum TextAnchor
	{
		Center,
		TopLeft,
		TopRight
	}

    public class TextControl : PageControl
	{
		public TextControl(XElement element)
			: base(element)
		{
			Position = GetPoint(element.Attribute(nameof(Position)));
			Color = GetTerminalColor(element.Attribute(nameof(Color)));
			TextSize = float.Parse(element.Attribute(nameof(TextSize)).Value);
			StringDef = element.Attribute(nameof(StringDef))?.Value;
			Text = StringDef ?? element.Attribute(nameof(Text))?.Value ?? string.Empty;

			Anchor = TextAnchor.Center;

			var anchorAttr = element.Attribute(nameof(Anchor));

			if(anchorAttr != null )
			{
				Anchor = (TextAnchor)Enum.Parse(typeof(TextAnchor), anchorAttr.Value);
			}
		}

		public TextAnchor Anchor { get; set; }

		public string Text { get; set; }
		public Point Position { get; set; }
		public TerminalColor Color { get; set; }
		public float TextSize { get; set; }


		public string StringDef { get; set; }

		public override bool ContainsPoint(Point point)
		{
			//TODO: is this needed? maybe for word import?
			return false;
		}

		public override void Draw(GameTime time, IServiceProvider services)
		{
			if (!Enabled) return;

			var screen = services.GetService<VirtualScreen>();


			screen.DrawText(Text, Color, TextSize, Position, Anchor);
		}

		public override void Update(GameTime time, IServiceProvider services)
		{
			if (!Enabled) return;

			var strings = services.GetService<Strings>();

			if (!string.IsNullOrEmpty(StringDef)
				&& Text != strings[StringDef])
			{
				Text = strings[StringDef];
			}
		}


		public override void AcceptVisitor(IControlVisitor visitor)
		{
			visitor.VisitText(this);
		}
	}
}
