using System.Text;
using System.Text.RegularExpressions;
using EZNEW.Security;

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

        #region Return the string value first char

        /// <summary>
        /// return the string value first char
        /// </summary>
        /// <param name="value">String value</param>
        /// <returns>Return the first char</returns>
        public static string GetFirstChar(this string value)
        {
            #region verify args

            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            #endregion

            char firstCode = value[0];
            if (firstCode.IsChineseLetter())
            {
                return firstCode.GetChineseCharFirtLetter();
            }
            return firstCode.ToString();
        }

        #endregion

        #region Return the chinese char first letter

        /// <summary>
        /// Return the chinese char first letter
        /// </summary>
        /// <param name="value">Chinese char</param>
        /// <returns>Return the first letter</returns>
        public static string GetChineseCharFirtLetter(this char value)
        {
            string firstCode = string.Empty;
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
                    firstCode = "A";
                }
                else if (chineseValue <= 45760)
                {
                    firstCode = "B";
                }
                else if (chineseValue <= 46317)
                {
                    firstCode = "C";
                }
                else if (chineseValue <= 46825)
                {
                    firstCode = "D";
                }
                else if (chineseValue <= 47009)
                {
                    firstCode = "E";
                }
                else if (chineseValue <= 47296)
                {
                    firstCode = "F";
                }
                else if (chineseValue <= 47613)
                {
                    firstCode = "G";
                }
                else if (chineseValue <= 48118)
                {
                    firstCode = "H";
                }
                else if (chineseValue <= 49061)
                {
                    firstCode = "J";
                }
                else if (chineseValue <= 49323)
                {
                    firstCode = "K";
                }
                else if (chineseValue <= 49895)
                {
                    firstCode = "L";
                }
                else if (chineseValue <= 50370)
                {
                    firstCode = "M";
                }
                else if (chineseValue <= 50613)
                {
                    firstCode = "N";
                }
                else if (chineseValue <= 50621)
                {
                    firstCode = "O";
                }
                else if (chineseValue <= 50905)
                {
                    firstCode = "P";
                }
                else if (chineseValue <= 51386)
                {
                    firstCode = "Q";
                }
                else if (chineseValue <= 51445)
                {
                    firstCode = "R";
                }
                else if (chineseValue <= 52217)
                {
                    firstCode = "S";
                }
                else if (chineseValue <= 52697)
                {
                    firstCode = "T";
                }
                else if (chineseValue <= 52979)
                {
                    firstCode = "W";
                }
                else if (chineseValue <= 53640)
                {
                    firstCode = "X";
                }
                else if (chineseValue <= 54480)
                {
                    firstCode = "Y";
                }
                else if (chineseValue <= 55289)
                {
                    firstCode = "Z";
                }
            }

            return firstCode;
        }

        #endregion

        #region Remove html tags

        /// <summary>
        /// Remove all of the html tags in the string value
        /// </summary>
        /// <param name="value">String value</param>
        /// <returns>Return the removed string</returns>
        public static string RemoveHtmlTags(this string value)
        {
            //remove scripts
            value = Regex.Replace(value, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //remove html
            value = Regex.Replace(value, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            value = Regex.Replace(value, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            value = Regex.Replace(value, @"-->", "", RegexOptions.IgnoreCase);
            value = Regex.Replace(value, @"<!--.*", "", RegexOptions.IgnoreCase);
            value = Regex.Replace(value, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            value = Regex.Replace(value, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            value = Regex.Replace(value, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            value = Regex.Replace(value, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            value = Regex.Replace(value, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            value = Regex.Replace(value, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            value = Regex.Replace(value, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            value = Regex.Replace(value, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            value = Regex.Replace(value, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            value = Regex.Replace(value, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            value.Replace("<", "");
            value.Replace(">", "");
            value.Replace("\r\n", "");
            return value;
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
