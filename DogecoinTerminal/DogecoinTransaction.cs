using Lib.Dogecoin;
using System;
using System.Collections.Generic;
using DogecoinTerminal.Common;


namespace DogecoinTerminal
{
	internal class DogecoinTransaction : IDogecoinTransaction
    {

        private int _workingTransactionId;
        private LibDogecoinContext _ctx;
        private IWalletSlot _slot;
        private IServiceProvider _services;
        private List<UTXO> _txUTXOs;

        public decimal Fee { get; private set; }

        public decimal Amount { get; private set; }

        public decimal Total
        {
            get
            {
                return Fee + Amount;
            }
        }

        internal decimal Remainder
        {
            get;
            private set;
        }

        public string To { get; private set; }

        public string From { get; private set; }

        public DogecoinTransaction(IServiceProvider services, IWalletSlot slot)
        {
            _services = services;
            _slot = slot;
            _txUTXOs = new List<UTXO>();
            _ctx = LibDogecoinContext.CreateContext();
        }



        public bool Send(string recipient, decimal amount)
        {
            var settings = _services.GetService<ITerminalSettings>();

            var dustLimit = settings.GetDecimal("dust-limit");

            if (amount < dustLimit)
            {
                return false;
            }

            _ctx.RemoveAllTransactions();

            _workingTransactionId = _ctx.StartTransaction();

            //it might make sense to order these
            var utxoEnumerator = _slot.UTXOStore.UTXOs.GetEnumerator();

            decimal fee = settings.GetDecimal("fee-per-utxo"); //fee per utxo
            decimal sum = 0M;
            int utxoCount = 0;

            do
            {
                if (!utxoEnumerator.MoveNext())
                {
                    break;
                }

                var utxo = utxoEnumerator.Current;

                sum += utxo.Amount;

                utxoCount++;

                _txUTXOs.Add(utxo);

                if (!_ctx.AddUTXO(_workingTransactionId, utxo.TransactionId, utxo.VOut))
                {
                    return false;
                }

            } while (sum < amount + fee * utxoCount);

            if (sum < amount + fee * utxoCount)
            {
                return false;
            }

            var totalFee = fee * utxoCount;

            var remainder = sum - totalFee - amount;

            var amountStr = amount.ToString();
            var feeStr = totalFee.ToString();
            var sumStr = sum.ToString();
            var remainderStr = remainder.ToString();


            Remainder = remainder;
            Fee = fee;
            Amount = amount;

            To = recipient;
            From = _slot.Address;


            if (Remainder > dustLimit)
            {

                if (!_ctx.AddOutput(_workingTransactionId, From, remainderStr))
                {
                    return false;
                }
            }

            if (!_ctx.AddOutput(_workingTransactionId, To, amountStr))
            {
                return false;
            }

            return true;
        }

        public bool Sign()
        {
            for (var i = 0; i < _txUTXOs.Count; i++)
            {
                if (!_ctx.SignTransactionWithPrivateKey(_workingTransactionId, i, GetPrivateKeyFromMnemonic(_slot.GetMnemonic())))
                {
                    return false;
                }
            }
            return true;
        }




        private string GetPrivateKeyFromMnemonic(string mnemonic)
        {
            var masterKeys = _ctx.GenerateHDMasterPubKeypairFromMnemonic(mnemonic);

            return _ctx.GetHDNodePrivateKeyWIFByPath(masterKeys.privateKey, Crypto.HDPATH, true);
        }

        public void Commit()
        {
            foreach (var utxo in _txUTXOs)
            {
                _slot.UTXOStore.RemoveUTXO(utxo);
            }

            var settings = _services.GetService<ITerminalSettings>();

            if (Remainder > settings.GetDecimal("dust-limit"))
            {
                _slot.UTXOStore.AddUTXO(new UTXO
                {
                    TransactionId = Crypto.GetTransactionIdFromRaw(GetRawTransaction()),
                    VOut = 0,// by our convention, our first output is back to ourselves.
                    Amount = Remainder
                });
            }

            _slot.UTXOStore.Save();
        }

        public void Dispose()
        {
            _ctx.ClearTransaction(_workingTransactionId);
            _ctx.Dispose();
        }

        public string GetRawTransaction()
        {
            return _ctx.GetRawTransaction(_workingTransactionId);
        }
    }
}
