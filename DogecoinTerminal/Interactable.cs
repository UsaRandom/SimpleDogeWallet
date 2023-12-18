using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal
{
	internal abstract class Interactable
	{

		public (int x, int y) Start;
		public (int x, int y) End;


		public Interactable((int x, int y) start,
							(int x, int y) end,
							Action<bool, Interactable> onInteract)
		{
			Start = start;
			End = end;
			OnInteract = onInteract ?? ((_,_) => { });
		}


		public abstract void Draw(VirtualScreen screen);


		public Action<bool, Interactable> OnInteract { get; private set; }


	}
}
