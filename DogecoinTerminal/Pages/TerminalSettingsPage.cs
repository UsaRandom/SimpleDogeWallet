using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DogecoinTerminal.Common.Components;
using DogecoinTerminal.Common;
using Microsoft.Xna.Framework;

namespace DogecoinTerminal.Pages
{
	internal class TerminalSettingsPage : AppPage
	{

		public TerminalSettingsPage(Game game)
			: base(game, true)
		{
			Interactables.Add(new AppText("Terminal Settings", TerminalColor.White, 5, (50, 10)));



			Interactables.Add(
				new AppButton("Update Op. Pin", (30, 40), (49, 55),
							  TerminalColor.DarkGrey, TerminalColor.White, 4,
							  (isFirst, self) =>
							  {
								  Game.Services.GetService<Router>().Route("pin", new PinCodePageSettings("Enter New Pin:", false), true,
									(dynamic newPin) =>
									{
										Game.Services.GetService<Router>().Route("pin", new PinCodePageSettings("Confirm Pin:", false), true,
										(dynamic confirmPin) =>
										{
											Game.Services.GetService<Router>().Route("msg", newPin == confirmPin ? "Pin Updated!" : "Pins did not match!", true);
										});
									});
							  }));

			Interactables.Add(
				new AppButton("RadioDoge", (51, 40), (70, 55),
							  TerminalColor.DarkGrey, TerminalColor.White, 4,
							  (isFirst, self) =>
							  {
								  Game.Services.GetService<Router>().Route("msg", "no work yet", true);
							  }));


			Interactables.Add(
				new AppButton("Remove Wallet", (30, 59), (49, 74),
							  TerminalColor.Red, TerminalColor.White, 4,
							  (isFirst, self) =>
							  {
								  Game.Services.GetService<Router>().Route("msg", "no work yet", true);
							  }));
		}


		public override void OnBack()
		{
			Game.Services.GetService<Router>().ClearCallbackStack();
		}

		protected override void OnNav(dynamic value, bool backable)
		{

		}
	}
}
