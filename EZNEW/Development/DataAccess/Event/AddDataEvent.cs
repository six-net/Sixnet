using System.Collections.Generic;

namespace EZNEW.Development.DataAccess.Event
{
    /// <summary>
    /// Add data event
    /// </summary>
    public class AddDataEvent : BaseDataAccessEvent
    {
        public AddDataEvent()
        {
            EventType = DataAccessEventType.AddData;
        }

        /// <summary>
        /// Gets or sets the event data
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Gets or sets the add values
        /// </summary>
        public Dictionary<string, dynamic> Values { get; set; }
    }
}
