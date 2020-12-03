using EZNEW.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Data.Cache.Policy
{
    /// <summary>
    /// Check data result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CheckDataResult<T>
    {
        /// <summary>
        /// Determines whether need to query database
        /// </summary>
        public bool QueryDatabase { get; set; }

        /// <summary>
        /// Determines whhether has value
        /// </summary>
        public bool HasValue { get; set; }

        public static CheckDataResult<T> Default(string message = "")
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                LogManager.LogWarning<QueryDataResult<T>>(message);
            }
            return new CheckDataResult<T>()
            {
                QueryDatabase = true,
                HasValue = false
            };
        }
    }
}
