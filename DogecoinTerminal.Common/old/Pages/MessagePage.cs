using DogecoinTerminal.Common.old.Components;
using DogecoinTerminal.Common.old;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace DogecoinTerminal.Common.old
{
	public class MessagePage : AppPage
	{
		private AppText MessageText;
		public MessagePage(Game game)
			:base(game)
		{
			Interactables.Add(
				new AppImage(Images.DogeImage,
					(43, 8), (57, 23), Images.DogeImageDim)
				);

			MessageText = new AppText(string.Empty, TerminalColor.White, 5, (50, 50));

			Interactables.Add(MessageText);

			Interactables.Add(
				new AppButton("wow",
						(40, 80), (60, 90),
						TerminalColor.Blue,
						TerminalColor.White,
						5,
						(isFirst, self) =>
						{
							Game.Services.GetService<Router>().Return("wow");
						}));
		}


		public override void OnBack()
		{
			Game.Services.GetService<Router>().Back();
		}

		protected override void OnNav(dynamic value, bool backable)
		{
			MessageText.Text = value;
		}


	}
}
