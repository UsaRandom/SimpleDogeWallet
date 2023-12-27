
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DogecoinTerminal.Common.Components;
using DogecoinTerminal.Common;
using Microsoft.Xna.Framework;
using System.Diagnostics.Metrics;

namespace DogecoinTerminal.Pages
{
	internal class UnlockTerminalPage : AppPage
	{

		public UnlockTerminalPage(Game game)
			: base(game)
		{
			Interactables.Add(
				new AppImage(Images.DogeImage,
					(43, 18), (57, 33), Images.DogeImageDim)
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
							var terminalService = Game.Services.GetService<ITerminalService>();
							var router = Game.Services.GetService<Router>();

							if(terminalService.Unlock(string.Empty))
							{
								router.Route("pin", new PinCodePageSettings("Set Operator Pin", false), true,
								(enteredPin) =>
								{
									if (string.IsNullOrEmpty(enteredPin))
									{
										return;
									}

									router.Route("pin", new PinCodePageSettings("Confirm Operator Pin", false), true,
									(confirmPin) =>
									{
										if (enteredPin != confirmPin)
										{
											return;
										}

										terminalService.UpdateOperatorPin(enteredPin);
										terminalService.Unlock(enteredPin);


										Game.Services.GetService<IDogecoinService>().OnSetup((setupSuccess) =>
										{
											if(setupSuccess)
											{
												Game.Services.GetService<Router>().Route("wallets", null, false);
											}
											else
											{
												Game.Services.GetService<Router>().Back();
											}
										});

									});
								});
							}
							else
							{
								Game.Services.GetService<Router>().Route("pin",
									new PinCodePageSettings("Enter Operator Pin to Unlock", false), true,
									(dynamic enteredPin) =>
									{
										if (terminalService.Unlock(enteredPin))
										{
											Game.Services.GetService<Router>().Route("wallets", null, false);
										}
									});
							}

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
			Game.Services.GetService<Router>().ClearCallbackStack();
		}


	}
}
