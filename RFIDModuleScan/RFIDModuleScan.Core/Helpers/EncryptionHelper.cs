using System;
using System.IO;
using System.Text;
using PCLCrypto;
using static PCLCrypto.WinRTCrypto;

namespace RFIDModuleScan.Core.Helpers
{

    public static class EncryptionHelper
    {
        private static string password = "G1n@pp7711#!0000";

        public static string Encrypt(string s)
        {
            var symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmName.Aes, SymmetricAlgorithmMode.Cbc, SymmetricAlgorithmPadding.Zeros);
            ICryptographicKey key = null;
            key = symProvider.CreateSymmetricKey(Encoding.UTF8.GetBytes(password));
            var plainString = s;
            var plain = Encoding.UTF8.GetBytes(plainString);
            var encrypted = CryptographicEngine.Encrypt(key, plain);
            var encryptedString = Convert.ToBase64String(encrypted);
            return encryptedString;
        }

        public static string Decrypt(string encryptedString)
        {
            var symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmName.Aes, SymmetricAlgorithmMode.Cbc, SymmetricAlgorithmPadding.None);
            ICryptographicKey key = null;
            key = symProvider.CreateSymmetricKey(Encoding.UTF8.GetBytes(password));
            var encrypted = Convert.FromBase64String(encryptedString);
            var decrypted = CryptographicEngine.Decrypt(key, encrypted);
            var decryptedString = Encoding.UTF8.GetString(decrypted, 0, decrypted.Length);

            var cleanedString = "";
            foreach(var ch in decryptedString.ToCharArray())
            {
                if (ch != '\0')
                {
                    cleanedString += ch;
                }
            }

            return cleanedString;
        }
    }

}