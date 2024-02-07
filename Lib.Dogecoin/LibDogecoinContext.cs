using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Lib.Dogecoin.Interop;

namespace Lib.Dogecoin
{
	public class LibDogecoinContext : IDisposable
	{
		private static object _lock = new object();
		private static LibDogecoinContext? _instance;

		internal static IntPtr _mainChain;
		internal static IntPtr _testChain;


		private bool _disposed = false;

		static LibDogecoinContext() {

			_mainChain = LibDogecoinInterop.chain_from_b58_prefix("D".NullTerminate());
			_testChain = LibDogecoinInterop.chain_from_b58_prefix("n".NullTerminate());
		}


		public static LibDogecoinContext Instance
		{
			get
			{
				return _instance ?? CreateContext();
			}
		}

		public static LibDogecoinContext CreateContext()
		{
			lock (_lock)
			{
				if (_instance == null || _instance._disposed)
				{
					_instance = new LibDogecoinContext();
					

					return _instance;
				}

				throw new Exception("Already using a LibDogecoinContext.");
			}
		}


		private LibDogecoinContext()
		{
			LibDogecoinInterop.dogecoin_ecc_start();
		}


		public (string privateKey, string publicKey) GeneratePrivPubKeyPair(bool testNet = false)
		{
			lock (_lock)
			{
				var privKey = new char[52];
				var pubKey = new char[34];

				LibDogecoinInterop.generatePrivPubKeypair(privKey, pubKey, testNet);

				return (privKey.TerminateNull(), pubKey.TerminateNull());
			}
		}

		public (string privateKey, string publicKey) GenerateHDMasterPubKeyPair(bool testNet = false)
		{
			lock (_lock)
			{
				var privKey = new char[111];
				var pubKey = new char[34];

				LibDogecoinInterop.generateHDMasterPubKeypair(privKey, pubKey, testNet);

				return (privKey.TerminateNull(), pubKey.TerminateNull());
			}
		}


		public string GenerateDerivedHDPubKey(string masterPrivKey)
		{
			lock (_lock)
			{
				var pubKey = new char[34];

				LibDogecoinInterop.generateDerivedHDPubkey(masterPrivKey.NullTerminate(), pubKey);

				return pubKey.TerminateNull();
			}
		}

		public string GetDerivedHDAddressByPath(string masterPrivKey, string derivedPath, bool isPriv)
		{
			lock (_lock)
			{
				var key = new char[111];

				LibDogecoinInterop.getDerivedHDAddressByPath(
					masterPrivKey.NullTerminate(),
					derivedPath.NullTerminate(),
					key,
					isPriv);

				return key.TerminateNull();
			}
		}

		public string GetHDNodePrivateKeyWIFByPath(string master, string path, bool priv)
		{
			lock (_lock)
			{
				var key = new char[111];

				var result = LibDogecoinInterop.getHDNodePrivateKeyWIFByPath(
					master.NullTerminate(),
					path.NullTerminate(),
					key,
					priv);

				return Marshal.PtrToStringAnsi(result);
			}
		}


		public bool VerifyPrivPubKeyPair(string privKey, string pubKey, bool testNet = false)
		{
			lock (_lock)
			{
				return 0 != LibDogecoinInterop.verifyPrivPubKeypair(privKey.NullTerminate(), pubKey.NullTerminate(), testNet);
			}
		}

		public bool VerifyHDMasterPubKeyPair(string privKey, string pubKey, bool testNet = false)
		{
			lock (_lock)
			{
				return 0 != LibDogecoinInterop.verifyHDMasterPubKeypair(privKey.NullTerminate(), pubKey.NullTerminate(), testNet);
			}
		}

		public bool VerifyP2pkhAddress(string pubKey)
		{
			lock (_lock)
			{
				return 0 != LibDogecoinInterop.verifyP2pkhAddress(pubKey.NullTerminate(), (uint)pubKey.Length);
			}
		}


		public string GenerateEnglishMnemonic(string entropy, string entropySize)
		{
			lock (_lock)
			{
				var mnemonic = new char[1024];

				LibDogecoinInterop.generateEnglishMnemonic(entropy.NullTerminate(), entropySize.NullTerminate(), mnemonic);

				return mnemonic.TerminateNull();
			}
		}


		public string GenerateRandomEnglishMnemonic(string entropySize)
		{
			lock (_lock)
			{
				var mnemonic = new char[1024];

				LibDogecoinInterop.generateRandomEnglishMnemonic(entropySize.NullTerminate(), mnemonic);

				return mnemonic.TerminateNull();
			}
		}


