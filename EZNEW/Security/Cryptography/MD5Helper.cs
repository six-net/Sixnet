using System;
using System.Security.Cryptography;
using System.Text;

namespace EZNEW.Security.Cryptography
{
    /// <summary>
    /// MD5 algorithm helper
    /// </summary>
    public static class MD5Helper
    {
        /// <summary>
        /// md5 provider
        /// </summary>
        static readonly MD5CryptoServiceProvider MD5CryptoServiceProvider = new MD5CryptoServiceProvider();

        #region Encrypts string

        /// <summary>
        /// Encrypts string
        /// </summary>
        /// <param name="originalValue">Original value</param>
        /// <returns>Return the encrypted value</returns>
        public static string Encrypt(string originalValue)
        {
            #region varyfy args

            if (string.IsNullOrWhiteSpace(originalValue))
            {
                return string.Empty;
            }

            #endregion

            byte[] valueBytes = Encoding.UTF8.GetBytes(originalValue);
            byte[] md5Bytes = MD5CryptoServiceProvider.ComputeHash(valueBytes);
            string encryptString = BitConverter.ToString(md5Bytes);
            return encryptString.Replace("-", string.Empty).ToLower();
        }

        #endregion

        #region Specifies the number of times to encrypt a string

        /// <summary>
        /// Specifies the number of times to encrypt a string
        /// </summary>
        /// <param name="originalValue">Original value</param>
        /// <param name="times">Encrypt times</param>
        /// <returns>Return the encrypted value</returns>
        public static string Encrypt(string originalValue, int times)
        {
            #region verify args

            if (string.IsNullOrWhiteSpace(originalValue))
            {
                return string.Empty;
            }

            #endregion

            string encryptValue;
            do
            {
                encryptValue = Encrypt(originalValue);
                times--;
            } while (times > 0);
            return encryptValue;
        }

        #endregion
    }
}
