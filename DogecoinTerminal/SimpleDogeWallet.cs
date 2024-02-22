﻿using DogecoinTerminal.Common;
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
		public const decimal DEFAULT_FEE_PER_UTXO = 0.002M;


		public const int MIN_PIN_LENGTH = 4;
		public const int TPM_FILE_NUMBER = 420; //nice
		public const string ADDRESS_FILE = "address";
		public const string LOADED_MNEMONIC_FILE = "loadedmnemonic";
		public const string UTXO_FILE = "utxos";

		public const string USING_USER_ENTERED_MNEMONIC_SETTING = "user-entered-mnemonic";

		private bool _usingUserEnteredMnemonic = false;
		private LibDogecoinContext _ctx;

		public SimpleDogeWallet(string address, IServiceProvider services)
		{
			Address = address;
			Services = services;


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

		public string GetMnemonic()
		{
			if(Services.GetService<ITerminalSettings>().GetBool(USING_USER_ENTERED_MNEMONIC_SETTING, false))
			{
				var key = _ctx.DecryptMnemonicWithTPM(TPM_FILE_NUMBER);

				return Crypto.Decrypt(File.ReadAllText(LOADED_MNEMONIC_FILE), key);
			}
			else
			{
				return _ctx.DecryptMnemonicWithTPM(TPM_FILE_NUMBER);
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

				simpleDogeWallet = new SimpleDogeWallet(address, services);

				
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
