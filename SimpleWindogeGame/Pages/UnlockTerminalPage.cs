using DogecoinTerminal.Common;
using DogecoinTerminal.Common.Pages;
using Microsoft.Xna.Framework;
using System;

namespace DogecoinTerminal.Pages
{
    [PageDef("Pages/Xml/UnlockTerminalPage.xml")]
	internal class UnlockTerminalPage : Page
	{
		private const string UNLOCK_BUTTON_NAME = "UnlockButton";

		public UnlockTerminalPage(IPageOptions options, Navigation navigation, Strings strings, Game game, IServiceProvider services) : base(options)
		{
			OnClick(UNLOCK_BUTTON_NAME, async _ => {

				await navigation.PushAsync<LoadingPage>();

				var numPadResponse = await navigation.PromptAsync<NumPadPage>(("title", strings["terminal-enteroppin-title"]));

				if (numPadResponse.Response == PromptResponse.YesConfirm &&
					SimpleDogeWallet.TryOpen(numPadResponse.Value.ToString(),
												services,
												out SimpleDogeWallet simpleWallet))
				{
					await navigation.TryInsertBeforeAsync<WalletPage, LoadingPage>(("wallet", simpleWallet)); 
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
