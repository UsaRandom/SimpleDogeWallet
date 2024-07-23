using SimpleDogeWallet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDogeWallet.WinForms
{
	public class WinFormsPlatformControlTypeSelector : DefaultPlatformControlTypeSelector
	{
		public override Type GetType(string typeName)
		{
			if(typeName == "SimpleDogeWallet.Common.TextInputControl")
			{
				return typeof(WinFormsTextInputControl);
			}
			return Type.GetType(typeName) ?? base.GetType(typeName);
		}
	}
}