		public string GetDerivedHDAddressFromMnemonic(int account, int index, string changeLevel, string mnemonic, string password, bool testNet)
		{
			lock (_lock)
			{
				var address = new char[1024];

				LibDogecoinInterop.getDerivedHDAddressFromMnemonic(
					(uint)account,
					(uint)index,
					changeLevel.NullTerminate(),
					mnemonic.NullTerminate(),
					password.NullTerminate(),
					address,
					testNet);

				return address.TerminateNull();
			}
		}




		public string P2pkhToQrString(string p2pkh)
		{
			lock (_lock)
			{
				var qrString = new char[3918 * 4];

				LibDogecoinInterop.qrgen_p2pkh_to_qr_string(p2pkh.NullTerminate(), qrString);

				return qrString.TerminateNull();
			}
		}


		public bool StringToQrPng(string qrString, string file, byte sizeMultiplier = 100)
		{
			lock (_lock)
			{
				return -1 != LibDogecoinInterop.qrgen_string_to_qr_pngfile(file.NullTerminate(), qrString.NullTerminate(), sizeMultiplier);
			}
		}

		public bool StringToQrJpg(string qrString, string file, byte sizeMultiplier = 100)
		{
			lock (_lock)
			{
				return -1 != LibDogecoinInterop.qrgen_string_to_qr_jpgfile(file.NullTerminate(), qrString.NullTerminate(), sizeMultiplier);
			}
		}




		public int StartTransaction()
		{
			lock (_lock)
			{
				return LibDogecoinInterop.start_transaction();
			}
		}


		public bool AddUTXO(int txIndex, string hexUTXOTxId, int vOut)
		{
			lock (_lock)
			{
				return 0 != LibDogecoinInterop.add_utxo(txIndex, hexUTXOTxId.NullTerminate(), vOut);
			}
		}


		public bool AddOutput(int txIndex, string destinationAddress, string amount)
		{
			lock (_lock)
			{
				return 0 != LibDogecoinInterop.add_output(txIndex, destinationAddress.NullTerminate(), amount.NullTerminate());
			}
		}


		public string FinalizeTransaction(int txIndex, string destinationAddress, string fee, string outputSum, string changeAddress)
		{
			lock (_lock)
			{
				return Marshal.PtrToStringAnsi(LibDogecoinInterop.finalize_transaction(
										txIndex,
										destinationAddress.NullTerminate(),
										fee.NullTerminate(),
										outputSum.NullTerminate(),
										changeAddress.NullTerminate()));


			}
		}


		public string SignRawTransaction(int inputindex, string incomingrawtx, string scripthex, int sighashtype, string privkey)
		{
			lock (_lock)
			{
				IntPtr result = Marshal.StringToHGlobalAnsi(incomingrawtx);
				//	var result = incomingrawtx.NullTerminate();

				LibDogecoinInterop.sign_raw_transaction(
										inputindex,
										result,
										scripthex.NullTerminate(),
										sighashtype,
										privkey.NullTerminate());

				return Marshal.PtrToStringAnsi(result);
			}
		}


		public (string privateKey, string publicKey) GenerateHDMasterPubKeypairFromMnemonic(string mnemonic, string pass = null, bool isTest = false)
		{
			lock (_lock)
			{
				var privKey = new char[255];
				var pubKey = new char[255];

				LibDogecoinInterop.generateHDMasterPubKeypairFromMnemonic(privKey, pubKey, mnemonic.NullTerminate(), null, isTest);

				return (privKey.TerminateNull(), pubKey.TerminateNull());
			}
		}



		public string AddressToPubKeyHash(string address)
		{
			lock (_lock)
			{
				var pubkeyhash = new char[50];

				var result = LibDogecoinInterop.dogecoin_p2pkh_address_to_pubkey_hash(address.NullTerminate(), pubkeyhash);

				return pubkeyhash.TerminateNull();
			}
		}

		public string GetRawTransaction(int txIndex)
		{
			lock (_lock)
			{
				IntPtr returnValue = LibDogecoinInterop.get_raw_transaction(txIndex);

				return Marshal.PtrToStringAnsi(returnValue);
			}
		}


		public bool SignTransaction(int txIndex, string scriptPubKey, string privKey)
		{
			lock (_lock)
			{
				return 0 != LibDogecoinInterop.sign_transaction(txIndex, scriptPubKey.NullTerminate(), privKey.NullTerminate());
			}
		}

		public bool SignTransactionWithPrivateKey(int txIndex, int vOutIndex, string privKey)
		{
			lock (_lock)
			{
				return 0 != LibDogecoinInterop.sign_transaction_w_privkey(txIndex, vOutIndex, privKey.NullTerminate());
			}
		}




		public void ClearTransaction(int txIndex)
		{
			lock (_lock)
			{
				LibDogecoinInterop.clear_transaction(txIndex);
			}
		}


		public void RemoveAllTransactions()
		{
			lock (_lock)
			{
				LibDogecoinInterop.remove_all();
			}
		}


