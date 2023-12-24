using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace DogecoinTerminal
{
	internal class TerminalService
	{
		public const int MAX_WALLET_SLOTS = 6;
		internal const string VERIFICATION_MSG = "much verify, wow";
		private const string OPVERIFY_DTF = "opverify.dtf";

		private WalletSlot[] _slots;


		private string _opPin = string.Empty;

		public TerminalService()
		{
			_slots = new WalletSlot[MAX_WALLET_SLOTS];
		}


		public bool Unlock(string operatorPin)
		{
			if (CheckOperatorPin(operatorPin))
			{
				_opPin = operatorPin;
				return true;
			}

			return false;
		}

		public void Lock()
		{
			_opPin = string.Empty;
		}


		public bool UpdateOperatorPin(string oldPin, string newPin)
		{
			if(CheckOperatorPin(oldPin))
			{
				File.Delete(OPVERIFY_DTF);
				File.WriteAllText(OPVERIFY_DTF, CryptoTools.Encrypt(OPVERIFY_DTF, newPin));

				foreach(var slot in _slots)
				{
					slot.UpdateOperatorPin(oldPin, newPin);
				}

				return true;
			}

			return false;
		}

		public WalletSlot GetSlot(int slot)
		{
			return _slots[slot];
		}


		public bool CheckOperatorPin(string pin)
		{
			//dogecoin terminal file, or .dtf
			if(!File.Exists(OPVERIFY_DTF))
			{
				// no opverify.dtf means we need to set our operator pin
				// so any pin provided is 'correct'.
				return true;
			}

			return CryptoTools.Decrypt(File.ReadAllText(OPVERIFY_DTF), pin) == VERIFICATION_MSG;
		}
		
	}

	//TODO: Update Slot Pin, create slot, delete slot, SetUTXOs, SendDogecoin
	internal class WalletSlot
	{

		public WalletSlot(int number)
		{
			SlotNumber = number;
			_slotPin = string.Empty;
		}

		private string _slotPin;

		public int SlotNumber { get; set; }

		public string Address { get; set; }

		public string KeyFile
		{
			get
			{
				return "slot_" + SlotNumber + ".dtf";
			}
		}


		public string VerifyFile
		{
			get
			{
				return "slot_" + SlotNumber + "_verify.dtf";
			}
		}


		public bool IsEmpty
		{
			get
			{
				return string.IsNullOrEmpty(Address);
			}
		}

		public bool Unlock(string operatorPin, string slotPin)
		{
			if(CheckSlotPin(operatorPin, slotPin))
			{
				_slotPin = slotPin;
				return true;
			}
			return false;
		}

		
		public void Lock()
		{
			_slotPin = string.Empty;
		}


		private bool CheckSlotPin(string operatorPin, string slotPin)
		{
			//dogecoin terminal file, or .dtf
			if (!File.Exists(VerifyFile))
			{
				// no opverify.dtf means we need to set our operator pin
				// so any pin provided is 'correct'.
				return true;
			}

			return CryptoTools.Decrypt(CryptoTools.Decrypt(File.ReadAllText(VerifyFile), operatorPin), slotPin) == TerminalService.VERIFICATION_MSG;
		}

		public void UpdateOperatorPin(string oldPin, string newPin)
		{
			if (!File.Exists(KeyFile))
			{
				return;
			}

			var oldFile = CryptoTools.Decrypt(File.ReadAllText(KeyFile), oldPin);

			File.Delete(KeyFile);

			File.WriteAllText(KeyFile, CryptoTools.Encrypt(oldFile, newPin));
		}

		private string GetMnemonic(string operatorPin, string slotPin)
		{
			
			if (!File.Exists(KeyFile))
			{
				return string.Empty;
			}

			return CryptoTools.Decrypt(CryptoTools.Decrypt(File.ReadAllText(KeyFile), operatorPin), slotPin);
		}
	}


	internal class CryptoTools
	{

		public static string Encrypt(string plainText, string key)
		{
			using (Aes aesAlg = Aes.Create())
			{
				aesAlg.Key = Encoding.UTF8.GetBytes(key);
				aesAlg.IV = GenerateRandomIV();

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
				aesAlg.Key = Encoding.UTF8.GetBytes(key);
				aesAlg.IV = GenerateRandomIV();

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


		static byte[] GenerateRandomIV()
		{
			using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
			{
				byte[] iv = new byte[16]; // 16 bytes for AES
				rng.GetBytes(iv);
				return iv;
			}
		}
	}
}
