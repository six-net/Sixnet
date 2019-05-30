using EZNEW.Develop.Domain.Aggregation;
using EZNEW.Framework.Extension;
using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Domain.Repository.Event
{
    public class DataEventHandler<T> : IRepositoryEventHandler
    {
        /// <summary>
        /// operation
        /// </summary>
        public DataOperation<T> Operation { get; set; }

        /// <summary>
        /// event type
        /// </summary>
        public EventType EventType { get; set; }

        /// <summary>
        /// repository type
        /// </summary>
        public Type HandlerRepositoryType { get; set; }

        /// <summary>
        /// object type
        /// </summary>
        public Type ObjectType { get; set; }

        /// <summary>
        /// execute
        /// </summary>
        /// <returns></returns>
        public IRepositoryEventHandleResult Execute(IEnumerable<T> datas)
        {
            if (datas.IsNullOrEmpty() || Operation == null)
            {
                return DataOperationEventResult.Empty;
            }
            Operation(datas);
            return DataOperationEventResult.Empty;
        }
    }
}
