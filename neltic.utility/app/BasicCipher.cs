namespace neltic
{
    using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.IO;
	using System.Linq;
	using System.Security.Cryptography;
	using System.Text;

    public sealed class BasicCipher
    {
        public static readonly BasicCipher Instance = new BasicCipher();

        private static readonly string vector = ConfigurationManager.AppSettings["BasicCipherVector"];
        private const int keysize = 256;
        private const string defaultPass = "Itz0r1.4";

        public string Encrypt(string token) { return Encrypt(token, defaultPass); }
        public string Encrypt(string token, string pass)
        {
            byte[] cipherTextBytes;
            byte[] vectorBytes = Encoding.UTF8.GetBytes(vector);
            byte[] tokenBytes = Encoding.UTF8.GetBytes(token);
            PasswordDeriveBytes password = new PasswordDeriveBytes(pass, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, vectorBytes);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(tokenBytes, 0, tokenBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                }
            }
            return Convert.ToBase64String(cipherTextBytes);
        }

        public string Decrypt(string token) { return Decrypt(token, defaultPass); }
        public string Decrypt(string token, string pass)
        {
            int decryptedByteCount;
            byte[] tokenBytes;
            byte[] vectorBytes = Encoding.ASCII.GetBytes(vector);
            byte[] cipherTextBytes = Convert.FromBase64String(token);
            PasswordDeriveBytes password = new PasswordDeriveBytes(pass, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, vectorBytes);
            using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    tokenBytes = new byte[cipherTextBytes.Length];
                    decryptedByteCount = cryptoStream.Read(tokenBytes, 0, tokenBytes.Length);
                }
            }
            return Encoding.UTF8.GetString(tokenBytes, 0, decryptedByteCount);
        }

    }
}
