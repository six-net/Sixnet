using EZNEW.Develop.Domain.Aggregation;
using EZNEW.Develop.UnitOfWork;
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
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        /// <returns></returns>
        public IRepositoryEventHandleResult Execute(IEnumerable<T> datas, ActivationOption activationOption = null)
        {
            if (datas.IsNullOrEmpty() || Operation == null)
            {
                return DataOperationEventResult.Empty;
            }
            Operation(datas, activationOption);
            return DataOperationEventResult.Empty;
        }
    }
}
