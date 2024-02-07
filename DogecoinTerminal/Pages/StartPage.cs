using DogecoinTerminal.Common.Pages;
using DogecoinTerminal.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DogecoinTerminal.Pages
{
	internal class StartPage : Page
	{
		public StartPage(IPageOptions options, Navigation navigation, DogecoinTerminalGame game) : base(options)
		{
			var hasWallet = File.Exists(SimpleDogeWallet.ADDRESS_FILE);//detect

			if (!hasWallet)
			{
				//start on the language selection screen:
				Task.Run(async () =>
				{
					await navigation.PromptAsync<LanguageSelectionPage>();



					await navigation.PushAsync<SetupWalletPage>();
				});
			}
			else
			{
				Task.Run(async () =>
				{
					game.Services.AddService(Lib.Dogecoin.LibDogecoinContext.CreateContext());

					await navigation.PushAsync<UnlockTerminalPage>();
				});
			}
		}

	}
}
