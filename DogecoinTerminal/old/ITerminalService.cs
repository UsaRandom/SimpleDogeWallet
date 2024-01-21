using DogecoinTerminal.Common;
using System;
using System.Collections.Generic;

namespace DogecoinTerminal.old
{
    internal interface ITerminalService
    {
        bool IsUnlocked { get; }

        bool ConfirmOperatorPin(string pin);

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

        string ShortAddress { get; }

        bool IsEmpty { get; }
        string SlotPin { get; }

        IUTXOStore UTXOStore { get; }



        bool Unlock(string slotPin);
        void Lock();

        bool Init(string slotPin);

        void UpdateOperatorPin(string newOperatorPin);

        void UpdateSlotPin(string slotPin);

        void ClearSlot();


        string GetMnemonic();

        string CalculateBalance();


        IDogecoinTransaction CreateTransaction(string receipient, decimal amount);

    }

    internal interface IUTXOStore
    {
        IEnumerable<UTXO> UTXOs { get; }

        void AddUTXO(UTXO utxoInfo);

        void RemoveUTXO(UTXO utxoInfo);

        void RemoveAll();

        void Save();
    }

    internal interface IDogecoinTransaction : IDisposable
    {
        decimal Fee { get; }

        decimal Amount { get; }

        decimal Total { get; }

        string To { get; }

        string From { get; }


        bool Send(string recipient, decimal amount);

        bool Sign();

        string GetRawTransaction();

        void Commit();

    }
}
