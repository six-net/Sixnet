using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Domain.Repository.Warehouse
{
    /// <summary>
    /// count result
    /// </summary>
    public class CountResult
    {
        /// <summary>
        /// new data count
        /// </summary>
        public long NewDataCount
        {
            get;set;
        }

        /// <summary>
        /// persistent data remove count
        /// </summary>
        public long PersistentDataRemoveCount
        {
            get;set;
        }

        /// <summary>
        /// persistent data count
        /// </summary>
        public long PersistentDataCount
        {
            get;set;
        }

        /// <summary>
        /// total data count
        /// </summary>
        public long TotalDataCount
        {
            get;set;
        }
    }
}
