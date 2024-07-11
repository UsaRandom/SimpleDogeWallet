using DogecoinTerminal.Common;
using Lib.Dogecoin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal
{
	public class SimpleDogeWallet
	{
		public const decimal DEFAULT_DUST_LIMIT = 0.001M;
		public const decimal DEFAULT_FEE_COEFF = 1.618M;

		
		public const int MIN_PIN_LENGTH = 4;
		public const string ADDRESS_FILE = "address";
		public const string LOADED_MNEMONIC_FILE = "loadedmnemonic";
		public const string UTXO_FILE = "utxos";

		public const string USING_USER_ENTERED_MNEMONIC_SETTING = "user-entered-mnemonic";

		private bool _usingUserEnteredMnemonic = false;
		private LibDogecoinContext _ctx;
		public int _tpmFileNumber;

		private decimal _pendingAmount;
		private string _pendingHash;


		public SimpleDogeWallet(string address, IServiceProvider services)
		{
			Address = address;
			Services = services;

			_tpmFileNumber = services.GetService<ITerminalSettings>().GetInt("tpm-file-number");

			_pendingHash = services.GetService<ITerminalSettings>().GetString("pending-hash", string.Empty);
			_pendingAmount = services.GetService<ITerminalSettings>().GetDecimal("pending-amount", 0);

			_ctx = LibDogecoinContext.Instance;
			_usingUserEnteredMnemonic = Services.GetService<ITerminalSettings>().GetBool(USING_USER_ENTERED_MNEMONIC_SETTING);

			LoadUTXOs();

			_instance = this;
		}



		public string Address
		{
			get;
			private set;
		}


		public void Save()
		{
			SaveUTXOs();
		}

		private static SimpleDogeWallet _instance;
		public static SimpleDogeWallet Instance
		{
			get
			{
				return _instance;
			}
		}

		public decimal GetBalance()
		{
			if(UTXOs.Count == 0)
			{
				return 0;
			}
			return UTXOs.Sum(utxo => utxo.Amount);
		}

		public List<UTXO> UTXOs
		{
			get;
			private set;
		}

		private IServiceProvider Services
		{
			get;
			set;
		}


		public decimal PendingAmount
		{
			get
			{
				return _pendingAmount;
			}
			set
			{
				_pendingAmount = value;
				Services.GetService<ITerminalSettings>().Set("pending-amount", _pendingAmount);
			}
		}

		public string PendingTxHash
		{
			get
			{
				return _pendingHash;
			}
			set
			{
				_pendingHash = value;
				Services.GetService<ITerminalSettings>().Set("pending-hash", _pendingHash);
			}
		}




		public string GetMnemonic()
		{
			if(Services.GetService<ITerminalSettings>().GetBool(USING_USER_ENTERED_MNEMONIC_SETTING, false))
			{
				var key = _ctx.DecryptMnemonicWithTPM(_tpmFileNumber);

				return Crypto.Decrypt(File.ReadAllText(LOADED_MNEMONIC_FILE), key);
			}
			else
			{
				return _ctx.DecryptMnemonicWithTPM(_tpmFileNumber);
			}
		}

		public static void UpdatePin(string oldPin, string newPin)
		{
			var address = Crypto.Decrypt(File.ReadAllText(ADDRESS_FILE), oldPin);

			File.WriteAllText(ADDRESS_FILE, Crypto.Encrypt(address, newPin));
		}

		public static bool TryOpen(string pin, IServiceProvider services, out SimpleDogeWallet simpleDogeWallet)
		{
			simpleDogeWallet = null;
			try
			{
				var address = Crypto.Decrypt(File.ReadAllText(ADDRESS_FILE), pin);

				if(!LibDogecoinContext.Instance.VerifyP2pkhAddress(address))
				{
					return false;
				}


				if(Instance == null)
				{
					simpleDogeWallet = new SimpleDogeWallet(address, services);
				}
				else
				{
					simpleDogeWallet = Instance;
				}

				return true;
			}
			catch (Exception ex)
			{
				
			}
			return false;
		}



		//TODO: Remove duplicate code.

		private void LoadUTXOs()
		{
			UTXOs = new List<UTXO>();

			if (File.Exists(UTXO_FILE))
			{
				var utxoString = File.ReadAllText(UTXO_FILE);

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

						UTXOs.Add(new UTXO
						{
							TxId = lineParts[0],
							VOut = int.Parse(lineParts[1]),
							AmountKoinu = long.Parse(lineParts[2])
						});
					}
				}
			}


			
		}

		private void SaveUTXOs()
		{
			var utxoContent = new StringBuilder();

			foreach (var utxo in UTXOs)
			{
				utxoContent.Append(utxo.TxId + "|");
				utxoContent.Append(utxo.VOut + "|");
				utxoContent.AppendLine(utxo.AmountKoinu?.ToString());
			}

			File.WriteAllText(UTXO_FILE, utxoContent.ToString());

		}


		public static void ClearWallet()
		{
			File.Delete(ADDRESS_FILE);
			File.Delete(LOADED_MNEMONIC_FILE);
			File.Delete(UTXO_FILE);
		}



	}
}