		public string SignMessage(string privateKey, string message)
		{
			lock (_lock)
			{
				//sign_message can return a char[]* or false
				//so we accept a pointer, try to cast to string and fallback on string.empty 
				var value = LibDogecoinInterop.sign_message(privateKey.NullTerminate(), message.NullTerminate());

				return Marshal.PtrToStringAnsi(value) ?? string.Empty;
			}
		}

		public bool VerifyMessage(string signature, string message, string address)
		{
			lock (_lock)
			{
				return LibDogecoinInterop.verify_message(signature.NullTerminate(), message.NullTerminate(), address.NullTerminate());
			}
		}



		public string GenerateMnemonicEncryptWithTPM(int fileNumber, bool overwrite = true, string lang = "eng", string space = " ")
		{
			lock(_lock)
			{
				var mnemonic = new char[2048];

				LibDogecoinInterop.dogecoin_generate_mnemonic_encrypt_with_tpm(mnemonic, fileNumber, overwrite, lang.NullTerminate(), space.NullTerminate(), null);


				return mnemonic.TerminateNull();
			}
		}



		public string DecryptMnemonicWithTPM(int fileNumber)
		{
			lock (_lock)
			{
				var mnemonic = new char[2048];

				LibDogecoinInterop.dogecoin_decrypt_mnemonic_with_tpm(mnemonic, fileNumber);


				return mnemonic.TerminateNull();
			}
		}


		public string[] ListKeysInTPM()
		{
			//TODO: This can be simplified by just looking at the ./source/ folder where they are stored.
			lock (_lock)
			{
				var keyNames = new List<string>();
				int count;
				IntPtr countPtr;
				bool result;

				count = 1000;

				IntPtr[] keyNamePointers = new IntPtr[count];

				result = LibDogecoinInterop.dogecoin_list_encryption_keys_in_tpm(Marshal.UnsafeAddrOfPinnedArrayElement(keyNamePointers, 0), out countPtr);

				// Check the result
				if (result)
				{
					// Retrieve the key names
					for (int i = 0; i < count; i++)
					{
						if (keyNamePointers[i] == IntPtr.Zero)
						{
							break;
						}
						else
						{
							keyNames.Add(Marshal.PtrToStringUni(keyNamePointers[i]));

							LibDogecoinInterop.dogecoin_free(keyNamePointers[i]);
						}
					}

				}

				Marshal.FreeCoTaskMem(countPtr);

				return keyNames.ToArray();
			}
		}



		public string KoinuToCoinString(ulong amount)
		{
			lock (_lock)
			{
				var coinStr = new char[21];

				LibDogecoinInterop.koinu_to_coins_str(amount, coinStr);

				return coinStr.TerminateNull();
			}
		}

		public ulong CoinStringToKoinu(string coinStr)
		{
			lock (_lock)
			{
				return LibDogecoinInterop.coins_to_koinu_str(coinStr.NullTerminate());
			}
		}

		public bool BroadcastTransaction(string rawTransaction, bool isMainNet = true)
		{
			if(string.IsNullOrEmpty(rawTransaction) || rawTransaction.Length > 0x003D0900)
			{
				return false;
			}
			return true;
			///* The above code is checking if the data is NULL, empty or larger than the maximum
				//size of a p2p message. */
				//if (data == NULL || strlen(data) == 0 || strlen(data) > DOGECOIN_MAX_P2P_MSG_SIZE)
				//{
				//	return showError("Transaction in invalid or to large.\n");
				//}
				//uint8_t* data_bin = dogecoin_malloc(strlen(data) / 2 + 1);
				//size_t outlen = 0;
				//utils_hex_to_bin(data, data_bin, strlen(data), &outlen);

				//dogecoin_tx* tx = dogecoin_tx_new();
				///* Deserializing the transaction and broadcasting it to the network. */
				//if (dogecoin_tx_deserialize(data_bin, outlen, tx, NULL))
				//{
				//	broadcast_tx(chain, tx, ips, maxnodes, timeout, debug);
				//}
				//else
				//{
				//	showError("Transaction is invalid\n");
				//	ret = 1;
				//}
				//dogecoin_free(data_bin);
				//dogecoin_tx_free(tx);


		}



		public bool BroadcastRawTransaction(string rawTransaction, bool isMainNet = true)
		{
			lock (_lock)
			{
				return LibDogecoinInterop.broadcast_raw_tx(_mainChain, rawTransaction.NullTerminate());
			}
		}

		public void Dispose()
		{
			lock (_lock)
			{
				_disposed = true;
				LibDogecoinInterop.dogecoin_ecc_stop();
			}

		}


		public const string ENTROPY_SIZE_128 = "128";
		public const string ENTROPY_SIZE_256 = "256";
	}
}