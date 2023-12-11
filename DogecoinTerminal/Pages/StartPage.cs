using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Pages
{
	internal class StartPage : AppPage
	{
		public override void Draw(VirtualScreen screen)
		{
			screen.DrawRectangle(TerminalColor.Red,
				(30,30), (36,34));

			screen.DrawText("Hello World!", TerminalColor.White, (31, 31));
		}

		public override void OnBack()
		{
			throw new NotImplementedException();
		}

		public override void OnNav(object value)
		{
			
		}

		public override void OnReturned(object value)
		{
			throw new NotImplementedException();
		}
	}
}
