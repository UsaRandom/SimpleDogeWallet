using DogecoinTerminal.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DogecoinTerminal.Pages.PinCodePage;

namespace DogecoinTerminal.Pages
{
	internal class WalletListPage : AppPage
	{
		public WalletListPage()
			:base(true)
		{
			Interactables.Add(
				new AppText("Select a Wallet/Slot:", TerminalColor.White, 6, (50, 20))
				) ;
			
			Interactables.Add(
				new AppButton("D8ZE..6TX", (15, 40), (35, 60),
					TerminalColor.DarkGrey, TerminalColor.White, 5,
					(isFirst, self) =>
					{
						Router.Instance.Route("pin", new PinCodePageSettings("Enter Pin:", false), true,
							(dynamic value) =>
							{
								if(value == "6969")
								{
									Router.Instance.Route("wallet", null, true);
								}
							});
					}));

			Interactables.Add(
				new AppButton("(Slot 2)", (40, 40), (60, 60),
					TerminalColor.LightGrey, TerminalColor.White, 5,
					(isFirst, self) =>
					{
						Router.Instance.Route("msg", "No work yet!", true);
					}));


			Interactables.Add(
				new AppButton("(Slot 3)", (65, 40), (85, 60),
					TerminalColor.LightGrey, TerminalColor.White, 5,
					(isFirst, self) =>
					{
						Router.Instance.Route("msg", "No work yet!", true);
					}));

			Interactables.Add(
				new AppButton("(Slot 4)", (15, 65), (35, 85),
					TerminalColor.LightGrey, TerminalColor.White, 5,
					(isFirst, self) =>
					{
						Router.Instance.Route("msg", "No work yet!", true);
					}));

			Interactables.Add(
				new AppButton("(Slot 5)", (40, 65), (60, 85),
					TerminalColor.LightGrey, TerminalColor.White, 5,
					(isFirst, self) =>
					{
						Router.Instance.Route("msg", "No work yet!", true);
					}));


			Interactables.Add(
				new AppButton("(Slot 6)", (65, 65), (85, 85),
					TerminalColor.LightGrey, TerminalColor.White, 5,
					(isFirst, self) =>
					{
						Router.Instance.Route("msg", "No work yet!", true);
					}));


			Interactables.Add(
				new AppButton("Lock", (2, 88), (12, 98),
					TerminalColor.DarkGrey, TerminalColor.White, 3,
					(isFirst, self) =>
					{
						Router.Instance.Route("home", null, false);
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
