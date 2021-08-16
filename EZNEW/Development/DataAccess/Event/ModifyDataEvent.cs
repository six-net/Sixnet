using System;
using System.Collections.Generic;
using EZNEW.Development.Query;

namespace EZNEW.Development.DataAccess.Event
{
    /// <summary>
    /// Modify data event
    /// </summary>
    [Serializable]
    public class ModifyDataEvent : BaseDataAccessEvent
    {
        public ModifyDataEvent()
        {
            EventType = DataAccessEventType.ModifyData;
        }

        /// <summary>
        /// Gets or sets the original values
        /// </summary>
        public Dictionary<string, dynamic> OriginalValues { get; set; }

        /// <summary>
        /// Gets or sets the new values
        /// </summary>
        public Dictionary<string, dynamic> NewValues { get; set; }

        /// <summary>
        /// Gets or sets the query object
        /// </summary>
        public IQuery Query { get; set; }
    }
}
