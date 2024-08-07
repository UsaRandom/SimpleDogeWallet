﻿
using Microsoft.Xna.Framework;
using System;
using System.Xml.Linq;

namespace SimpleDogeWallet.Common
{
    public abstract class PageControl : IPageControl
	{

		public PageControl(XElement element)
		{
			Name = element.Attribute(nameof(Name)).Value;
			var enabledAttr = element.Attribute(nameof(Enabled));

			if(enabledAttr != null && bool.TryParse(enabledAttr.Value, out bool isEnabled))
			{
				Enabled = isEnabled;
			}
			else
			{
				Enabled = true;
			}
		}

		protected Point GetPoint(XAttribute attribute)
		{
			string[] values = attribute.Value.Split(',');
			return new Point(int.Parse(values[0]), int.Parse(values[1]));
		}

		protected TerminalColor GetTerminalColor(XAttribute attribute)
		{
			switch(attribute.Value)
			{
				case "White":
					return TerminalColor.White;
				case "Blue":
					return TerminalColor.Blue;
				case "Green":
					return TerminalColor.Green;
				case "Grey":
					return TerminalColor.Grey;
				case "Red":
					return TerminalColor.Red;
				case "DarkGrey":
					return TerminalColor.DarkGrey;
				case "LightGrey":
					return TerminalColor.LightGrey;
				default:
					throw new Exception($"{attribute.Name} has invalid color: {attribute.Value}");
			}
		}

		public string Name { get; private set; }

		public bool Enabled { get; set; }

		public abstract bool ContainsPoint(Point point);

		public abstract void Draw(GameTime time, IServiceProvider services);

		public abstract void Update(GameTime time, IServiceProvider services);


		public abstract void AcceptVisitor(IControlVisitor visitor);

		public virtual void OnControlShown(IServiceProvider services)
		{

		}

		public virtual void OnControlHidden(IServiceProvider services)
		{

		}
	}
}
