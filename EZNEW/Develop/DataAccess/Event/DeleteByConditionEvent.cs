using System;
using EZNEW.Develop.CQuery;

namespace EZNEW.Develop.DataAccess.Event
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
