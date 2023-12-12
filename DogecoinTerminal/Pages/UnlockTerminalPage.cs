using DogecoinTerminal.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DogecoinTerminal.Pages.PinCodePage;

namespace DogecoinTerminal.Pages
{
	internal class UnlockTerminalPage : AppPage
	{
		private const string UNLOCK_PIN = "420.69";


		public UnlockTerminalPage()
		{
			Interactables.Add(
				new AppImage(Images.DogeImage,
					(50, 25), (15, 15), Images.DogeImageDim)
				);

			Interactables.Add(
				new AppText("Đogecoin Terminal", TerminalColor.White, 5, (50, 50))
				);

			Interactables.Add(
				new AppButton("Unlock",
						(40, 60), (60, 70),
						TerminalColor.Green,
						TerminalColor.White,
						5,
						(isFirst, self) =>
						{
							Router.Instance.Route("pin",
								new PinCodePageSettings("Enter Pin", false), true,
								(dynamic enteredPin) =>
								{
									if(enteredPin == UNLOCK_PIN)
									{
										Router.Instance.Route("wallets", null, false);
									}
								});
						}));
		}


		public override void OnBack()
		{

		}

		protected override void OnNav(dynamic value, bool backable)
		{
			//alright, so the router keeps track of the callbacks we use when chaining pages.
			//sounds fine, but there is a problem.... the got' dang back button.
			//
			//... so we have to do this on a few pages lol
			//
			//what kind of shitty programmer am i?
			Router.Instance.ClearCallbackStack();
		}


	}
}
