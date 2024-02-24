﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Common.Pages
{
    [PageDef("Pages/Xml/YesNoPage.xml")]
	public class YesNoPage : PromptPage
	{
		public YesNoPage(IPageOptions options) : base(options)
		{
			var msg = options.GetOption<string>("message");

			var msgControl = GetControl<TextControl>("MessageText");
			msgControl.Text = msg;

			OnClick("YesButton", _ =>
			{
				Submit();
			});

			OnClick("NoButton", _ =>
			{
				Cancel();
			});

		}
	}
}
