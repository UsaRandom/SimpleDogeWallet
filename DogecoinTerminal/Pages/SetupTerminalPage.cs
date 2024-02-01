using DogecoinTerminal.Common;
using DogecoinTerminal.Common.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Pages
{
    [PageDef("Pages/Xml/SetupTerminalPage.xml")]
	internal class SetupTerminalPage : Page
	{
		private const string SETUP_BUTTON_NAME = "SetupButton";

		public SetupTerminalPage(IPageOptions options) : base(options)
		{
			OnClick(SETUP_BUTTON_NAME, async _ => {

			});
		}

	}
}
