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

		public UnlockTerminalPage()
		{
			Interactables.Add(
				new AppImage(Images.DogeImage,
					(50, 25), (15, 15), Images.DogeImageDim)
				);

			Interactables.Add(
				new AppText("Dogecoin Terminal", TerminalColor.White, 1.5, (50, 50))
				);

			Interactables.Add(
				new AppButton("Unlock",
						(40, 60), (60, 70),
						TerminalColor.Green,
						TerminalColor.White,
						1,
						(isFirst, self) =>
						{
							Router.Instance.Route("pin",
								new PinCodePageSettings("Enter Pin", false), true,
								(dynamic enteredPin) =>
								{
									Router.Instance.Route("pin",
										new PinCodePageSettings("Confirm Pin", false), true,
										(dynamic confirmedPin) =>
										{

											if(enteredPin == confirmedPin)
											{
												Router.Instance.Route("msg", "Pin Confirmed: " + enteredPin, true);
											}

										});
								});
						}));
		}


		protected override void Draw(VirtualScreen screen)
		{

		}

		public override void OnBack()
		{

		}

		protected override void OnNav(dynamic value, bool backable)
		{
			
		}


		public override void Update()
		{

		}
	}
}
