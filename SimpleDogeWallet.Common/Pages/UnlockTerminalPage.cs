using SimpleDogeWallet.Common;
using SimpleDogeWallet.Common.Pages;
using Microsoft.Xna.Framework;
using System;

namespace SimpleDogeWallet.Pages
{
    [PageDef("Pages/Xml/UnlockTerminalPage.xml")]
	public class UnlockTerminalPage : Page
	{
		private const string UNLOCK_BUTTON_NAME = "UnlockButton";

		public UnlockTerminalPage(IPageOptions options, Navigation navigation, Strings strings, Game game, IServiceProvider services) : base(options)
		{
			OnClick(UNLOCK_BUTTON_NAME, async _ => {

				await navigation.PushAsync<LoadingPage>();

				var numPadResponse = await navigation.PromptAsync<NumPadPage>(("title", strings["terminal-enteroppin-title"]));

				if (numPadResponse.Response == PromptResponse.YesConfirm &&
					SimpleDogeWallet.TryOpen(numPadResponse.Value.ToString()))
				{
					await navigation.TryInsertBeforeAsync<WalletPage, LoadingPage>(("wallet", SimpleDogeWallet.Instance)); 
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
