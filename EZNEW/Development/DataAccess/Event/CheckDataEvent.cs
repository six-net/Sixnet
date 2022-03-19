using System;
using EZNEW.Development.Query;

namespace EZNEW.Development.DataAccess.Event
{
    /// <summary>
    /// Check data data access event
    /// </summary>
    [Serializable]
    public class CheckDataEvent : BaseDataAccessEvent
    {
        public CheckDataEvent()
        {
            EventType = DataAccessEventType.CheckData;
        }

        /// <summary>
        /// Gets or sets the query object
        /// </summary>
        public IQuery Query { get; set; }

        /// <summary>
        /// Gets or sets whether has value
        /// </summary>
        public bool HasValue { get; set; }
    }
}
