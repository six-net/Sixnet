// "Company © 2025. All rights reserved."

namespace Sixnet.Drawing.VerificationCode
{
    /// <summary>
    /// Verification code
    /// </summary>
    [Serializable]
    public class SixnetVerificationCodeValue
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
