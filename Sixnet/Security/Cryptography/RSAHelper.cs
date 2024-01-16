using System;
using System.Collections.Concurrent;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Sixnet.Security.Cryptography
{
    public static class RSAHelper
    {
        static readonly ConcurrentDictionary<string, RSA> _RSACollections = new ConcurrentDictionary<string, RSA>();

        #region Encrypt

        /// <summary>
        /// Encrypt value
        /// </summary>
        /// <param name="publicKey">Public key</param>
        /// <param name="originalValue">Original value</param>
        /// <param name="encoding">Encoding(The default is UTF-8)</param>
        /// <returns>Return the encrypted value</returns>
        public static string Encrypt(string publicKey, string originalValue, Encoding encoding = null)
        {
            var rsa = CreateRSAProviderFromPublicKey(publicKey);
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            return Convert.ToBase64String(rsa.Encrypt(encoding.GetBytes(originalValue), RSAEncryptionPadding.Pkcs1));
        }

        #endregion

        #region Decrypt

        /// <summary>
        /// Decrypt value
        /// </summary>
        /// <param name="privateKey">Private key</param>
        /// <param name="encryptedValue">Encrypted value</param>
        /// <param name="encoding">Encoding(The default is UTF-8)</param>
        /// <returns>Return the original value</returns>
        public static string Decrypt(string privateKey, string encryptedValue, Encoding encoding = null)
        {
            var rsa = CreateRSAProviderFromPrivateKey(privateKey);
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            return encoding.GetString(rsa.Decrypt(Convert.FromBase64String(encryptedValue), RSAEncryptionPadding.Pkcs1));
        }

        #endregion

        #region Sign

        /// <summary>
        /// Sign data
        /// </summary>
        /// <param name="privateKey">Private key</param>
        /// <param name="originalValue">Original value</param>
        /// <param name="encoding">Encoding(The default is UTF-8)</param>
        /// <param name="hashAlgorithmName">Hash algorithm nam(The default is SHA256)</param>
        /// <returns></returns>
        public static string Sign(string privateKey, string originalValue, Encoding encoding = null, HashAlgorithmName? hashAlgorithmName = null)
        {
            var rsa = CreateRSAProviderFromPrivateKey(privateKey);
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            if (hashAlgorithmName == null)
            {
                hashAlgorithmName = HashAlgorithmName.SHA256;
            }
            byte[] dataBytes = encoding.GetBytes(originalValue);
            var signatureBytes = rsa.SignData(dataBytes, hashAlgorithmName.Value, RSASignaturePadding.Pkcs1);
            return Convert.ToBase64String(signatureBytes);
        }

        #endregion

        #region Verify sign

        /// <summary>
        /// Verify sign
        /// </summary>
        /// <param name="publicKey">Public key</param>
        /// <param name="originalData">Original data</param>
        /// <param name="sign">Sign</param>
        /// <param name="encoding">Encoding(The default is UTF-8)</param>
        /// <param name="hashAlgorithmName">Hash algorithm nam(The default is SHA256)</param>
        /// <returns></returns>
        public static bool Verify(string publicKey, string originalData, string sign, Encoding encoding = null, HashAlgorithmName? hashAlgorithmName = null)
        {
            var rsa = CreateRSAProviderFromPublicKey(publicKey);
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            if (hashAlgorithmName == null)
            {
                hashAlgorithmName = HashAlgorithmName.SHA256;
            }
            byte[] dataBytes = encoding.GetBytes(originalData);
            byte[] signBytes = Convert.FromBase64String(sign);
            var verify = rsa.VerifyData(dataBytes, signBytes, hashAlgorithmName.Value, RSASignaturePadding.Pkcs1);
            return verify;
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
            if (string.IsNullOrWhiteSpace(publicKey))
            {
                throw new ArgumentException(nameof(publicKey));
            }
            if (_RSACollections.TryGetValue(publicKey, out var rsa) && rsa != null)
            {
                return rsa;
            }

            // encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
            byte[] seqOid = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] seq = new byte[15];
            var x509Key = Convert.FromBase64String(publicKey);

            // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
            using (MemoryStream mem = new MemoryStream(x509Key))
            {
                using (BinaryReader binr = new BinaryReader(mem))  //wrap Memory Stream with BinaryReader for easy reading
                {
                    byte bt = 0;
                    ushort twobytes = 0;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                        binr.ReadByte();    //advance 1 byte
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();   //advance 2 bytes
                    else
                        return null;

                    seq = binr.ReadBytes(15);       //read the Sequence OID
                    if (!CompareByteArrays(seq, seqOid))    //make sure Sequence for OID is correct
                        return null;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8103) //data read as little endian order (actual data order for Bit String is 03 81)
                        binr.ReadByte();    //advance 1 byte
                    else if (twobytes == 0x8203)
                        binr.ReadInt16();   //advance 2 bytes
                    else
                        return null;

                    bt = binr.ReadByte();
                    if (bt != 0x00)     //expect null byte next
                        return null;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                        binr.ReadByte();    //advance 1 byte
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();   //advance 2 bytes
                    else
                        return null;

                    twobytes = binr.ReadUInt16();
                    byte lowbyte = 0x00;
                    byte highbyte = 0x00;

                    if (twobytes == 0x8102) //data read as little endian order (actual data order for Integer is 02 81)
                        lowbyte = binr.ReadByte();  // read next bytes which is bytes in modulus
                    else if (twobytes == 0x8202)
                    {
                        highbyte = binr.ReadByte(); //advance 2 bytes
                        lowbyte = binr.ReadByte();
                    }
                    else
                        return null;
                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };   //reverse byte order since asn.1 key uses big endian order
                    int modsize = BitConverter.ToInt32(modint, 0);

                    int firstbyte = binr.PeekChar();
                    if (firstbyte == 0x00)
                    {   //if first byte (highest order) of modulus is zero, don't include it
                        binr.ReadByte();    //skip this null byte
                        modsize -= 1;   //reduce modulus buffer size by 1
                    }

                    byte[] modulus = binr.ReadBytes(modsize);   //read the modulus bytes

                    if (binr.ReadByte() != 0x02)            //expect an Integer for the exponent data
                        return null;
                    int expbytes = (int)binr.ReadByte();        // should only need one byte for actual exponent data (for all useful values)
                    byte[] exponent = binr.ReadBytes(expbytes);

                    // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                    rsa = RSA.Create();
                    RSAParameters rsaKeyInfo = new RSAParameters
                    {
                        Modulus = modulus,
                        Exponent = exponent
                    };
                    rsa.ImportParameters(rsaKeyInfo);
                    _RSACollections[publicKey] = rsa;
                    return rsa;
                }
            }
        }

        #endregion

        #region Create RSA provider from private key

        /// <summary>
        /// Create RSA provider from private key
        /// </summary>
        /// <param name="privateKey">Private key</param>
        /// <returns>Return a RSA instance</returns>
        static RSA CreateRSAProviderFromPrivateKey(string privateKey)
        {
            if (string.IsNullOrWhiteSpace(privateKey))
            {
                throw new ArgumentException(nameof(privateKey));
            }
            if (_RSACollections.TryGetValue(privateKey, out var rsa) && rsa != null)
            {
                return rsa;
            }

            var privateKeyBits = Convert.FromBase64String(privateKey);

            rsa = RSA.Create();
            var rsaParameters = new RSAParameters();

            using (BinaryReader binr = new BinaryReader(new MemoryStream(privateKeyBits)))
            {
                byte bt = 0;
                ushort twobytes = 0;
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                    binr.ReadByte();
                else if (twobytes == 0x8230)
                    binr.ReadInt16();
                else
                    throw new Exception("Unexpected value read binr.ReadUInt16()");

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102)
                    throw new Exception("Unexpected version");

                bt = binr.ReadByte();
                if (bt != 0x00)
                    throw new Exception("Unexpected value read binr.ReadByte()");

                rsaParameters.Modulus = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.Exponent = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.D = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.P = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.Q = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.DP = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.DQ = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.InverseQ = binr.ReadBytes(GetIntegerSize(binr));
            }
            rsa.ImportParameters(rsaParameters);
            _RSACollections[privateKey] = rsa;
            return rsa;
        }

        #endregion

        #region Get integer size

        static int GetIntegerSize(BinaryReader binaryReader)
        {
            byte bt = 0;
            int count = 0;
            bt = binaryReader.ReadByte();
            if (bt != 0x02)
                return 0;
            bt = binaryReader.ReadByte();

            if (bt == 0x81)
                count = binaryReader.ReadByte();
            else
            if (bt == 0x82)
            {
                var highbyte = binaryReader.ReadByte();
                var lowbyte = binaryReader.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;
            }

            while (binaryReader.ReadByte() == 0x00)
            {
                count -= 1;
            }
            binaryReader.BaseStream.Seek(-1, SeekOrigin.Current);
            return count;
        }

        #endregion

        #region Compare byte arrays

        static bool CompareByteArrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }

        #endregion
    }
}
