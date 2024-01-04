using DogecoinTerminal.Common;
using DogecoinTerminal.Common.Pages;
using Microsoft.Xna.Framework;

namespace DogecoinTerminal.Pages
{
    [PageDef("Pages/Xml/UnlockTerminalPage.xml")]
	internal class UnlockTerminalPage : Page
	{
		private const string UNLOCK_BUTTON_NAME = "UnlockButton";

		public UnlockTerminalPage(IPageOptions options, Navigation navigation, ITerminalService terminalService, Strings strings, Game game) : base(options)
		{
			OnClick(UNLOCK_BUTTON_NAME, async _ => {

				await navigation.PushAsync<BlankPage>();

				var numPadResponse = await navigation.PromptAsync<NumPadPage>(("title", strings["terminal-enteroppin-title"]));

				if (numPadResponse.Response == PromptResponse.YesConfirm
					&& terminalService.Unlock(numPadResponse.Value.ToString()))
				{
					await navigation.TryInsertBeforeAsync<WalletListPage, BlankPage>();
				}

				navigation.Pop();
			});

			OnClick("ExitButton", async _ =>
			{
				game.Exit();
			});
		}

	}
}
