using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Common.Pages
{
	[PageDef("Pages/Xml/ShortMessagePage.xml")]
	public class ShortMessagePage : PromptPage
	{
		public ShortMessagePage(IPageOptions options) : base(options)
		{
			//ShortMessagePage requires a 'message' option provided to display.
			var msg = options.GetOption<string>("message");

			var msgControl = GetControl<TextControl>("MessageText");
			msgControl.Text = msg;

			OnClick("OkButton", _ =>
			{

				//PromptPages support the submit/cancel functionality
				Submit();
			});

			OnClick("BackButton", _ =>
			{
				Cancel();
			});

		}
	}
}
