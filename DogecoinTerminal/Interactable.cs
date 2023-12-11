using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal
{
	internal abstract class Interactable
	{
		public Interactable(Action<bool> onInteract)
		{
			OnInteract = onInteract;
		}

		public Action<bool> OnInteract { get; private set; }


	}
}
