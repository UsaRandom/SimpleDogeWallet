using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal
{
	internal abstract class AppPage
	{
		public abstract void Draw(VirtualScreen screen);

		public abstract void OnBack();

		public abstract void OnReturned(object value);

		public abstract void OnNav(object value);
	}
}
