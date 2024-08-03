using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SimpleDogeWallet.Common
{
	public class Crypto
    {
        public const string HDPATH = "m/44'/3'/0'/0/0";

        public static string Encrypt(string plainText, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                byte[] keyBytes = Encoding.ASCII.GetBytes(Sha256Hash(key));
                byte[] aesKey = SHA256.Create().ComputeHash(keyBytes);
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
                byte[] aesKey = SHA256.Create().ComputeHash(keyBytes);
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
                            try
                            {

								return srDecrypt.ReadToEnd();
							}
                            catch (Exception ex)
                            {
                                return null;
                            }
                        }
                    }
                }
            }
        }


        public static string GetTransactionIdFromRaw(string rawTransaction)
        {
            byte[] transactionBytes = HexStringToByteArray(rawTransaction);

            byte[] hash1, hash2;
            using (SHA256 sha256 = SHA256.Create())
            {
                hash1 = sha256.ComputeHash(transactionBytes);
                hash2 = sha256.ComputeHash(hash1);
            }

            Array.Reverse(hash2);

            return BitConverter.ToString(hash2).Replace("-", "").ToLower();
        }


        public static byte[] HexStringToByteArray(string hex)
        {
            int length = hex.Length / 2;
            byte[] byteArray = new byte[length];
            for (int i = 0; i < length; i++)
            {
                byteArray[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return byteArray;
        }

        public static string Sha256Hash(string value)
        {
            byte[] data = Encoding.ASCII.GetBytes(value);
            data = new SHA256Managed().ComputeHash(data);
            string hash = Encoding.ASCII.GetString(data);
            return hash;
        }
		public static bool VerifyP2PKHAddress(string address)
		{
			if (address.Length != 34 || !address.StartsWith("D"))
			{
				return false;
			}

			byte[] decodedAddress = DecodeBase58(address.Substring(1));
			byte[] checksum = new byte[4];
			Array.Copy(decodedAddress, decodedAddress.Length - 4, checksum, 0, 4);

			byte[] expectedChecksum = Sha256(Sha256(decodedAddress.Take(decodedAddress.Length - 4).ToArray()));
			Array.Copy(expectedChecksum, 0, checksum, 0, 4);

			return checksum.SequenceEqual(expectedChecksum.Take(4));
		}

		private static byte[] DecodeBase58(string input)
		{
			const string base58chars = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
			int zeroCount = 0;
			long result = 0;

			for (int i = 0; i < input.Length; i++)
			{
				int digit = base58chars.IndexOf(input[i]);
				if (digit < 0)
				{
					throw new ArgumentException("Invalid Base58 character", nameof(input));
				}

				result = result * 58 + digit;

				if (input[i] == '1')
				{
					zeroCount++;
				}
				else
				{
					break;
				}
			}

			byte[] output = new byte[input.Length - zeroCount];
			for (int i = zeroCount; i < input.Length; i++)
			{
				int digit = base58chars.IndexOf(input[i]);
				output[i - zeroCount] = (byte)(result % 256);
				result /= 256;
			}

			Array.Reverse(output);
			return output;
		}

		private static byte[] Sha256(byte[] data)
		{
			using (var sha256 = System.Security.Cryptography.SHA256.Create())
			{
				return sha256.ComputeHash(data);
			}
		}

	}
}
