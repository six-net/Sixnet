using System;
using System.Collections.Generic;
using EZNEW.Language;
using EZNEW.Language.Chinese;

namespace EZNEW.Model
{
    /// <summary>
    /// Chinese text
    /// </summary>
    [Serializable]
    public struct ChineseText
    {
        #region Fields

        /// <summary>
        /// Chinese spelling
        /// </summary>
        string spelling;

        /// <summary>
        /// Spelling short form
        /// </summary>
        string spellingShort;

        /// <summary>
        /// Spelling is inited
        /// </summary>
        bool spellingInit;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the EZNEW.ValueType.ChineseText
        /// </summary>
        /// <param name="chineseText">Chinese text</param>
        public ChineseText(string chineseText)
        {
            Text = chineseText.Trim();
            spelling = "";
            spellingShort = "";
            spellingInit = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets spelling
        /// </summary>
        public string Spelling
        {
            get
            {
                return GetSpelling();
            }
        }

        /// <summary>
        /// Gets spelling short
        /// </summary>
        public string SpellingShort
        {
            get
            {
                return GetSpellingShort();
            }
        }

        /// <summary>
        /// Gets chinese text
        /// </summary>
        public string Text { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets spelling
        /// </summary>
        /// <returns>Return the spelling</returns>
        string GetSpelling()
        {
            if (spellingInit)
            {
                InitSpelling();
            }
            return spelling;
        }

        /// <summary>
        /// Gets spelling short
        /// </summary>
        /// <returns>Return spelling short</returns>
        string GetSpellingShort()
        {
            if (spellingInit)
            {
                InitSpelling();
            }
            return spellingShort;
        }

        /// <summary>
        /// Init spelling
        /// </summary>
        void InitSpelling()
        {
            if (!string.IsNullOrWhiteSpace(Text))
            {
                var chineseProvider = ChineseManager.GetChineseProvider();
                if (chineseProvider == null)
                {
                    return;
                }
                spelling = chineseProvider.GetSpellingBySimpleChinese(Text);
                spellingShort = chineseProvider.GetSpellingShortSimpleChinese(Text);
            }
            spellingInit = true;
        }

        /// <summary>
        /// Override ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Text;
        }

        /// <summary>
        /// Implicit convert to string
        /// </summary>
        /// <param name="text">Chinese text</param>
        public static implicit operator string(ChineseText text)
        {
            return text.Text;
        }

        /// <summary>
        /// Implicit convert to chinese text
        /// </summary>
        /// <param name="value">Sttring value</param>
        public static implicit operator ChineseText(string value)
        {
            return new ChineseText(value);
        }

        #endregion
    }
}
