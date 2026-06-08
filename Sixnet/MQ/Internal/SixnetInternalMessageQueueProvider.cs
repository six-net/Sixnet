// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Exceptions;

namespace Sixnet.MQ.Internal
{
    /// <summary>
    /// Internal message queue provider
    /// </summary>
    public class SixnetInternalMessageQueueProvider : ISixnetMessageQueueProvider
    {
        /// <summary>
        /// Add queue
        /// </summary>
        /// <param name="parameter">Add queue parameter</param>
        /// <returns></returns>
        public Task AddQueueAsync(SixnetAddQueueParameter parameter)
        {
            AddQueue(parameter);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Add queue
        /// </summary>
        /// <param name="parameter">Add queue parameter</param>
        /// <returns></returns>
        public void AddQueue(SixnetAddQueueParameter parameter)
        {
            SixnetInternalQueueManager.AddQueues(parameter?.Queues?.Select(c => c.Name).ToList());
        }

        /// <summary>
        /// Delete queue
        /// </summary>
        /// <param name="parameter">Delete queue parameter</param>
        /// <returns></returns>
        public Task DeleteQueueAsync(SixnetDeleteQueueParameter parameter)
        {
            DeleteQueue(parameter);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Delete queue
        /// </summary>
        /// <param name="parameter">Delete queue parameter</param>
        /// <returns></returns>
        public void DeleteQueue(SixnetDeleteQueueParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter == null, nameof(parameter));

            switch (parameter.Scope)
            {
                case SixnetQueueScope.Server:
                    SixnetInternalQueueManager.DeleteAllQueue();
                    break;
                case SixnetQueueScope.Queues:
                    SixnetInternalQueueManager.DeleteQueue(parameter.QueueNames);
                    break;
            }
        }

        /// <summary>
        /// Enqueue
        /// </summary>
        /// <param name="parameter">Enqueue parameter</param>
        /// <returns></returns>
        public Task EnqueueAsync(SixnetEnqueueParameter parameter)
        {
            Enqueue(parameter);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Enqueue
        /// </summary>
        /// <param name="parameter">Enqueue parameter</param>
        /// <returns></returns>
        public void Enqueue(SixnetEnqueueParameter parameter)
        {
            if ((parameter?.Endpoint?.QueueNames.IsNullOrEmpty() ?? true)
            || (parameter?.Message == null))
            {
                return;
            }
            foreach (var queueName in parameter.Endpoint.QueueNames)
            {
                var queue = SixnetInternalQueueManager.GetQueue(queueName);
                if (queue == null && SixnetMQ.GetMessageQueueOptions().AutoCreateInternalQueue)
                {
                    SixnetInternalQueueManager.AddQueue(queueName);
                    queue = SixnetInternalQueueManager.GetQueue(queueName);
                }
                SixnetDirectThrower.ThrowSixnetExceptionIf(queue == null, $"Not found queue:{queueName}");
                queue?.Enqueue(parameter.Message);
            }
        }

        /// <summary>
        /// Consume
        /// </summary>
        /// <param name="parameter">Consume parameter</param>
        /// <returns></returns>
        public Task ConsumeAsync(SixnetConsumeParameter parameter)
        {
            Consume(parameter);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Consume
        /// </summary>
        /// <param name="parameter">Consume parameter</param>
        /// <returns></returns>
        public void Consume(SixnetConsumeParameter parameter)
        {
            if (parameter?.Queues.IsNullOrEmpty() ?? true)
            {
                return;
            }
            foreach (var queueEntry in parameter.Queues)
            {
                var queue = SixnetInternalQueueManager.GetQueue(queueEntry.QueueName);
                queue?.Consume(queueEntry.Count);
            }
        }

        /// <summary>
        /// Abort consume
        /// </summary>
        /// <param name="parameter">Abort consume parameter</param>
        public Task AbortConsumeAsync(SixnetAbortConsumeParameter parameter)
        {
            AbortConsume(parameter);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Abort consume
        /// </summary>
        /// <param name="parameter">Abort consume parameter</param>
        public void AbortConsume(SixnetAbortConsumeParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter == null, nameof(parameter));

            switch (parameter.Scope)
            {
                case SixnetQueueScope.Server:
                    SixnetInternalQueueManager.AbortAllConsume();
                    break;
                case SixnetQueueScope.Queues:
                    SixnetInternalQueueManager.AbortConsume(parameter.QueueNames);
                    break;
            }
        }
    }
}
