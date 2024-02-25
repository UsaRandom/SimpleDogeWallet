using DogecoinTerminal.Common;
using DogecoinTerminal.Common.Pages;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Pages
{
	[PageDef("Pages/Xml/DisclaimerPage.xml")]
	internal class DisclaimerPage : PromptPage
	{
		public DisclaimerPage(IPageOptions options) : base(options)
		{
			OnClick("OkButton", _ =>
			{
				Submit();
			});
		}

		public override void Draw(GameTime gameTime, IServiceProvider services)
		{
			var screen = services.GetService<VirtualScreen>();


			screen.DrawRectangle(TerminalColor.Grey, new Point(5, 36), new Point(95, 64));

			base.Draw(gameTime, services);
		}

	}
}
