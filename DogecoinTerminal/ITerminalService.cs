using DogecoinTerminal.Common;
using System;
using System.Collections.Generic;

namespace DogecoinTerminal
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
        bool IsEmpty { get; }
        string SlotPin { get; }

        IUTXOStore UTXOStore { get; }



        bool Unlock(string slotPin);
        void Lock();

        void Init(string slotPin);

        void UpdateOperatorPin(string newOperatorPin);

        void UpdateSlotPin(string slotPin);

        void ClearSlot();


        string GetMnemonic();

        string CalculateBalance();


        IDogecoinTransaction CreateTransaction(string receipient, decimal amount);

    }

    internal interface IUTXOStore
    {
        IEnumerable<UTXOInfo> UTXOs { get; }

        bool Unlock(string opPin, string slotPin);

        void AddUTXO(UTXOInfo utxoInfo);

        void RemoveUTXO(UTXOInfo utxoInfo);

        void RemoveAll();

        void OnWalletSlotDelete();

        void UpdateOperatorPin(string newOperatorPin);

        void UpdateSlotPin(string slotPin);

        void Save();
    }

    internal interface IDogecoinTransaction : IDisposable
    {

        decimal Fee { get; }

        decimal Amount { get; }

        decimal Total { get; }

        string Recipient { get; }

        string From { get; }


        bool Send(string recipient, decimal amount);

        bool Sign();

        string GetRawTransaction();

        void Commit();

    }
}
