using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Sms
{
    /// <summary>
    /// Sms account
    /// </summary>
    public class SmsAccount
    {
        /// <summary>
        /// Gets or sets the account name
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// Gets or sets the password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the sign
        /// </summary>
        public string Sign { get; set; }

        /// <summary>
        /// Gets or sets the config
        /// </summary>
        public Dictionary<string, string> ConfigItems { get; set; }

        internal SmsAccount Clone()
        {
            return new SmsAccount()
            {
                AccountName = AccountName,
                ConfigItems = ConfigItems?.ToDictionary(c => c.Key, c => c.Value) ?? new Dictionary<string, string>(0),
                Password = Password,
                Sign = Sign
            };
        }
    }
}
