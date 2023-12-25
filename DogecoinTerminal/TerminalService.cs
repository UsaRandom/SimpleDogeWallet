using DogecoinTerminal.Common;
using Lib.Dogecoin;
using Microsoft.Xna.Framework;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace DogecoinTerminal
{
	internal class TerminalService : ITerminalService
	{
		private const int MAX_SLOT_COUNT = 6;
		private const string OP_PIN_VERIFY_FILE = "op_pin_verify.dtf";
		private const string OP_PIN_VERIFY_STATEMENT = "much verify, very wow";


		private Game _game;
		private string _opPin;
		private IWalletSlot[] _slots;

		public bool IsUnlocked { get; private set; }

		public TerminalService(Game game)
		{
			_slots = new IWalletSlot[MAX_SLOT_COUNT];
			_game = game;
			_opPin = string.Empty;
			IsUnlocked = false;
		}

		public void ClearSlot(int slot)
		{
			if(IsUnlocked)
			{
				GetWalletSlot(slot).ClearSlot();
			}
		}

		public IWalletSlot GetWalletSlot(int slot)
		{
			if (_slots[slot] == null)
			{
				_slots[slot] = new WalletSlot(_game, _opPin, slot);
			}

			return _slots[slot];
		}

		public void Lock()
		{
			_opPin = string.Empty;
			IsUnlocked = false;
		}

		public bool Unlock(string operatorPin)
		{
			if(OpPinIsSet() && operatorPin != string.Empty)
			{
				if(Crypto.Decrypt(File.ReadAllText(OP_PIN_VERIFY_FILE), operatorPin) == OP_PIN_VERIFY_STATEMENT)
				{
					_opPin = operatorPin;
					IsUnlocked = true;
					return true;
				}
			}

			//Op pin not set, we unlock for operator setup.
			if(!OpPinIsSet() && operatorPin == string.Empty)
			{
				_opPin = operatorPin;
				IsUnlocked = true;
				return true;
			}

			return false;
		}

		public bool UpdateOperatorPin(string newOperatorPin)
		{
			if(!IsUnlocked)
			{
				return false;
			}

			if(string.IsNullOrEmpty(newOperatorPin))
			{
				return false;
			}
			
			for(var i = 0; i < MAX_SLOT_COUNT; i++)
			{
				var walletSlot = GetWalletSlot(i);

				walletSlot.UpdateOperatorPin(newOperatorPin);
			}

			File.Delete(OP_PIN_VERIFY_FILE);
			File.WriteAllText(OP_PIN_VERIFY_FILE, Crypto.Encrypt(OP_PIN_VERIFY_STATEMENT, newOperatorPin));

			_opPin = newOperatorPin;

			return true;
		}

		private bool OpPinIsSet()
		{
			return File.Exists(OP_PIN_VERIFY_FILE);
		}
	}

	//TODO: Update Slot Pin, create slot, delete slot, SetUTXOs, SendDogecoin
	internal class WalletSlot : IWalletSlot
	{
		//address file
		//utxo file
		//key file

		private string SlotAddressFile
		{
			get
			{
				return $"slot_{SlotNumber}_address.dtf";
			}
		}

		private string UTXOFile
		{
			get
			{
				return $"slot_{SlotNumber}_utxos.dtf";
			}
		}
		private string KeyFile
		{
			get
			{
				return $"slot_{SlotNumber}_key.dtf";
			}
		}


		private Game _game;
		private string _opPin;
		private string _slotPin;

		public WalletSlot(Game game, string opPin, int slotNumber)
		{
			_game = game;
			_opPin = opPin;
			_slotPin = string.Empty;
			SlotNumber = slotNumber;
			UTXOs = new List<UTXOInfo>();
		}

		public bool IsEmpty
		{
			get
			{
				return !File.Exists(SlotAddressFile);
			}
		}

		public bool IsUnlocked { get; private set; }

		public int SlotNumber { get; private set; }

		public string Address
		{
			get
			{
				if (!IsEmpty && _opPin != string.Empty)
				{
					return Crypto.Decrypt(File.ReadAllText(SlotAddressFile), _opPin);
				}
				return string.Empty;
			}
		}

		public string SlotPin
		{
			get
			{
				return _slotPin;
			}
		}

		public IEnumerable<UTXOInfo> UTXOs { get; private set; }


		public void Init(string slotPin)
		{
			ClearSlot();

			_slotPin = slotPin;

			//create mnemonic
			using (var ctx = LibDogecoinContext.CreateContext())
			{
				var newMnemonic = ctx.GenerateRandomEnglishMnemonic(LibDogecoinContext.ENTROPY_SIZE_256);

				var keys = ctx.GenerateHDMasterPubKeypairFromMnemonic(newMnemonic);
				
				if(ctx.VerifyHDMasterPubKeyPair(keys.privateKey, keys.publicKey))
				{
					File.WriteAllText(KeyFile, Crypto.Encrypt(Crypto.Encrypt(newMnemonic, _slotPin), _opPin));
					File.WriteAllText(SlotAddressFile, Crypto.Encrypt(keys.publicKey, _opPin));
				}
			}

			//new wallet slots are initialized unlocked.
			Unlock(slotPin);
		}


		public string CalculatetBalance()
		{
			var sum = 0M;

			foreach(var  utxo in UTXOs)
			{
				sum += utxo.Amount;
			}

			return sum.ToString("#,##0.00");
		}


		public bool Unlock(string slotPin)
		{
			if(IsEmpty)
			{
				return false;
			}

			if(File.Exists(UTXOFile))
			{
				try
				{
					var utxoList = new List<UTXOInfo>();
					var utxoFileContent = File.ReadAllText(UTXOFile);

					if (string.IsNullOrEmpty(utxoFileContent))
					{
						var lines = utxoFileContent.Split('\n');

						foreach (var line in lines)
						{
							if (string.IsNullOrEmpty(line))
							{
								continue;
							}

							var lineParts = line.Split('|');

							utxoList.Add(new UTXOInfo
							{
								TransactionId = lineParts[0],
								VOut = int.Parse(lineParts[1]),
								Amount = decimal.Parse(lineParts[2])
							});
						}
					}

					UTXOs = utxoList;
				}
				catch
				{
					//UTXO Parse error, most likely our slot pin is incorrect.
					return false;
				}
			}

			if(!File.Exists(KeyFile))
			{
				//key file is mandatory
				return false;
			}


			try
			{
				var mnemonic = Crypto.Decrypt(Crypto.Decrypt(File.ReadAllText(KeyFile), _opPin), slotPin);

				if(mnemonic.Split(' ').Length == 24)
				{
					_slotPin = slotPin;
					IsUnlocked = true;
					return true;
				}
			}
			catch
			{
				//error parsing key file, most likely incorrect pin
				return false;
			}

			return false;
		}

		public void Lock()
		{
			IsUnlocked = false;
			_slotPin = string.Empty;
		}

		public void UpdateOperatorPin(string newOperatorPin)
		{
			if(IsEmpty)
			{
				return;
			}

			//update address file
			var address = Address;

			File.WriteAllText(SlotAddressFile, Crypto.Encrypt(address, newOperatorPin));

			if(File.Exists(UTXOFile))
			{
				//update utxo file
				var utxoContent = Crypto.Decrypt(File.ReadAllText(UTXOFile), _opPin);
				File.WriteAllText(UTXOFile, Crypto.Encrypt(utxoContent, newOperatorPin));
			}

			//update key file
			var keyContent = Crypto.Decrypt(File.ReadAllText(KeyFile), _opPin);
			File.WriteAllText(KeyFile, Crypto.Encrypt(keyContent, newOperatorPin));


			_opPin = newOperatorPin;
		}


		public void UpdateSlotPin(string newSlotPin)
		{
			if (!IsUnlocked)
			{
				return;
			}

			if (File.Exists(UTXOFile))
			{
				//update utxo file
				var utxoContent = Crypto.Decrypt(Crypto.Decrypt(File.ReadAllText(UTXOFile), _opPin), _slotPin);
				File.WriteAllText(UTXOFile, Crypto.Encrypt(Crypto.Encrypt(utxoContent, newSlotPin), _opPin));
			}

			//update key file
			var keyContent = Crypto.Decrypt(Crypto.Decrypt(File.ReadAllText(KeyFile), _opPin), _slotPin);
			File.WriteAllText(KeyFile, Crypto.Encrypt(Crypto.Encrypt(keyContent, newSlotPin), _opPin));

			_slotPin = newSlotPin;
		}


		public void ClearSlot()
		{
			File.Delete(SlotAddressFile);
			File.Delete(UTXOFile);
			File.Delete(KeyFile);

			_slotPin = string.Empty;
		}

		public string CreateTransaction(string receipient, decimal amount)
		{
			using(var ctx = LibDogecoinContext.CreateContext())
			{
				var workingTransactionId = ctx.StartTransaction();

				try
				{
					var utxoEnumerator = UTXOs.GetEnumerator();

					decimal fee = 0.01M; //fee per utxo
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

						ctx.AddUTXO(workingTransactionId, utxo.TransactionId, utxo.VOut);

					} while (sum < amount + (fee * utxoCount));

					if (sum < amount + (fee * utxoCount))
					{
						throw new Exception("Not enough dogecoin to send.");
					}

					var totalFee = (fee * utxoCount);

					var remainder = sum - totalFee - amount;

					ctx.AddOutput(workingTransactionId, receipient, amount.ToString());
					ctx.AddOutput(workingTransactionId, Address, remainder.ToString());

					var privateKey = GetPrivateKey();

					for (var i = 0; i < utxoCount; i++)
					{
						ctx.SignTransactionWithPrivateKey(workingTransactionId, i, privateKey);
					}

					return ctx.GetRawTransaction(workingTransactionId);

				}
				finally
				{
					ctx.ClearTransaction(workingTransactionId);
				}
			}
		}


		public void UpdateUTXOs(IEnumerable<UTXOInfo> utxos)
		{
			UTXOs = utxos;
		}


		private string GetPrivateKey()
		{
			try
			{
				var mnemonic = GetMnemonic();

				using (var ctx = LibDogecoinContext.CreateContext())
				{
					var keys = ctx.GenerateHDMasterPubKeypairFromMnemonic(mnemonic);

					return keys.privateKey;
				}
			}
			catch
			{
				return string.Empty;
			}
		}

		public string GetMnemonic()
		{
			if (IsUnlocked)
			{
				return Crypto.Decrypt(Crypto.Decrypt(File.ReadAllText(KeyFile), _opPin), _slotPin);
			}
			return string.Empty;
		}
	}


	internal class Crypto
	{

		public static string Encrypt(string plainText, string key)
		{
			using (Aes aesAlg = Aes.Create())
			{
				byte[] keyBytes = Encoding.ASCII.GetBytes(Sha256Hash(key));
				byte[] aesKey = SHA256Managed.Create().ComputeHash(keyBytes);
				byte[] aesIV = MD5.Create().ComputeHash(keyBytes);
				aesAlg.Key = aesKey;
				aesAlg.IV = aesIV;

				ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

				using (MemoryStream msEncrypt = new MemoryStream())
				{
					using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
					{
						using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
						{
							swEncrypt.Write(plainText);
						}
					}

					return Convert.ToBase64String(msEncrypt.ToArray());
				}
			}
		}

		public static string Decrypt(string cipherText, string key)
		{
			using (Aes aesAlg = Aes.Create())
			{
				byte[] keyBytes = Encoding.ASCII.GetBytes(Sha256Hash(key));
				byte[] aesKey = SHA256Managed.Create().ComputeHash(keyBytes);
				byte[] aesIV = MD5.Create().ComputeHash(keyBytes);
				aesAlg.Key = aesKey;
				aesAlg.IV = aesIV;

				ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

				using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
				{
					using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
					{
						using (StreamReader srDecrypt = new StreamReader(csDecrypt))
						{
							return srDecrypt.ReadToEnd();
						}
					}
				}
			}
		}



		public static string Sha256Hash(string value)
		{
			byte[] data = Encoding.ASCII.GetBytes(value);
			data = new SHA256Managed().ComputeHash(data);
			String hash = Encoding.ASCII.GetString(data);
			return hash;
		}

	}
}
