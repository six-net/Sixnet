using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace Sixnet.Security.Cryptography
{
    /// <summary>
    /// Defines aes helper
    /// </summary>
    public static class AesHelper
    {
        static readonly ConcurrentDictionary<string, Aes> AesCollections = new ConcurrentDictionary<string, Aes>();
        const string DefaultIV = "~!@#$%^&*()_+123";

        #region Encrypt

        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="originalValue">Original value</param>
        /// <param name="iv">IV</param>
        /// <param name="encoding">Encoding(The default is UTF-8)</param>
        /// <returns>Return a encrypted value</returns>
        public static string Encrypt(string key, string originalValue, string iv = DefaultIV, Encoding encoding = null, CipherMode mode = CipherMode.ECB, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            var aes = GetAes(key, iv, mode, paddingMode, encoding);
            using (var encryptor = aes.CreateEncryptor())
            {
                byte[] encryptArray = encoding.GetBytes(originalValue);
                return Convert.ToBase64String(encryptor.TransformFinalBlock(encryptArray, 0, encryptArray.Length));
            }
        }

        #endregion

        #region Decrypt

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="encryptedValue">Original value</param>
        /// <param name="iv">IV</param>
        /// <param name="encoding">Encoding(The default is UTF-8)</param>
        /// <returns>Return a encrypted value</returns>
        public static string Decrypt(string key, string encryptedValue, string iv = DefaultIV, Encoding encoding = null, CipherMode mode = CipherMode.ECB, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            var aes = GetAes(key, iv, mode, paddingMode, encoding);
            using (var decryptor = aes.CreateDecryptor())
            {
                byte[] decryptArray = Convert.FromBase64String(encryptedValue);
                return encoding.GetString(decryptor.TransformFinalBlock(decryptArray, 0, decryptArray.Length));
            }
        }

        #endregion

        #region Get aes

        static Aes GetAes(string key, string iv, CipherMode mode = CipherMode.ECB, PaddingMode paddingMode = PaddingMode.PKCS7, Encoding encoding = null)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException(nameof(key));
            }
            if (string.IsNullOrWhiteSpace(iv))
            {
                iv = DefaultIV;
            }
            string aesKey = $"{key}{iv}{mode}{paddingMode}{encoding.EncodingName}";
            if (AesCollections.TryGetValue(aesKey, out var aes) && aes != null)
            {
                return aes;
            }
            aes = Aes.Create();
            aes.Mode = mode;
            aes.Padding = paddingMode;
            aes.Key = encoding.GetBytes(key);
            aes.IV = encoding.GetBytes(iv);
            AesCollections[aesKey] = aes;
            return aes;
        }

        #endregion
    }
}
