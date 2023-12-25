using DogecoinTerminal.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal
{
	internal interface ITerminalService
	{
		bool IsUnlocked { get; }

		bool Unlock(string operatorPin);
		void Lock();


		bool UpdateOperatorPin(string newOperatorPin);
		IWalletSlot GetWalletSlot(int slot);
		void ClearSlot(int slot);
	}


	internal interface IWalletSlot
	{
		bool IsUnlocked { get; }
		int SlotNumber { get; }
		string Address { get; }
		bool IsEmpty { get; }
		string SlotPin { get; }

		IEnumerable<UTXOInfo> UTXOs { get; }


		bool Unlock(string slotPin);
		void Lock();

		void Init(string slotPin);

		void UpdateOperatorPin(string newOperatorPin);

		void UpdateSlotPin(string slotPin);

		void ClearSlot();


		string GetMnemonic();

		string CalculatetBalance();

		void UpdateUTXOs(IEnumerable<UTXOInfo> utxos);

		string CreateTransaction(string receipient, decimal amount);
	}
}
