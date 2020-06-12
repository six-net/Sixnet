using System;
using EZNEW.Develop.CQuery;

namespace EZNEW.Develop.DataAccess.Event
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
