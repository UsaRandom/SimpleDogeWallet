using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Components
{
	internal class AppButton : Interactable
	{

		public AppButton(string name, Action<bool> onInteract)
			: base(onInteract)
		{
			MyName = name;
		}

		public string MyName
		{
			get;set;
		}
	}
}
