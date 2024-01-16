using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Sixnet.Security.Cryptography
{
    /// <summary>
    /// RSA key configuration
    /// </summary>
    public class RSAKeyConfiguration
    {
        /// <summary>
        /// Public key
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// Private key
        /// </summary>
        public string PrivateKey { get; set; }
    }
}
