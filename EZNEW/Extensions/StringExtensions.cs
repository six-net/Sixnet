using System.Text;
using System.Text.RegularExpressions;
using EZNEW.Security.Cryptography;

namespace System
{
    /// <summary>
    /// String extensions
    /// </summary>
    public static class StringExtensions
    {
        #region Encrypy string by MD5

        /// <summary>
        /// Encrypy a string by MD5
        /// </summary>
        /// <param name="value">String value</param>
        /// <returns>Return encrypy value</returns>
        public static string MD5(this string value)
        {
            return MD5Helper.Encrypt(value);
        }

        #endregion

        #region Split a string to a string array

        /// <summary>
        /// Split a string to a string array
        /// </summary>
        /// <param name="stringValue">String value</param>
        /// <param name="splitString">Split string</param>
        /// <param name="removeEmpty">Whether remove empty string</param>
        /// <returns>Return the string array</returns>
        public static string[] LSplit(this string stringValue, string splitString, bool removeEmpty = true)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return new string[0];
            }
            if (string.IsNullOrWhiteSpace(splitString))
            {
                return new string[] { stringValue };
            }
            return stringValue.Split(new string[] { splitString }, removeEmpty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
        }

        #endregion

        #region Return the string value first letter

        /// <summary>
        /// Get the string value first letter
        /// </summary>
        /// <param name="value">String value</param>
        /// <returns>Return the first letter</returns>
        public static string GetFirstLetter(this string value)
        {
            #region verify args

            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            #endregion

            char firstChar = value[0];
            if (firstChar.IsChineseLetter())
            {
                return firstChar.GetChineseFirstLetter();
            }
            return firstChar.ToString();
        }

        #endregion

        #region Return the chinese first letter

        /// <summary>
        /// Return the chinese char first letter
        /// </summary>
        /// <param name="value">Chinese char</param>
        /// <returns>Return the first letter</returns>
        public static string GetChineseFirstLetter(this char value)
        {
            string firstLetter = string.Empty;
            byte[] valueBytes = Encoding.Default.GetBytes(value.ToString());
            if (valueBytes != null && valueBytes.Length == 2)
            {
                int byteOneValue = valueBytes[0];
                int byteTwoValue = valueBytes[1];
                int chineseValue = byteOneValue * 256 + byteTwoValue;
                // 'A'; 45217..45252  'B'; 45253..45760 'C'; 45761..46317  'D'; 46318..46825 'E'; 46826..47009 'F'; 47010..47296  'G'; 47297..47613
                // 'H'; 47614..48118 'J'; 48119..49061 'K'; 49062..49323 'L'; 49324..49895 'M'; 49896..50370 'N'; 50371..50613  
                // 'O'; 50614..50621 'P'; 50622..50905 'Q'; 50906..51386 'R'; 51387..51445 'S'; 51446..52217  'T'; 52218..52697  
                // not have U,V
                // 'W'; 52698..52979  'X'; 52980..53640  'Y'; 53689..54480 'Z'; 54481..55289
                if (chineseValue < 45217 || chineseValue > 55289)
                {
                    return string.Empty;
                }
                if (chineseValue <= 45252)
                {
                    firstLetter = "A";
                }
                else if (chineseValue <= 45760)
                {
                    firstLetter = "B";
                }
                else if (chineseValue <= 46317)
                {
                    firstLetter = "C";
                }
                else if (chineseValue <= 46825)
                {
                    firstLetter = "D";
                }
                else if (chineseValue <= 47009)
                {
                    firstLetter = "E";
                }
                else if (chineseValue <= 47296)
                {
                    firstLetter = "F";
                }
                else if (chineseValue <= 47613)
                {
                    firstLetter = "G";
                }
                else if (chineseValue <= 48118)
                {
                    firstLetter = "H";
                }
                else if (chineseValue <= 49061)
                {
                    firstLetter = "J";
                }
                else if (chineseValue <= 49323)
                {
                    firstLetter = "K";
                }
                else if (chineseValue <= 49895)
                {
                    firstLetter = "L";
                }
                else if (chineseValue <= 50370)
                {
                    firstLetter = "M";
                }
                else if (chineseValue <= 50613)
                {
                    firstLetter = "N";
                }
                else if (chineseValue <= 50621)
                {
                    firstLetter = "O";
                }
                else if (chineseValue <= 50905)
                {
                    firstLetter = "P";
                }
                else if (chineseValue <= 51386)
                {
                    firstLetter = "Q";
                }
                else if (chineseValue <= 51445)
                {
                    firstLetter = "R";
                }
                else if (chineseValue <= 52217)
                {
                    firstLetter = "S";
                }
                else if (chineseValue <= 52697)
                {
                    firstLetter = "T";
                }
                else if (chineseValue <= 52979)
                {
                    firstLetter = "W";
                }
                else if (chineseValue <= 53640)
                {
                    firstLetter = "X";
                }
                else if (chineseValue <= 54480)
                {
                    firstLetter = "Y";
                }
                else if (chineseValue <= 55289)
                {
                    firstLetter = "Z";
                }
            }
            return firstLetter;
        }

        #endregion

        #region Remove html tags

        /// <summary>
        /// Remove all of the html tags in the string value
        /// </summary>
        /// <param name="htmlString">Html string</param>
        /// <returns>Return the removed string</returns>
        public static string RemoveHtmlTags(this string htmlString)
        {
            //remove scripts
            htmlString = Regex.Replace(htmlString, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //remove html
            htmlString = Regex.Replace(htmlString, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"-->", "", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"<!--.*", "", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            htmlString.Replace("<", "");
            htmlString.Replace(">", "");
            htmlString.Replace("\r\n", "");
            return htmlString;
        }

        #endregion

        #region Replace by regex

        /// <summary>
        /// Encrypt string value by regex
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="pattern">Reegex pattern</param>
        /// <param name="replaceValue">Replace value</param>
        /// <returns>Return the new string</returns>
        public static string ReplaceByRegex(this string value, string pattern, string replaceValue)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(pattern))
            {
                return value;
            }
            Regex reg = new Regex(pattern);
            return reg.Replace(value, replaceValue);
        }

        #endregion

        #region String to binary

        /// <summary>
        /// Convert string value to binary string
        /// </summary>
        /// <param name="value">Original value</param>
        /// <param name="encoding">Encoding</param>
        /// <returns>Return the binary string</returns>
        public static string ToBinaryString(this string value, Encoding encoding = null)
        {
            if (value == null)
            {
                return null;
            }
            encoding = encoding ?? Encoding.Default;
            byte[] data = encoding.GetBytes(value);
            StringBuilder result = new StringBuilder(data.Length * 8);
            foreach (byte b in data)
            {
                result.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
            }
            return result.ToString();
        }

        #endregion

        #region Binary to string

        /// <summary>
        /// Convert binary string to original string
        /// </summary>
        /// <param name="binaryString">Binary string value</param>
        /// <param name="encoding">Encoding</param>
        /// <returns>Return original string</returns>
        public static string ToOriginalString(this string binaryString, Encoding encoding = null)
        {
            if (string.IsNullOrWhiteSpace(binaryString))
            {
                return string.Empty;
            }
            encoding = encoding ?? Encoding.Default;
            CaptureCollection captures = Regex.Match(binaryString, @"([01]{8})+").Groups[1].Captures;
            byte[] data = new byte[captures.Count];
            for (int i = 0; i < captures.Count; i++)
            {
                data[i] = Convert.ToByte(captures[i].Value, 2);
            }
            return encoding.GetString(data, 0, data.Length);
        }

        #endregion
    }
}
