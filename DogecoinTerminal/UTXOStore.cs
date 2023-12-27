using DogecoinTerminal.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal
{
	internal class UTXOStore : IUTXOStore
	{
		private IWalletSlot _slot;
		private string _opPin;
		private string _slotPin;

		internal string UTXOFile
		{
			get
			{
				return $"slot_{_slot.SlotNumber}_utxos.dtf";
			}
		}

		private IList<UTXOInfo> _utxos;

		public UTXOStore(IWalletSlot slot)
		{
			_slot = slot;
		}


		public bool Unlock(string opPin, string slotPin)
		{
			_opPin = opPin;
			_slotPin = slotPin;

			if (File.Exists(UTXOFile))
			{
				try
				{
					var utxoFileContent = Crypto.Decrypt(Crypto.Decrypt(File.ReadAllText(UTXOFile), _opPin), _slotPin);

					LoadUTXOsFromString(utxoFileContent);

					return true;
				}
				catch
				{
					//UTXO Parse error, most likely our slot pin is incorrect.
					return false;
				}
				
			}

			_utxos = new List<UTXOInfo>();
			return true;
		}

		public IEnumerable<UTXOInfo> UTXOs
		{
			get
			{
				return _utxos;
			}
		}

		public void AddUTXO(UTXOInfo utxoInfo)
		{
			_utxos.Add(utxoInfo);
		}

		public void RemoveUTXO(UTXOInfo utxoInfo)
		{
			UTXOInfo utxoToRemove = null;

			foreach(var  utxo in _utxos)
			{
                if (utxo.TransactionId == utxoInfo.TransactionId &&
					utxo.VOut == utxoInfo.VOut &&
					utxo.Amount == utxoInfo.Amount)
                {
					utxoToRemove = utxo;
					break;
				}
            }

			if(utxoToRemove != null)
			{
				_utxos.Remove(utxoToRemove);
			}
		}


		public void OnWalletSlotDelete()
		{
			File.Delete(UTXOFile);
		}

		public void RemoveAll()
		{
			_utxos.Clear();
		}

		private void LoadUTXOsFromString(string utxoString)
		{
			_utxos = new List<UTXOInfo>();

			if (!string.IsNullOrEmpty(utxoString))
			{
				utxoString = utxoString.Replace("\r", string.Empty);

				var lines = utxoString.Split('\n');

				foreach (var line in lines)
				{
					if (string.IsNullOrEmpty(line))
					{
						continue;
					}

					var lineParts = line.Split('|');

					_utxos.Add(new UTXOInfo
					{
						TransactionId = lineParts[0],
						VOut = int.Parse(lineParts[1]),
						Amount = decimal.Parse(lineParts[2])
					});
				}
			}
		}

		public void Save()
		{
			var utxoContent = new StringBuilder();

			foreach (var utxo in _utxos)
			{
				utxoContent.Append(utxo.TransactionId + "|");
				utxoContent.Append(utxo.VOut + "|");
				utxoContent.AppendLine(utxo.Amount.ToString());
			}

			File.WriteAllText(UTXOFile, Crypto.Encrypt(Crypto.Encrypt(utxoContent.ToString(), _slotPin), _opPin));
		}

		public void UpdateOperatorPin(string newOperatorPin)
		{
			if (File.Exists(UTXOFile))
			{
				//update utxo file
				var utxoContent = Crypto.Decrypt(File.ReadAllText(UTXOFile), _opPin);
				File.WriteAllText(UTXOFile, Crypto.Encrypt(utxoContent, newOperatorPin));
			}
			_opPin = newOperatorPin;
		}

		public void UpdateSlotPin(string newSlotPin)
		{
			if (File.Exists(UTXOFile))
			{
				//update utxo file
				var utxoContent = Crypto.Decrypt(Crypto.Decrypt(File.ReadAllText(UTXOFile), _opPin), _slotPin);
				File.WriteAllText(UTXOFile, Crypto.Encrypt(Crypto.Encrypt(utxoContent, newSlotPin), _opPin));
			}
			_slotPin = newSlotPin;
		}

	}
}
