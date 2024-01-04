using Lib.Dogecoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal
{
	internal class TPM2MnemonicProvider : IMnemonicProvider
	{
		public string GetMnemonic(LibDogecoinContext ctx, int slotId)
		{
			return ctx.GenerateMnemonicEncryptWithTPM(slotId+1);
		}
	}
	internal class Simple24WordMnemonicProvider : IMnemonicProvider
	{
		public string GetMnemonic(LibDogecoinContext ctx, int slotId)
		{
			return ctx.GenerateRandomEnglishMnemonic(LibDogecoinContext.ENTROPY_SIZE_256);
		}
	}
}
