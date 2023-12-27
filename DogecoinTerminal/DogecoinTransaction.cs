using DogecoinTerminal.Common;
using Lib.Dogecoin;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal
{
	internal class DogecoinTransaction : IDogecoinTransaction
	{

		private int _workingTransactionId;
		private LibDogecoinContext _ctx;
		private IWalletSlot _slot;
		private Game _game;
		private List<UTXOInfo> _txUTXOs;

		public decimal Fee { get; private set; }

		public decimal Amount { get; private set; }

		public decimal Total
		{
			get
			{
				return Fee + Amount;
			}
		}

		internal decimal Remainer
		{
			get;
			private set;
		}

		public string Recipient { get; private set; }

		public string From { get; private set; }

		public DogecoinTransaction(Game game, IWalletSlot slot)
		{
			_game = game;
			_slot = slot;
			_txUTXOs = new List<UTXOInfo>();
			_ctx = LibDogecoinContext.CreateContext();
		}



		public bool Send(string recipient, decimal amount)
		{
			var settings = _game.Services.GetService<ITerminalSettingsService>();

			var dustLimit = settings.Get<decimal>("dust-limit");

			if (amount < dustLimit)
			{
				return false;
			}

			_ctx.RemoveAllTransactions();

			_workingTransactionId = _ctx.StartTransaction();

			//it might make sense to order these decending
			var utxoEnumerator = _slot.UTXOStore.UTXOs.GetEnumerator();

			decimal fee = settings.Get<decimal>("fee-per-utxo"); //fee per utxo
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

				if(!_ctx.AddUTXO(_workingTransactionId, utxo.TransactionId, utxo.VOut))
				{
					return false;
				}

			} while (sum < amount + (fee * utxoCount));

			if (sum < amount + (fee * utxoCount))
			{
				return false;
			}

			var totalFee = (fee * utxoCount);

			var remainder = sum - totalFee - amount;

			var amountStr = amount.ToString();
			var feeStr = totalFee.ToString();
			var sumStr = sum.ToString();
			var remainderStr = remainder.ToString();


			this.Remainer = remainder;
			this.Fee = fee;
			this.Amount = amount;
			
			this.Recipient = recipient;
			this.From = _slot.Address;


			if(this.Remainer > dustLimit)
			{

				if (!_ctx.AddOutput(_workingTransactionId, this.From, remainderStr))
				{
					return false;
				}
			}

			if (!_ctx.AddOutput(_workingTransactionId, this.Recipient, amountStr))
			{
				return false;
			}

			return true;
		}

		public bool Sign()
		{
			foreach(var utxo in _txUTXOs)
			{
				if(!_ctx.SignTransactionWithPrivateKey(_workingTransactionId, utxo.VOut, GetPrivateKeyFromMnemonic(_slot.GetMnemonic())))
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
			foreach(var utxo in _txUTXOs)
			{
				_slot.UTXOStore.RemoveUTXO(utxo);
			}

			var settings = _game.Services.GetService<ITerminalSettingsService>();

			if (Remainer > settings.Get<decimal>("dust-limit"))
			{

				_slot.UTXOStore.AddUTXO(new UTXOInfo
				{
					TransactionId = Crypto.GetTransactionIdFromRaw(GetRawTransaction()),
					VOut = 0,// by our convention, our first output is back to ourselves.
					Amount = Remainer
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
