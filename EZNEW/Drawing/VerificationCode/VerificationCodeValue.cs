using System;

namespace EZNEW.Drawing.VerificationCode
{
    /// <summary>
    /// Verification code
    /// </summary>
    [Serializable]
    public class VerificationCodeValue
    {
        /// <summary>
        /// Gets or sets the verification code value
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the verification code image file bytes
        /// </summary>
        public byte[] FileBytes { get; set; }
    }
}
