using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal
{
	internal abstract class AppPage
	{
		protected AppPage()
		{
			Interactables = new List<Interactable>();
		}


		public IList<Interactable> Interactables { get; set; }


		public abstract void Draw(VirtualScreen screen);

		public abstract void Update();

		public abstract void OnBack();

		public abstract void OnReturned(object value);

		public abstract void OnNav(object value);
	}
}
