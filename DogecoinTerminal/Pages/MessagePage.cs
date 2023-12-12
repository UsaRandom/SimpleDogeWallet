using DogecoinTerminal.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Pages
{
	internal class MessagePage : AppPage
	{
		private AppText MessageText;
		public MessagePage()
		{
			Interactables.Add(
				new AppImage(Images.DogeImage,
					(50, 25), (15, 15), Images.DogeImageDim)
				);

			MessageText = new AppText(string.Empty, TerminalColor.White, 5, (50, 50));

			Interactables.Add(MessageText);

			Interactables.Add(
				new AppButton("wow",
						(40, 60), (60, 70),
						TerminalColor.Blue,
						TerminalColor.White,
						5,
						(isFirst, self) =>
						{
							Router.Instance.Return("wow");
						}));
		}


		public override void OnBack()
		{
			Router.Instance.Back();
		}

		protected override void OnNav(dynamic value, bool backable)
		{
			MessageText.Text = value;
		}


	}
}
