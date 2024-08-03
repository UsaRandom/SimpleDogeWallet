using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDogeWallet.Common
{
	public class DefaultPlatformControlTypeSelector : IPlatformControlTypeSelector
	{
		public virtual Type GetType(string typeName)
		{
			return Type.GetType(typeName);
		}
	}
}
