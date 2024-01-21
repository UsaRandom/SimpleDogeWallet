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

		private void Playground()
		{
			var settings = new TerminalSettings();

			await navigation.PushAsync<BlankPage>();

			var acknowledge = await navigation.PromptAsync<ShortMessagePage>(("message", "Don't share seed phrase, have pen & paper ready!"));

			if (acknowledge.Response == PromptResponse.YesConfirm)
			{
				string mnemonic = string.Empty;

				using (var ctx = LibDogecoinContext.CreateContext())
				{
					mnemonic = mnemonicProvider.GetMnemonic(ctx, slot.SlotNumber);
				}

				if (!string.IsNullOrEmpty(mnemonic))
				{
					await navigation.TryInsertBeforeAsync<BackupCodePage, BlankPage>(("mnemonic", mnemonic));
				}
			}

			navigation.Pop();


			new TerminalFlowBuilder()
					.ConfirmPrompt<ShortMessagePage>(("message", "Don't share seed phrase, have pen & paper ready!"))
					.If()
					
		}



		public void WalletTypes()
		{
			//separate our terminal, wallet and common services/pages

			/*
			 * Terminal Pages:
			 *  - Setup
			 *  - Operator Pin Page
			 *  - Unlock Terminal Pin Page
			 *  - Home Page
			 *  - Settings
			 *  - Wallet Type Selector
			 *  
			 * Services:
			 *	 - TerminalService 
			 *		- await ConfirmOperatorPin():bool
			 *		- 
			 */


			/*
			 * Common Pages:
			 *  - Mnemonics Display Page
			 *  
			 * Services:
			 *  - Crypto.cs
			 *  - TPM could be a shared service.
			 *  - Selected Object
			 *  
			 *  
			 */

			/*
			 * Wallet Services/Pages:
			 *  - QRDogeWalletPage
			 *  - UTXOStore
			 *  - DogecoinTransaction
			 * 
			 */

			

			var qrDoge = new QRDogeWallet(int slot);



		}

	}
}
