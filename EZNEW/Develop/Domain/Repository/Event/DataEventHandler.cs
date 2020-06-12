using System;
using System.Collections.Generic;
using EZNEW.Develop.Domain.Aggregation;
using EZNEW.Develop.UnitOfWork;

namespace EZNEW.Develop.Domain.Repository.Event
{
    /// <summary>
    /// Data operation event handler
    /// </summary>
    /// <typeparam name="T">data type</typeparam>
    public class DataEventHandler<T> : IRepositoryEventHandler
    {
        /// <summary>
        /// Gets or sets the data operation
        /// </summary>
        public DataOperation<T> Operation { get; set; }

        /// <summary>
        /// Gets or sets the event type
        /// </summary>
        public EventType EventType { get; set; }

        /// <summary>
        /// Gets or sets handler repository type
        /// </summary>
        public Type HandlerRepositoryType { get; set; }

        /// <summary>
        /// Gets or sets the object type
        /// </summary>
        public Type ObjectType { get; set; }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        /// <returns></returns>
        public IRepositoryEventExecuteResult Execute(IEnumerable<T> datas, ActivationOption activationOption = null)
        {
            if (datas.IsNullOrEmpty() || Operation == null)
            {
                return DataOperationEventExecuteResult.Empty;
            }
            Operation(datas, activationOption);
            return DataOperationEventExecuteResult.Empty;
        }
    }
}
