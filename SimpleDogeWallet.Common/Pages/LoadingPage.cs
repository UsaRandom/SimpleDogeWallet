using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDogeWallet.Common.Pages
{
    [PageDef("Pages/Xml/LoadingPage.xml")]
	public class LoadingPage : Page
	{

		private TextControl _loadingTextControl;

		public LoadingPage(IPageOptions options) : base(options)
		{
			_loadingTextControl = GetControl<TextControl>("LoadingPageText");
		}


		public string StringDef
		{
			get
			{
				return _loadingTextControl.StringDef;
			}
			set
			{
				_loadingTextControl.StringDef = value;
			}
		}

	}
}
