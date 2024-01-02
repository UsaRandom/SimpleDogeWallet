using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Common.Pages
{
	[PageDef("Pages/Xml/NumPadPage.xml")]
	public  class NumPadPage : PromptPage
	{

		public NumPadPage(IPageOptions options) : base(options)
		{


			for (var i = 0; i < 10; i++)
			{
				OnClick($"Button_{i}", _ => { AddText($"{i}"); });
			}

			OnClick($"Button_.", _ => { AddText("."); });
			OnClick($"Button_Delete", _ => { DeleteChar(); });
		}


		private void AddText(string text)
		{
		}

		private void DeleteChar()
		{

		}
	}
}
