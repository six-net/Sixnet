using System;
using System.Collections.Generic;

namespace EZNEW.Develop.DataAccess.Event
{
    /// <summary>
    /// Delete data event
    /// </summary>
    [Serializable]
    public class DeleteDataEvent : BaseDataAccessEvent
    {
        /// <summary>
        /// Gets or sets the data
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Gets or sets the key values
        /// </summary>
        public Dictionary<string, dynamic> KeyValues { get; set; }
    }
}
