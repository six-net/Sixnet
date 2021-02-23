using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Sms
{
    /// <summary>
    /// Sms result
    /// </summary>
    public abstract class SmsResult
    {
        /// <summary>
        /// Gets or sets the request id
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Gets or sets the result
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// Gets or sets the code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets whether success
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the sms account
        /// </summary>
        public SmsAccount SmsAccount { get; set; }

        /// <summary>
        /// Get sor sets the sms options
        /// </summary>
        public SmsOptions SmsOptions { get; set; }

        public static T NoSmsProvider<T>() where T : SmsResult, new()
        {
            return new T()
            {
                Success = false,
                Description = "No sms provider specified"
            };
        }

        public static T NoSmsAccount<T>() where T : SmsResult, new()
        {
            return new T()
            {
                Success = false,
                Description = "No sms account specified"
            };
        }

        public static T SendSuccess<T>() where T : SmsResult, new()
        {
            return new T()
            {
                Success = true
            };
        }
    }
}
