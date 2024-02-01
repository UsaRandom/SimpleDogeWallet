
using Lib.Dogecoin.Interop;
using System.Runtime.InteropServices;

namespace Lib.Dogecoin
{
	public static class PublicExtensions
	{

		static PublicExtensions()
		{
			freeDelegate = LibDogecoinInterop.dogecoin_free;
		}


		private delegate void dogecoin_free_delegate(IntPtr target);
		private static dogecoin_free_delegate freeDelegate;

		public static string GetP2PKHAddress(this string scriptPubKey)
		{
			return UnsafeGetP2PKHAddress(scriptPubKey);
		}

		private static unsafe string UnsafeGetP2PKHAddress(string scriptPubKey)
		{
			char[] address = new char[35];

			var cStr = LibDogecoinInterop.cstr_new(scriptPubKey.NullTerminate());

			IntPtr freePtr = Marshal.GetFunctionPointerForDelegate(freeDelegate);
			var partsPtr = LibDogecoinInterop.vector_new(16, freePtr);

			var type = LibDogecoinInterop.dogecoin_script_classify(cStr, partsPtr);

			if (type == dogecoin_tx_out_type.DOGECOIN_TX_PUBKEYHASH)
			{
				byte[] hash = new byte[20];
				Marshal.Copy((*partsPtr).data[0], hash, 0, 20);

				LibDogecoinInterop.dogecoin_p2pkh_addr_from_hash160(hash, LibDogecoinContext._mainChain, address, 35);
			}

			LibDogecoinInterop.vector_free(partsPtr, true);
			LibDogecoinInterop.cstr_free(cStr, 1);

			return address.TerminateNull();
		}

	}

	internal static class Extensions
	{

		public static char[] NullTerminate(this string str)
		{
			var result = new char[str.Length + 1];
			str.CopyTo(0, result, 0, str.Length);
			result[str.Length] = '\0';
			return result;
		}

		public static string TerminateNull(this char[] chars)
		{
			int nullIndex = Array.IndexOf(chars, '\0');

			nullIndex = (nullIndex == -1) ? chars.Length : nullIndex;
			return new string(chars, 0, nullIndex);
		}

	}
}
