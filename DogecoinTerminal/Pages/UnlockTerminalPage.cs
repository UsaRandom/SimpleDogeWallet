using DogecoinTerminal.Common;

namespace DogecoinTerminal.Pages
{
	[PageDef("Pages/Xml/UnlockTerminal.xml")]
	internal class UnlockTerminalPage : Page
	{
		private const string UNLOCK_BUTTON_NAME = "UnlockButton";

		public UnlockTerminalPage(IPageOptions options, Navigation navigation, ITerminalService terminalService, Strings strings) : base(options)
		{
			OnClick(UNLOCK_BUTTON_NAME, async _ => {

				var numPadResponse = await navigation.PromptAsync<NumberPadPage>(("title", strings["terminal-enteroppin-title"]));

				if (terminalService.ConfirmOperatorPin(numPadResponse.Value.ToString()) &&
				   numPadResponse.Response == PromptResponse.YesConfirm)
				{
					await navigation.PushAsync<SettingsPage>();
				}

			});
		}

	}
}
