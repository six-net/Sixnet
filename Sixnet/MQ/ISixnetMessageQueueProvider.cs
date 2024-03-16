using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sixnet.MQ
{
    /// <summary>
    /// Defines message queue provider
    /// </summary>
    public interface ISixnetMessageQueueProvider
    {
        /// <summary>
        /// Add queue
        /// </summary>
        /// <param name="parameter">Add queue parameter</param>
        /// <returns></returns>
        Task AddQueueAsync(AddQueueParameter parameter);

        /// <summary>
        /// Add queue
        /// </summary>
        /// <param name="parameter">Add queue parameter</param>
        /// <returns></returns>
        void AddQueue(AddQueueParameter parameter);

        /// <summary>
        /// Delete queue
        /// </summary>
        /// <param name="parameter">Delete queue parameter</param>
        /// <returns></returns>
        Task DeleteQueueAsync(DeleteQueueParameter parameter);

        /// <summary>
        /// Delete queue
        /// </summary>
        /// <param name="parameter">Delete queue parameter</param>
        /// <returns></returns>
        void DeleteQueue(DeleteQueueParameter parameter);

        /// <summary>
        /// Enqueue
        /// </summary>
        /// <param name="parameter">Enqueue parameter</param>
        /// <returns></returns>
        Task EnqueueAsync(EnqueueParameter parameter);

        /// <summary>
        /// Enqueue
        /// </summary>
        /// <param name="parameter">Enqueue parameter</param>
        /// <returns></returns>
        void Enqueue(EnqueueParameter parameter);

        /// <summary>
        /// Consume
        /// </summary>
        /// <param name="parameter">Consume parameter</param>
        /// <returns></returns>
        Task ConsumeAsync(ConsumeParameter parameter);

        /// <summary>
        /// Consume
        /// </summary>
        /// <param name="parameter">Consume parameter</param>
        /// <returns></returns>
        void Consume(ConsumeParameter parameter);

        /// <summary>
        /// Abort consume
        /// </summary>
        /// <param name="parameter">Abort consume parameter</param>
        Task AbortConsumeAsync(AbortConsumeParameter parameter);

        /// <summary>
        /// Abort consume
        /// </summary>
        /// <param name="parameter">Abort consume parameter</param>
        void AbortConsume(AbortConsumeParameter parameter);
    }
}
