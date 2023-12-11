using DogecoinTerminal.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Pages
{
	internal class StartPage : AppPage
	{

		public StartPage()
		{
			Interactables.Add(
				new AppButton("Go To Test Page",
						(30, 30), (40, 50),
						(isFirst) =>
						{
							Router.Instance.Route("test", null, false);
						}));
		}

		public override void Draw(VirtualScreen screen)
		{
			foreach (var item in Interactables)
			{
				item.Draw(screen);
			}
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

		public override void Update()
		{

		}
	}
}
