using System;
using System.Drawing;

namespace EZNEW.Drawing.VerificationCode
{
    /// <summary>
    /// Verification code provider
    /// </summary>

    [Serializable]
    public abstract class VerificationCodeProvider
    {
        #region Fields

        protected int lengthValue = 5;//code length

        protected int minLength = 1;//min length

        protected int maxLenght = 50;//max length

        protected static readonly char[] charArray = { '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'm', 'n', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        protected static readonly Random random = new Random();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the code length
        /// </summary>
        public int Length
        {
            get
            {
                return lengthValue;
            }
            set
            {
                SetCodeLength(value);
            }
        }

        /// <summary>
        /// Gets or sets the verification code type
        /// </summary>
        public VerificationCodeType CodeType { get; set; } = VerificationCodeType.NumberAndLetter;

        /// <summary>
        /// Gets or sets the font size
        /// </summary>
        public int FontSize { get; set; } = 30;

        /// <summary>
        /// Gets or sets the char space
        /// </summary>
        public int SpaceBetween { get; set; } = 1;

        /// <summary>
        /// Gets or sets the background color.
        /// Use White by default.
        /// </summary>
        public Color BackgroundColor { get; set; } = Color.White;

        /// <summary>
        /// Gets or sets the font color.
        /// Use random colors if set to null value.
        /// </summary>
        public Color? FontColor { get; set; } = null;

        /// <summary>
        /// Gets or sets the interfere lines number
        /// </summary>
        public int InterfereNum { get; set; } = 3;

        /// <summary>
        /// Gets or sets the interfere line color
        /// Use random colors if set to null value
        /// </summary>
        public Color? InterfereColor { get; set; } = null;

        /// <summary>
        /// Gets or sets the font family name
        /// </summary>
        public string FontFamilyName { get; set; } = string.Empty;

        #endregion

        #region Methods

        #region Set code length

        /// <summary>
        /// Sets code length
        /// </summary>
        /// <param name="length">Code length</param>
        private void SetCodeLength(int length)
        {
            if (length <= 0)
            {
                return;
            }
            length = length < minLength ? minLength : length;
            length = length > maxLenght ? maxLenght : length;
            lengthValue = length;
        }

        #endregion

        #region Generate Code

        /// <summary>
        /// Generate code
        /// </summary>
        /// <returns>Return the code image bytes</returns>
        public abstract VerificationCodeValue CreateCode();

        #endregion

        #endregion
    }
}
