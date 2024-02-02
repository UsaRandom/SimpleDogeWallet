using DogecoinTerminal.Common.Pages;
using DogecoinTerminal.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Pages
{

	[PageDef("Pages/Xml/SetupWalletPage.xml")]
	internal class SetupWalletPage : Page
	{

		public SetupWalletPage(IPageOptions options, Navigation navigation, Strings strings, Game game) : base(options)
		{
		
		}


		public override void Draw(GameTime gameTime, IServiceProvider services)
		{
			var screen = services.GetService<VirtualScreen>();

			screen.DrawRectangle(TerminalColor.LightGrey, new Point(49, 28), new Point(50, 62));

			base.Draw(gameTime, services);
		}
	}
}
