using DogecoinTerminal.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Pages
{
	internal class UnlockTerminalPage : AppPage
	{

		public UnlockTerminalPage()
		{
			Interactables.Add(
				new AppText("Dogecoin Terminal", TerminalColor.White, 1.5, (50, 50))
				);

			Interactables.Add(
				new AppButton("Unlock",
						(40, 60), (60, 70),
						TerminalColor.Green,
						TerminalColor.White,
						1,
						(isFirst) =>
						{
							Router.Instance.Route("test", null, true);
						}));
		}


		protected override void Draw(VirtualScreen screen)
		{

		}

		public override void OnBack()
		{

		}

		protected override void OnNav(object value, bool backable)
		{
			
		}

		public override void OnReturned(object value)
		{
			throw new NotImplementedException();
		}

		public override void Update()
		{

		}
	}
}
