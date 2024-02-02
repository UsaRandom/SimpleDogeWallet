using Cyotek.Drawing.BitmapFont;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DogecoinTerminal.Common.Controls
{
	public class XmlExporterControlVisitor : IControlVisitor
	{


		public XmlExporterControlVisitor()
		{
			DTCommonNamespace = "DogecoinTerminal.Common";

			PageElement = new XElement("Page",
				new XAttribute(XNamespace.Xmlns + "common", DTCommonNamespace));
		}


		public XElement PageElement { get; set; }

		private XNamespace DTCommonNamespace { get; set; }

		public void VisitTextInput(TextInputControl textInputControl)
		{
			VisitButton(textInputControl);
		}

		public void VisitButton(ButtonControl control)
		{
			XElement buttonControl = new XElement(DTCommonNamespace + "ButtonControl",
				new XAttribute("StartPosition", $"{control.StartPosition.X},{control.StartPosition.Y}"),
				new XAttribute("EndPosition", $"{control.EndPosition.X},{control.EndPosition.Y}"),
				new XAttribute("TextSize", control.TextSize),
				new XAttribute("Name", control.Name));

			if (control.BackgroundColor != null)
			{
				buttonControl.Add(new XAttribute("BackgroundColor", control.BackgroundColor.Name));
			}
			if (control.ForegroundColor != null)
			{
				buttonControl.Add(new XAttribute("ForegroundColor", control.ForegroundColor.Name));
			}

			if(!string.IsNullOrEmpty(control.StringDef))
			{
				buttonControl.Add(new XAttribute("StringDef", control.StringDef));
			}
			else
			{
				buttonControl.Add(new XAttribute("Text", control.Text));
			}

			PageElement.Add(buttonControl);

		}

		public void VisitImage(ImageControl control)
		{

			XElement imageControl = new XElement(DTCommonNamespace + "ImageControl",
				new XAttribute("Name", control.Name),
				new XAttribute("StartPosition", $"{control.StartPosition.X},{control.StartPosition.Y}"),
				new XAttribute("EndPosition", $"{control.EndPosition.X},{control.EndPosition.Y}"),
				new XAttribute("ImageSource", control.ImageSource));

			if(control.BackgroundColor != null)
			{
				imageControl.Add(new XAttribute("BackgroundColor", control.BackgroundColor.Name));
			}

			PageElement.Add(imageControl);
		}

		public void VisitSprite(SpriteControl control)
		{
			XElement spriteControl = new XElement(DTCommonNamespace + "SpriteControl",
				new XAttribute("SpriteSheetSource", control.SpriteSheetSource),
				new XAttribute("Rows", control.Rows),
				new XAttribute("Columns", control.Columns),
				new XAttribute("FrameCount", control.FrameCount),
				new XAttribute("FrameTimeMs", control.FrameTimeMs),
				new XAttribute("StartPosition", $"{control.StartPosition.X},{control.StartPosition.Y}"),
				new XAttribute("EndPosition", $"{control.EndPosition.X},{control.EndPosition.Y}"),
				new XAttribute("Name", control.Name));
			PageElement.Add(spriteControl);
		}

		public void VisitText(TextControl control)
		{

			XElement textControl = new XElement(DTCommonNamespace + "TextControl",
				new XAttribute("Name", control.Name),
				new XAttribute("Color", control.Color.Name),
				new XAttribute("Position", $"{control.Position.X},{control.Position.Y}"),
				new XAttribute("TextSize", control.TextSize));


			if (!string.IsNullOrEmpty(control.StringDef))
			{
				textControl.Add(new XAttribute("StringDef", control.StringDef));
			}
			else
			{
				textControl.Add(new XAttribute("Text", control.Text));
			}

			PageElement.Add(textControl);
		}
	
	}
}
