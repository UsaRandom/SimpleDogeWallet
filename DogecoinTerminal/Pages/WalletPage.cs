using DogecoinTerminal.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DogecoinTerminal.Pages.PinCodePage;

namespace DogecoinTerminal.Pages
{
	internal class WalletPage : AppPage
	{

		public WalletPage()
			: base(true)
		{

			Interactables.Add(
				new AppText("D8ZEVbgf4yPs3MK8dMJJ7PpSyBKsbd66TX",
							TerminalColor.White, 4, (50, 20)));

			Interactables.Add(
				new AppText("Đ420.69",
							TerminalColor.White, 8, (50, 30)));


			Interactables.Add(
				new AppButton("Send", (30, 40), (49, 55),
							  TerminalColor.DarkGrey, TerminalColor.White, 4,
							  (isFirst, self) =>
								{
									Router.Instance.Route("pin", new PinCodePageSettings("Amount to Send:", true), true,
										(dynamic value) =>
										{
											var amountToSend = float.Parse(value);

											Router.Instance.Route("msg", "You want to send Đ" + amountToSend, true);

										});
								}));

			Interactables.Add(
				new AppButton("Receive", (51, 40), (70, 55),
							  TerminalColor.DarkGrey, TerminalColor.White, 4,
							  (isFirst, self) =>
							  {

							  }));

			Interactables.Add(
				new AppButton("Update Pin", (30, 59), (49, 74),
							  TerminalColor.DarkGrey, TerminalColor.White, 4,
							  (isFirst, self) =>
							  {

								  Router.Instance.Route("pin", new PinCodePageSettings("Enter New Pin:", false), true,
									  (dynamic newPin) =>
									  {
										  Router.Instance.Route("pin", new PinCodePageSettings("Confirm Pin:", false), true,
										  (dynamic confirmPin) =>
										  {
											  Router.Instance.Route("msg", newPin == confirmPin ? "Pin Updated!" : "Pins did not match!", true);
										  });
									  });
							  }));


			Interactables.Add(
				new AppButton("Show Backup\n    Phrases", (51, 59), (70, 74),
							  TerminalColor.DarkGrey, TerminalColor.White, 4,
							  (isFirst, self) =>
							  {

							  }));



			Interactables.Add(
				new AppButton("Delete", (51, 78), (70, 93),
							  TerminalColor.Red, TerminalColor.White, 4,
							  (isFirst, self) =>
							  {

							  }));


		}


		public override void OnBack()
		{

		}

		protected override void OnNav(dynamic value, bool backable)
		{

		}
	}
}
