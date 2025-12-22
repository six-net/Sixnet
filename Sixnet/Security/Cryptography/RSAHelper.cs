// "Company © 2025. All rights reserved."

using System.Collections.Concurrent;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using Sixnet.Exceptions;

namespace Sixnet.Security.Cryptography
{
    /// <summary>
    /// RSA helper
    /// </summary>
    public static class RSAHelper
    {
        static readonly ConcurrentDictionary<string, RSA> RSACollections = new ConcurrentDictionary<string, RSA>();

        #region Encrypt

        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="value">Original value</param>
        /// <param name="publicKey">Public key</param>
        /// <param name="encoding">Encoding(The default is UTF-8)</param>
        /// <param name="encryptionPadding">Encryption padding(Default is Pkcs1)</param>
        /// <returns>Return the encrypted value</returns>
        public static string Encrypt(string value, string publicKey, Encoding encoding = null, RSAEncryptionPadding encryptionPadding = null)
        {
            var rsa = CreateRSAProviderFromPublicKey(publicKey); ;
            encoding ??= Encoding.UTF8;
            encryptionPadding ??= RSAEncryptionPadding.Pkcs1;
            return System.Convert.ToBase64String(rsa.Encrypt(encoding.GetBytes(value), encryptionPadding));
        }

        #endregion

        #region Decrypt

        /// <summary>
        /// Decrypt value
        /// </summary>
        /// <param name="value">Original value</param>
        /// <param name="privateKey">Private key</param>
        /// <param name="encoding">Encoding(The default is UTF-8)</param>
        /// <param name="encryptionPadding">Encryption padding(Default is Pkcs1)</param>
        /// <returns>Return the original value</returns>
        public static string Decrypt(string value, string privateKey, Encoding encoding = null, RSAEncryptionPadding encryptionPadding = null)
        {
            var rsa = CreateRSAProviderFromPrivateKey(privateKey);
            encoding ??= Encoding.UTF8;
            encryptionPadding ??= RSAEncryptionPadding.Pkcs1;
            return encoding.GetString(rsa.Decrypt(System.Convert.FromBase64String(value), encryptionPadding));
        }

        #endregion

        #region Sign

        /// <summary>
        /// Sign data
        /// </summary>
        /// <param name="value">Original value</param>
        /// <param name="privateKey">Private key</param>
        /// <param name="encoding">Encoding(The default is UTF-8)</param>
        /// <param name="hashAlgorithmName">Hash algorithm nam(The default is SHA256)</param>
        /// <param name="signaturePadding">Encryption padding(Default is Pkcs1)</param>
        /// <returns></returns>
        public static string Sign(string value, string privateKey, Encoding encoding = null, HashAlgorithmName? hashAlgorithmName = null, RSASignaturePadding signaturePadding = null)
        {
            var rsa = CreateRSAProviderFromPrivateKey(privateKey);
            encoding ??= Encoding.UTF8;
            hashAlgorithmName ??= HashAlgorithmName.SHA256;
            signaturePadding ??= RSASignaturePadding.Pkcs1;
            var dataBytes = encoding.GetBytes(value);
            var signatureBytes = rsa.SignData(dataBytes, hashAlgorithmName.Value, signaturePadding);
            return System.Convert.ToBase64String(signatureBytes);
        }

        #endregion

        #region Verify sign

        /// <summary>
        /// Verify sign
        /// </summary>
        /// <param name="value">Original data</param>
        /// <param name="sign">Sign</param>
        /// <param name="publicKey">Public key</param>
        /// <param name="encoding">Encoding(The default is UTF-8)</param>
        /// <param name="hashAlgorithmName">Hash algorithm nam(The default is SHA256)</param>
        /// <returns></returns>
        public static bool Verify(string value, string sign, string publicKey, Encoding encoding = null, HashAlgorithmName? hashAlgorithmName = null, RSASignaturePadding signaturePadding = null)
        {
            var rsa = CreateRSAProviderFromPublicKey(publicKey);
            encoding ??= Encoding.UTF8;
            hashAlgorithmName ??= HashAlgorithmName.SHA256;
            signaturePadding ??= RSASignaturePadding.Pkcs1;
            var dataBytes = encoding.GetBytes(value);
            var signBytes = System.Convert.FromBase64String(sign);
            return rsa.VerifyData(dataBytes, signBytes, hashAlgorithmName.Value, signaturePadding);
        }

        #endregion

        #region Create RSA provider from public key

        /// <summary>
        /// Create RSA provider from public key
        /// </summary>
        /// <param name="publicKey">Public key</param>
        /// <returns>Return a RSA instance</returns>
        static RSA CreateRSAProviderFromPublicKey(string publicKey)
        {
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(publicKey), nameof(publicKey));
            return RSACollections.GetOrAdd(publicKey, key =>
            {
                var rsa = RSA.Create();
                rsa.ImportSubjectPublicKeyInfo(System.Convert.FromBase64String(publicKey), out var _);
                return rsa;
            });
        }

        #endregion

        #region Create RSA provider from internal key

        /// <summary>
        /// Create RSA provider from internal key
        /// </summary>
        /// <param name="privateKey">Private key</param>
        /// <returns>Return a RSA instance</returns>
        static RSA CreateRSAProviderFromPrivateKey(string privateKey)
        {
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(privateKey), nameof(privateKey));
            return RSACollections.GetOrAdd(privateKey, key =>
            {
                var rsa = RSA.Create();
                rsa.ImportPkcs8PrivateKey(System.Convert.FromBase64String(privateKey), out var _);
                return rsa;
            });
        }

        #endregion
    }
}
