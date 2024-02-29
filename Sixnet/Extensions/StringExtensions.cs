using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sixnet.App;
using Sixnet.Localization;
using Sixnet.Security.Cryptography;

namespace System
{
    /// <summary>
    /// String extensions
    /// </summary>
    public static class StringExtensions
    {
        #region Fields

        static readonly char[] PathTrimChars = new char[2] { '/', '\\' };
        static int[] LetterCodes = { 45217, 45253, 45761, 46318, 46826, 47010, 47297, 47614, 48119, 48119, 49062, 49324, 49896, 50371, 50614, 50622, 50906, 51387, 51446, 52218, 52698, 52698, 52698, 52980, 53689, 54481 };
        static Encoding gbk = Encoding.GetEncoding("GBK");

        #endregion

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

        #region Return the string first letter string

        public static string GetAllWordsFirstLetter(this string value)
        {
            return string.Join("", value.Select(c => c.ToString().GetFirstLetter()));
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
            var firstLetter = value.ToString();
            var valueBytes = gbk.GetBytes(value.ToString());
            if (valueBytes != null && valueBytes.Length == 2)
            {
                short byteOneValue = valueBytes[0];
                short byteTwoValue = valueBytes[1];
                int chineseCode = (byteOneValue << 8) + byteTwoValue;
                if (chineseCode >= 45217 && chineseCode <= 55289)
                {
                    for (int i = 25; i >= 0; i--)
                    {
                        if (chineseCode >= LetterCodes[i])
                        {
                            firstLetter = gbk.GetString(new byte[] { (byte)(65 + i) });
                            break;
                        }
                    }
                }
                // 'A'; 45217..45252  'B'; 45253..45760 'C'; 45761..46317  'D'; 46318..46825 'E'; 46826..47009 'F'; 47010..47296  'G'; 47297..47613
                // 'H'; 47614..48118 'J'; 48119..49061 'K'; 49062..49323 'L'; 49324..49895 'M'; 49896..50370 'N'; 50371..50613  
                // 'O'; 50614..50621 'P'; 50622..50905 'Q'; 50906..51386 'R'; 51387..51445 'S'; 51446..52217  'T'; 52218..52697  
                // not have U,V
                // 'W'; 52698..52979  'X'; 52980..53640  'Y'; 53689..54480 'Z'; 54481..55289
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

        #region Return uri path and query

        /// <summary>
        /// Return uri path and query
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns></returns>
        public static string GetUriPathAndQuery(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }
            var path = value;
            if (Uri.IsWellFormedUriString(value, UriKind.Absolute))
            {
                Uri uri = new Uri(value);
                path = uri.PathAndQuery?.Trim(PathTrimChars);
            }
            string virtualPath = SixnetApplication.VirtualPath?.Trim(PathTrimChars);
            if (!string.IsNullOrWhiteSpace(virtualPath))
            {
                var virthalPathArray = path.LSplit(virtualPath);
                path = virthalPathArray.Length > 0 ? virthalPathArray[0] : string.Empty;
            }
            return path?.Trim(PathTrimChars) ?? string.Empty;
        }

        #endregion

        #region Get local string

        /// <summary>
        /// Convert to local string
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="args">Args</param>
        /// <returns></returns>
        public static string Localize(this string value,params string[] args)
        {
            return SixnetLocalizer.GetString(value,args);
        }

        /// <summary>
        /// Convert to local string
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="args">Args</param>
        /// <returns></returns>
        public static string Localize<TResourceSource>(this string value, params string[] args)
        {
            return SixnetLocalizer.GetString<TResourceSource>(value, args);
        }        

        #endregion
    }
}
