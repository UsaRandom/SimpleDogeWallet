using Microsoft.Xna.Framework;
using System;
using System.Xml.Linq;

namespace DogecoinTerminal.Common
{
	public abstract class PageControl : IPageControl
	{

		public PageControl(XElement element)
		{
			Name = element.Attribute(nameof(Name)).Value;
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

		public abstract bool ContainsPoint(Point point);

		public abstract void Draw(GameTime time, IServiceProvider services);

		public abstract void Update(GameTime time, IServiceProvider services);
	}
}
