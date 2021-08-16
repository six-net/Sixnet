using System;
using System.Collections.Generic;
using EZNEW.Development.Domain.Aggregation;

namespace EZNEW.Development.Domain.Repository.Event
{
    /// <summary>
    /// Query event handler
    /// </summary>
    /// <typeparam name="T">Data type</typeparam>
    public class QueryEventHandler<T> : IRepositoryEventHandler
    {
        /// <summary>
        /// Gets or sets event type
        /// </summary>
        public EventType EventType { get; set; }

        /// <summary>
        /// Gets or sets the handler repository type
        /// </summary>
        public Type HandlerRepositoryType { get; set; }

        /// <summary>
        /// Gets or sets the object type
        /// </summary>
        public Type ObjectType { get; set; }

        /// <summary>
        /// Gets or sets the operation
        /// </summary>
        public QueryData<T> Operation { get; set; }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns>Return datas</returns>
        public List<T> Execute(IEnumerable<T> datas)
        {
            if (datas.IsNullOrEmpty() || Operation == null)
            {
                return new List<T>(0);
            }
            return Operation(datas);
        }
    }
}
