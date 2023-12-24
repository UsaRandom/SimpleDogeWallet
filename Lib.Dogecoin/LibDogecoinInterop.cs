using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Dogecoin
{
	/*
	 * Note,
	 * 
	 * This is not a complete set of bindings for libdogecoin, but a starting point
	 * for those who want to build ontop of libdogecoin. 
	 * 
	 * Please see the dogecoin foundation's repo for infomration on how to build
	 * and use libdogecoin. 
	 */
	internal static class LibDogecoinInterop
	{
		private const string DLL_NAME = "dogecoin";


		//!init static ecc context
		[DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void dogecoin_ecc_start();

		//!destroys the static ecc context
		[DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void dogecoin_ecc_stop();

		/* generates a private and public keypair (a wallet import format private key and a p2pkh ready-to-use corresponding dogecoin address)*/
		[DllImport(DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int generatePrivPubKeypair(
			[Out, MarshalAs(UnmanagedType.LPArray)] char[] wif_privkey,
			[Out, MarshalAs(UnmanagedType.LPArray)] char[] p2pkh_pubkey,
			[MarshalAs(UnmanagedType.I1)] bool is_testnet
		);

		/* generates a hybrid deterministic WIF master key and p2pkh ready-to-use corresponding dogecoin address. */
		[DllImport(DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int generateHDMasterPubKeypair(
			[Out, MarshalAs(UnmanagedType.LPArray)] char[] wif_privkey_master,
			[Out, MarshalAs(UnmanagedType.LPArray)] char[] p2pkh_pubkey_master,
			[MarshalAs(UnmanagedType.I1)] bool is_testnet
		);

		/* generates a new dogecoin address from a HD master key */
		[DllImport(DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int generateDerivedHDPubkey(
			[MarshalAs(UnmanagedType.LPArray)] char[] wif_privkey_master,
			[Out, MarshalAs(UnmanagedType.LPArray)] char[] p2pkh_pubkey
		);

		/* get derived hd address by custom path */
		[DllImport(DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int getDerivedHDAddressByPath(
			[MarshalAs(UnmanagedType.LPArray)] char[] masterkey,
			[MarshalAs(UnmanagedType.LPArray)] char[] derived_path,
			[Out, MarshalAs(UnmanagedType.LPArray)] char[] outaddress,
			[MarshalAs(UnmanagedType.I1)]bool outprivkey
		);


		/* verify that a private key and dogecoin address match */
		[DllImport(DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int verifyPrivPubKeypair(
			[MarshalAs(UnmanagedType.LPArray)] char[] wif_privkey,
			[MarshalAs(UnmanagedType.LPArray)] char[] p2pkh_pubkey,
			[MarshalAs(UnmanagedType.I1)] bool is_testnet
		);


		/* verify that a HD Master key and a dogecoin address matches */
		[DllImport(DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int verifyHDMasterPubKeypair(
			[MarshalAs(UnmanagedType.LPArray)] char[] wif_privkey_master,
			[MarshalAs(UnmanagedType.LPArray)] char[] p2pkh_pubkey_master,
			[MarshalAs(UnmanagedType.I1)] bool is_testnet
		);

		/* verify that a HD Master key and a dogecoin address matches */
		[DllImport(DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int verifyP2pkhAddress(
			[MarshalAs(UnmanagedType.LPArray)] char[] p2pkh_pubkey,
			uint len
		);

		/* Generates an English mnemonic phrase from given hex entropy */
		[DllImport(DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int generateEnglishMnemonic(
			[MarshalAs(UnmanagedType.LPArray)] char[] entropy,
			[MarshalAs(UnmanagedType.LPArray)] char[] size,
			[Out, MarshalAs(UnmanagedType.LPArray)] char[] mnemonic
		);

		/* Generates a random (e.g. "128" or "256") English mnemonic phrase */
		[DllImport(DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int generateRandomEnglishMnemonic(
			[MarshalAs(UnmanagedType.LPArray)] char[] size,
			[Out, MarshalAs(UnmanagedType.LPArray)] char[] mnemonic
		);


		/* Generates a HD master key and p2pkh ready-to-use corresponding dogecoin address from a mnemonic */
		[DllImport(DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int getDerivedHDAddressFromMnemonic(
			uint account,
			uint index,
			[MarshalAs(UnmanagedType.LPArray)] char[] change_level,
			[MarshalAs(UnmanagedType.LPArray)] char[] mnemonic,
			[MarshalAs(UnmanagedType.LPArray)] char[] pass,
			[Out, MarshalAs(UnmanagedType.LPArray)] char[] p2pkh_pubkey,
			[MarshalAs(UnmanagedType.I1)] bool is_testnet
		);


		/* sign a message with a private key */
		[DllImport(DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr sign_message(
			[MarshalAs(UnmanagedType.LPArray)] char[] privkey,
			[MarshalAs(UnmanagedType.LPArray)] char[] msg
		);


		/* verify a message with a address */
		[DllImport(DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool verify_message(
			[MarshalAs(UnmanagedType.LPArray)] char[] sig,
			[MarshalAs(UnmanagedType.LPArray)] char[] msg,
			[MarshalAs(UnmanagedType.LPArray)] char[] address
		);


		/* create a QR text formatted string (with line breaks) from an incoming p2pkh*/
		[DllImport(DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int qrgen_p2pkh_to_qr_string(
			[MarshalAs(UnmanagedType.LPArray)] char[] in_p2pkh,
			[Out, MarshalAs(UnmanagedType.LPArray)] char[] outString
		);


		/* Creates a .png file with the filename outFilename, from string inString, w. size factor of SizeMultiplier.*/

		[DllImport(DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int qrgen_string_to_qr_pngfile(
			[MarshalAs(UnmanagedType.LPArray)] char[] outFilename,
			[MarshalAs(UnmanagedType.LPArray)] char[] inString,
			byte sizeMultiplier
		);

		/* Creates a .jpg file with the filename outFilename, from string inString, w. size factor of SizeMultiplier.*/
		[DllImport(DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int qrgen_string_to_qr_jpgfile(
			[MarshalAs(UnmanagedType.LPArray)] char[] outFilename,
			[MarshalAs(UnmanagedType.LPArray)] char[] inString,
			byte sizeMultiplier
		);


		/* create a new dogecoin transaction: Returns the (txindex) in memory of the transaction being worked on. */
		[DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int start_transaction();


		/* add a utxo to the transaction being worked on at (txindex), specifying the utxo's txid and vout. returns 1 if successful.*/
		[DllImport(DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int add_utxo(
			int txindex,
			[MarshalAs(UnmanagedType.LPArray)] char[] hex_utxo_txid,
			int vout
		);




		/* add an output to the transaction being worked on at (txindex) of amount (amount) in dogecoins, returns 1 if successful. */
		[DllImport(DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int add_output(
			int txindex,
			[MarshalAs(UnmanagedType.LPArray)] char[] destinationaddress,
			[MarshalAs(UnmanagedType.LPArray)] char[] amount
		);


		/* finalize the transaction being worked on at (txindex), with the (destinationaddress) paying a fee of (subtractedfee), */
		/* re-specify the amount in dogecoin for verification, and change address for change. If not specified, change will go to the first utxo's address. */
		[DllImport(DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr finalize_transaction(
			int txindex,
			[MarshalAs(UnmanagedType.LPArray)] char[] destinationaddress,
			[MarshalAs(UnmanagedType.LPArray)] char[] subtractedfee,
			[Out, MarshalAs(UnmanagedType.LPArray)] char[] out_dogeamount_for_verification,
			[MarshalAs(UnmanagedType.LPArray)] char[] changeaddress
		);


		[DllImport(DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sign_raw_transaction(int inputindex,

			IntPtr incomingrawtx,
			[MarshalAs(UnmanagedType.LPArray)] char[] scripthex,
			int sighashtype,
			[MarshalAs(UnmanagedType.LPArray)] char[] privkey);




		/**
		 * @brief This function generates a HD master key and p2pkh ready-to-use corresponding dogecoin address from a mnemonic.
		 *
		 * @param hd_privkey_master The generated master private key.
		 * @param p2pkh_pubkey_master The generated master public key.
		 * @param mnemonic The mnemonic code words.
		 * @param passphrase The passphrase (optional).
		 * @param is_testnet The flag denoting which network, 0 for mainnet and 1 for testnet.
		 *
		 * return: 0 (success), -1 (fail)
		 */
		[DllImport(DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int generateHDMasterPubKeypairFromMnemonic
			([Out, MarshalAs(UnmanagedType.LPArray)] char[] hd_privkey_master,
			[Out, MarshalAs(UnmanagedType.LPArray)] char[] p2pkh_pubkey_master,
			[MarshalAs(UnmanagedType.LPArray)] char[] mnemonic,
			[MarshalAs(UnmanagedType.LPArray)] char[] pass,
			[MarshalAs(UnmanagedType.I1)] bool is_testnet);



		/* retrieve the raw transaction at (txindex) as a hex string (char*) */
		[DllImport(DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr get_raw_transaction(
			int txindex
		);



		/* sign a raw transaction in memory at (txindex), sign (inputindex) with (scripthex) of (sighashtype), with (privkey) */
		[DllImport(DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sign_transaction(
			int txindex,
			[MarshalAs(UnmanagedType.LPArray)] char[] script_pubkey,
			[MarshalAs(UnmanagedType.LPArray)] char[] privkey
		);

		/* Sign a formed transaction with working transaction index (txindex), prevout.n index (vout_index) and private key (privkey) */
		[DllImport(DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sign_transaction_w_privkey(
			int txindex,
			int vout_index,
			[MarshalAs(UnmanagedType.LPArray)] char[] privkey
		);


		/**
		 * It takes a p2pkh address and converts it to a compressed public key in
		 * hexadecimal format. It then strips the network prefix and checksum
		 */
		[DllImport(DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern byte dogecoin_p2pkh_address_to_pubkey_hash(
			[MarshalAs(UnmanagedType.LPArray)] char[] p2pkh,
			[Out, MarshalAs(UnmanagedType.LPArray)] char[] pubkeyHash
		);

		/* clear all internal working transactions */
		[DllImport(DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void remove_all();

		/* clear the transaction at (txindex) in memory */
		[DllImport(DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void clear_transaction(int txindex);





		/* Koinu functions */

		[DllImport(DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern int koinu_to_coins_str(
			ulong koinu,
			[Out, MarshalAs(UnmanagedType.LPArray)] char[] str
		);



		[DllImport(DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong coins_to_koinu_str(
			[MarshalAs(UnmanagedType.LPArray)] char[] coins
		);


		/* Memory functions */

		[DllImport(DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr dogecoin_char_vla(uint size);

		[DllImport(DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern void dogecoin_free(IntPtr ptr);


	}
}
