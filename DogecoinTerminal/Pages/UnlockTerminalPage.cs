using DogecoinTerminal.Common;
using DogecoinTerminal.Common.Pages;

namespace DogecoinTerminal.Pages
{
	[PageDef("Pages/Xml/UnlockTerminal.xml")]
	internal class UnlockTerminalPage : Page
	{
		private const string UNLOCK_BUTTON_NAME = "UnlockButton";

		public UnlockTerminalPage(IPageOptions options, Navigation navigation, ITerminalService terminalService, Strings strings) : base(options)
		{
			OnClick(UNLOCK_BUTTON_NAME, async _ => {

				var numPadResponse = await navigation.PromptAsync<NumPadPage>(("title", strings["terminal-enteroppin-title"]));

				if (numPadResponse.Response == PromptResponse.YesConfirm
					&& terminalService.Unlock( numPadResponse.Value.ToString()))
				{
					//Ok, so this is after we've entered the operator pin to unlock the terminal, we now request to navigate to a WalletList page
					//await navigation.PushAsync<WalletListPage>();

					await navigation.PushAsync<WalletListPage>();


				}

			});
		}

	}
}
