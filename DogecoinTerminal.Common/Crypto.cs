using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DogecoinTerminal.Common
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
                            return srDecrypt.ReadToEnd();
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

    }
}
