using System;
using EZNEW.Develop.CQuery;

namespace EZNEW.Develop.DataAccess.Event
{
    /// <summary>
    /// Query data event
    /// </summary>
    [Serializable]
    public class QueryDataEvent : BaseDataAccessEvent
    {
        public QueryDataEvent()
        {
            EventType = DataAccessEventType.QueryData;
        }

        /// <summary>
        /// Gets or sets the query object
        /// </summary>
        public IQuery Query { get; set; }

        /// <summary>
        /// Gets or sets the datas
        /// </summary>
        public dynamic Datas { get; set; }
    }
}
