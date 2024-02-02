using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal
{
	/*
	 * Not production stuff, like a little playground.
	 * FYI: this file isn't fed to the compiler
	 */
	internal class Notes
	{


		class SimpleWallet
		{
			string Address;
			string GetMnemonic();
			UTXO[] GetUTXOs();
			void AddUTXO(UTXO utxo);
			void RemoveUTXO(UTXO utxo);
			decimal GetBalance();
			long GetBalanceInKoinu();
		}

		class SimpleWalletFileStore
		{
			bool WalletExists
			{
				get
				{
					return false;
				}
			}

			SimpleWallet TryUnlock(string pin);
			bool UpdatePin(string oldPin, string newPin);
		}

		private void Playground()
		{

			var wallet = new SimpleTPM2WalletBuilder()
								.UseProvidedMnemonic(string mnemonic)
								.UseNewRandomMnemonic()
								.



			var wallet = new SimpleTPM2Wallet();

			var address = wallet.Address;
			var mnemonic = wallet.GetMnemonic();

			wallet.AddUTXO(UTXO);
			wallet.RemoveUTXO(UTXO);

			wallet.GetBalance();

			if(wallet.TryUnlock(pin))
			{

			}

			wallet.SetPin();
			
		}

	}

	internal class SimpleTPM2Wallet
	{

	}
}
