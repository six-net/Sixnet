using System;

namespace EZNEW.Drawing.VerificationCode
{
    /// <summary>
    /// Defines verification code type
    /// </summary>
    [Serializable]
    public enum VerificationCodeType
    {
        Number = 2,
        Letter = 4,
        NumberAndLetter = 6
    }
}
