using Lib.Dogecoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal
{
	public interface IMnemonicProvider
	{
		string GetMnemonic(LibDogecoinContext ctx, int slotId);
	}



}
