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

		protected AppPage()
		{
			Interactables = new List<Interactable>();


			_backButton = new AppButton(
					"<", (5, 5), (10, 10),
					TerminalColor.DarkGrey,
					TerminalColor.White,
					2,
					(isFirst) =>
					{
						if (isFirst && _isBackable)
						{

							Router.Instance.Back();

						}
					}
				);
		}


		public IList<Interactable> Interactables { get; set; }


		protected abstract void Draw(VirtualScreen screen);


		public void DrawScreen(VirtualScreen screen)
		{
			foreach (var item in Interactables)
			{
				item.Draw(screen);
			}

			this.Draw(screen);
		}

		public abstract void Update();

		public abstract void OnBack();

		public abstract void OnReturned(object value);

		protected abstract void OnNav(object value, bool backable);

		public void OnNavigation(object value, bool backable)
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
