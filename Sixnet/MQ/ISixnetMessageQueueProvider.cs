// "Company © 2025. All rights reserved."

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
        Task AddQueueAsync(SixnetAddQueueParameter parameter);

        /// <summary>
        /// Add queue
        /// </summary>
        /// <param name="parameter">Add queue parameter</param>
        /// <returns></returns>
        void AddQueue(SixnetAddQueueParameter parameter);

        /// <summary>
        /// Delete queue
        /// </summary>
        /// <param name="parameter">Delete queue parameter</param>
        /// <returns></returns>
        Task DeleteQueueAsync(SixnetDeleteQueueParameter parameter);

        /// <summary>
        /// Delete queue
        /// </summary>
        /// <param name="parameter">Delete queue parameter</param>
        /// <returns></returns>
        void DeleteQueue(SixnetDeleteQueueParameter parameter);

        /// <summary>
        /// Enqueue
        /// </summary>
        /// <param name="parameter">Enqueue parameter</param>
        /// <returns></returns>
        Task EnqueueAsync(SixnetEnqueueParameter parameter);

        /// <summary>
        /// Enqueue
        /// </summary>
        /// <param name="parameter">Enqueue parameter</param>
        /// <returns></returns>
        void Enqueue(SixnetEnqueueParameter parameter);

        /// <summary>
        /// Consume
        /// </summary>
        /// <param name="parameter">Consume parameter</param>
        /// <returns></returns>
        Task ConsumeAsync(SixnetConsumeParameter parameter);

        /// <summary>
        /// Consume
        /// </summary>
        /// <param name="parameter">Consume parameter</param>
        /// <returns></returns>
        void Consume(SixnetConsumeParameter parameter);

        /// <summary>
        /// Abort consume
        /// </summary>
        /// <param name="parameter">Abort consume parameter</param>
        Task AbortConsumeAsync(SixnetAbortConsumeParameter parameter);

        /// <summary>
        /// Abort consume
        /// </summary>
        /// <param name="parameter">Abort consume parameter</param>
        void AbortConsume(SixnetAbortConsumeParameter parameter);
    }
}
