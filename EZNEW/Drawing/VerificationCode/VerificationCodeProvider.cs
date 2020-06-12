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

        protected int length = 5;//code length

        protected int minLength = 1;//min length

        protected int maxLenght = 50;//max length

        protected VerificationCodeType codeType = VerificationCodeType.NumberAndLetter;//type

        protected int interfereNum = 3; //interfere num

        protected static readonly char[] charArray = { '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'm', 'n', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        protected int fontSize = 30;

        protected int spaceBetween = 1;

        protected Color backgroundColor = Color.White;

        protected Color? fontColor = null;//use random color if null

        protected Color? interfereColor = null;//use random color if null

        protected string frontFamilyName = "";//front family name

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
                return length;
            }
            set
            {
                SetCodeLength(value);
            }
        }

        /// <summary>
        /// Gets or sets the verification code type
        /// </summary>
        public VerificationCodeType CodeType
        {
            get
            {
                return codeType;
            }
            set
            {
                codeType = value;
            }
        }

        /// <summary>
        /// Gets or sets the font size
        /// </summary>
        public int FontSize
        {
            get
            {
                return fontSize;
            }
            set
            {
                fontSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the char space
        /// </summary>
        public int SpaceBetween
        {
            get
            {
                return spaceBetween;
            }
            set
            {
                spaceBetween = value;
            }
        }

        /// <summary>
        /// Gets or sets the background color.
        /// Use White by default.
        /// </summary>
        public Color BackgroundColor
        {
            get
            {
                return backgroundColor;
            }
            set
            {
                backgroundColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the font color.
        /// Use random colors if set to null value.
        /// </summary>
        public Color? FontColor
        {
            get
            {
                return fontColor;
            }
            set
            {
                fontColor = value;
            }

        }

        /// <summary>
        /// Gets or sets the interfere lines number
        /// </summary>
        public int InterfereNum
        {
            get
            {
                return interfereNum;
            }
            set
            {
                interfereNum = value;
            }
        }

        /// <summary>
        /// Gets or sets the interfere line color
        /// Use random colors if set to null value
        /// </summary>
        public Color? InterfereColor
        {
            get
            {
                return interfereColor;
            }
            set
            {
                interfereColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the front family name
        /// </summary>
        public string FrontFamilyName
        {
            get
            {
                return frontFamilyName;
            }
            set
            {
                frontFamilyName = value;
            }
        }

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
            this.length = length;
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
