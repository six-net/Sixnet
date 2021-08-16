using System;
using EZNEW.Development.Query;

namespace EZNEW.Development.DataAccess.Event
{
    /// <summary>
    /// Delete by condition event
    /// </summary>
    [Serializable]
    public class DeleteByConditionEvent : BaseDataAccessEvent
    {
        public DeleteByConditionEvent()
        {
            EventType = DataAccessEventType.DeleteByCondition;
        }

        /// <summary>
        /// Gets or sets the query object
        /// </summary>
        public IQuery Query { get; set; }
    }
}
