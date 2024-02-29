using System;
using Sixnet.DependencyInjection;

namespace Sixnet.Drawing.VerificationCode
{
    /// <summary>
    /// Verification code factory
    /// </summary>
    public static class SixnetVerificationCodeFactory
    {
        /// <summary>
        /// Get verificatio code privider
        /// </summary>
        /// <returns>Return the verification code provider</returns>
        public static SixnetVerificationCodeProvider GetVerificationCodeProvider()
        {
            return SixnetContainer.GetService<SixnetVerificationCodeProvider>();
        }

        /// <summary>
        /// Generate verification code
        /// </summary>
        /// <returns>Return thee verification code value</returns>
        public static VerificationCodeValue GenerateVerificationCode()
        {
            var codeProvider = GetVerificationCodeProvider();
            if (codeProvider == null)
            {
                throw new InvalidOperationException("No verification code generators are set");
            }
            return codeProvider.CreateCode();
        }
    }
}
