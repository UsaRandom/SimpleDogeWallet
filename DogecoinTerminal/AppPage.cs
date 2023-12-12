using DogecoinTerminal.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal
{
	internal abstract class AppPage
	{
		private bool _isBackable = false;

		private AppButton _backButton;

		protected AppPage(bool showDoge = false)
		{
			Interactables = new List<Interactable>();


			_backButton = new AppButton(
					"<", (5, 5), (15, 15),
					TerminalColor.LightGrey,
					TerminalColor.White,
					5,
					(isFirst, self) =>
					{
						if (isFirst && _isBackable)
						{

							Router.Instance.Back();

						}
					}
				);


			if(showDoge )
			{
				Interactables.Add(
					new AppImage(Images.DogeImage,
						(90, 10), (10, 10), Images.DogeImageDim)
					);

				Interactables.Add(
					new AppText("Dogecoin Terminal", TerminalColor.White, 3, (90, 25))
					);
			}
		}


		public IList<Interactable> Interactables { get; set; }


		public void DrawScreen(VirtualScreen screen)
		{
			foreach (var item in Interactables)
			{
				item.Draw(screen);
			}
		}


		public abstract void OnBack();


		protected abstract void OnNav(dynamic value, bool backable);

		public void OnNavigation(dynamic value, bool backable)
		{
			_isBackable = backable;

			if (_isBackable)
			{
				Interactables.Add(_backButton);
			}
			else
			{
				Interactables.Remove(_backButton);
			}

			OnNav(value, backable);
		}
	}
}
