using EZNEW.Develop.Domain.Aggregation;
using EZNEW.Framework.Extension;
using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Domain.Repository.Event
{
    public class QueryEventHandler<T> : IRepositoryEventHandler
    {
        /// <summary>
        /// event type
        /// </summary>
        public EventType EventType { get; set; }

        /// <summary>
        /// handler repository type
        /// </summary>
        public Type HandlerRepositoryType { get; set; }

        /// <summary>
        /// object type
        /// </summary>
        public Type ObjectType { get; set; }

        /// <summary>
        /// operation
        /// </summary>
        public QueryData<T> Operation { get; set; }

        /// <summary>
        /// execute
        /// </summary>
        /// <returns></returns>
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
