using System;
using EZNEW.Develop.Command;
using EZNEW.Develop.CQuery;

namespace EZNEW.Develop.DataAccess.Event
{
    /// <summary>
    /// Aggregate function data event
    /// </summary>
    [Serializable]
    public class AggregateFunctionEvent : BaseDataAccessEvent
    {
        public AggregateFunctionEvent()
        {
            EventType = DataAccessEventType.AggregateFunction;
        }

        /// <summary>
        /// Gets or sets the operate type
        /// </summary>
        public OperateType OperateType { get; set; }

        /// <summary>
        /// Gets or sets the query object
        /// </summary>
        public IQuery Query { get; set; }

        /// <summary>
        /// Gets or sets the result value
        /// </summary>
        public dynamic Value { get; set; }
    }
}
