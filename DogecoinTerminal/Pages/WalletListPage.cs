using DogecoinTerminal.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Pages
{
	internal class WalletListPage : AppPage
	{
		public WalletListPage()
			:base(true)
		{
			Interactables.Add(
				new AppText("Select a Wallet:", TerminalColor.White, 6, (50, 33))

				) ;
			Interactables.Add(
				new AppButton("(Slot 1)", (25, 60), (35, 70),
					TerminalColor.LightGrey, TerminalColor.White, 5,
					(isFirst, self) =>
					{
						Router.Instance.Route("wallet", 1, true);
					}));
		}

		public override void OnBack()
		{
			//one of those places we should do this.
			Router.Instance.ClearCallbackStack();
		}

		protected override void OnNav(dynamic value, bool backable)
		{

		}
	}
}
