using SimpleDogeWallet.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace SimpleDogeWallet.Common
{
	public class ContactControl : PageControl
	{

		public ContactControl(XElement element)
			: base(element)
		{
			StartPosition = GetPoint(element.Attribute(nameof(StartPosition)));
			EndPosition = GetPoint(element.Attribute(nameof(EndPosition)));
			BackgroundColor = GetTerminalColor(element.Attribute(nameof(BackgroundColor)));
			ForegroundColor = GetTerminalColor(element.Attribute(nameof(ForegroundColor)));
			IsSelected = false;
		}


		public Contact Contact { get; set; }

		public bool IsSelected { get; set; }
		public Point StartPosition { get; set; }
		public Point EndPosition { get; set; }
		public TerminalColor BackgroundColor { get; set; }
		public TerminalColor ForegroundColor { get; set; }
		public int TextSize { get; set; }


		public override bool ContainsPoint(Point point)
		{
			if (!Enabled) return false;

			if (point.X >= StartPosition.X && point.X < EndPosition.X &&
				point.Y >= StartPosition.Y && point.Y < EndPosition.Y)
			{
				return true;
			}
			return false;
		}

		public override void Draw(GameTime time, IServiceProvider services)
		{
			if (!Enabled) return;

			var screen = services.GetService<VirtualScreen>();

			if (IsSelected)
			{
				screen.DrawRectangle(TerminalColor.Blue, StartPosition, EndPosition);
			}
			else
			{
				screen.DrawRectangle(BackgroundColor, StartPosition, EndPosition);
			}

			if(Contact != default)
			{
				screen.DrawText(Contact.Name, ForegroundColor, 3,
					new Point(StartPosition.X + ((EndPosition.X - StartPosition.X) / 2),
							  StartPosition.Y + ((EndPosition.Y - StartPosition.Y) / 3)));

				screen.DrawText(Contact.ShortAddress, ForegroundColor, 4,
					new Point(StartPosition.X + ((EndPosition.X - StartPosition.X) / 2),
							  StartPosition.Y + (((EndPosition.Y - StartPosition.Y) / 3)*2)));

			}


		}

		public override void Update(GameTime time, IServiceProvider services)
		{
			if (!Enabled) return;

			//var strings = services.GetService<Strings>();

			//if (!string.IsNullOrEmpty(StringDef)
			//	&& Text != strings[StringDef])
			//{
			//	Text = strings[StringDef];
			//}
		}

		public override void AcceptVisitor(IControlVisitor visitor)
		{
			visitor.VisitContact(this);
			//visitor.visi(this);
		}
	}
}
