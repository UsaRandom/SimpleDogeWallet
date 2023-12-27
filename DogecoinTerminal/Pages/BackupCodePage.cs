
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DogecoinTerminal.Common;
using DogecoinTerminal.Common.Components;
using Microsoft.Xna.Framework;

namespace DogecoinTerminal.Pages
{
	internal class BackupCodePage : AppPage
	{
		private string[] _backupCodes;
		private AppButton _nextButton;

		public BackupCodePage(Game game)
			: base(game, false)
		{

			var count = 1;

			for(var x = 10; x < 90; x += 20)
			{
				for(var y = 22; y < 90; y += 12)
				{
					Interactables.Add(
						new AppButton(count + ". wow",
									 (x,y), (x+18, y+10),
									 TerminalColor.DarkGrey,
									 TerminalColor.White, 3,
									 (_,_) => {  }));
					count++;
				}
			}


			Interactables.Add((_nextButton = new AppButton("Next", (90, 90), (98, 98), TerminalColor.Green, TerminalColor.White, 4, (isFirst, self) =>
			{
				Game.Services.GetService<Router>().Return("wow");
			})));


			Interactables.Add(new AppText("Backup Phrases", TerminalColor.White, 5, (50, 10)));

		}

		public override void OnBack()
		{
			for (var i = 0; i < 24; i++)
			{
				((AppButton)Interactables[i]).Text = $"{i + 1}. wow";
			}
			Game.Services.GetService<Router>().Back();
		}

		protected override void OnNav(dynamic value, bool backable)
		{

			var settings = (BackupCodePageSettings)value;

			Interactables.Remove(_nextButton);

			if(settings.ShowNextButton)
			{
				Interactables.Add(_nextButton);
			}


			for (var i = 0; i < 24; i++)
			{
				((AppButton)Interactables[i]).Text = $"{i + 1}. wow";
			}

			var words = settings.Mnemonic.Split(' ');

			for(var i = 0; i < 24; i++)
			{
				((AppButton)Interactables[i]).Text = $"{i+1}. {words[i]}";
			}


		}



	}

	internal class BackupCodePageSettings
	{
		public BackupCodePageSettings(string mnemonic, bool showNextButton)
		{
			Mnemonic = mnemonic;
			ShowNextButton = showNextButton;
		}

		public string Mnemonic { get; private set; }
		public bool ShowNextButton { get; private set; }
	}
}
