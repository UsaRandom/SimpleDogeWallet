using Lib.Dogecoin;
using System;
using System.Collections.Generic;
using DogecoinTerminal.Common;


namespace DogecoinTerminal
{
    internal class DogecoinTransaction
    {

        private int _workingTransactionId;
        private LibDogecoinContext _ctx;
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

        public SimpleDogeWallet Wallet { get; private set; }

		public DogecoinTransaction(IServiceProvider services, SimpleDogeWallet wallet)
        {
            _services = services;
            Wallet = wallet;
            _txUTXOs = new List<UTXO>();
            _ctx = services.GetService<LibDogecoinContext>();
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
            var utxoEnumerator = Wallet.UTXOs.GetEnumerator();


            //NOTE: I might want to change this to fee per byte.
            decimal fee = settings.GetDecimal("fee-per-utxo"); //fee per utxo
            decimal sum = 0M;
            int utxoCount = 2; //min 2 utxo (receipient + change)

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

                if (!_ctx.AddUTXO(_workingTransactionId, utxo.TxId, utxo.VOut))
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
            From = Wallet.Address;


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
            try
			{
				var masterKeys = _ctx.GenerateHDMasterPubKeypairFromMnemonic(Wallet.GetMnemonic().Replace("-", " "));

				var pk = _ctx.GetHDNodePrivateKeyWIFByPath(masterKeys.privateKey, Crypto.HDPATH, true);
		
				for (var i = 0; i < _txUTXOs.Count; i++)
				{
					if (!_ctx.SignTransactionWithPrivateKey(_workingTransactionId, i, pk))
					{
						return false;
					}
				}
				return true;
			}
            finally
            {
                //force collection after signing
				GC.Collect(0, GCCollectionMode.Forced, true);
				GC.Collect(1, GCCollectionMode.Forced, true);
			}
        }


        public string GetRawTransaction()
        {
            return _ctx.GetRawTransaction(_workingTransactionId);
        }
    }
}
