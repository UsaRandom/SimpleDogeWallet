using DogecoinTerminal.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Pages
{
	internal class BackupCodePage : AppPage
	{
		private string[] _backupCodes;

		public BackupCodePage()
			: base(false)
		{
			var count = 1;

			for(var x = 10; x < 90; x += 20)
			{
				for(var y = 22; y < 90; y += 12)
				{
					Interactables.Add(
						new AppButton(count + " wow",
									 (x,y), (x+18, y+10),
									 TerminalColor.DarkGrey,
									 TerminalColor.White, 3,
									 (_,_) => {  }));
					count++;
				}
			}


		}

		public override void OnBack()
		{

		}

		protected override void OnNav(dynamic value, bool backable)
		{

		}
	}
}
