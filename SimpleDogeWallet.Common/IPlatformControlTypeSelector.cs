using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDogeWallet.Common
{
	public interface IPlatformControlTypeSelector
	{
		Type GetType(string typeName);
	}
}
