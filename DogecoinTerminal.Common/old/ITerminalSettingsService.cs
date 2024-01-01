using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Common.old
{
	public interface ITerminalSettingsService
	{
		T Get<T>(string settingName);

		void Set(string settingName, object value);

		bool IsSet(string settingName);
	}
}
