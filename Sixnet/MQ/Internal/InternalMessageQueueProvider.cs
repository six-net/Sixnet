using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sixnet.Exceptions;

namespace Sixnet.MQ.InProcess
{
    /// <summary>
    /// Internal message queue provider
    /// </summary>
    public class InternalMessageQueueProvider : ISixnetMessageQueueProvider
    {
        /// <summary>
        /// Add queue
        /// </summary>
        /// <param name="parameter">Add queue parameter</param>
        /// <returns></returns>
        public Task AddQueueAsync(AddQueueParameter parameter)
        {
            AddQueue(parameter);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Add queue
        /// </summary>
        /// <param name="parameter">Add queue parameter</param>
        /// <returns></returns>
        public void AddQueue(AddQueueParameter parameter)
        {
            InternalQueueManager.AddQueues(parameter?.Queues?.Select(c => c.Name).ToList());
        }

        /// <summary>
        /// Delete queue
        /// </summary>
        /// <param name="parameter">Delete queue parameter</param>
        /// <returns></returns>
        public Task DeleteQueueAsync(DeleteQueueParameter parameter)
        {
            DeleteQueue(parameter);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Delete queue
        /// </summary>
        /// <param name="parameter">Delete queue parameter</param>
        /// <returns></returns>
        public void DeleteQueue(DeleteQueueParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter == null, nameof(parameter));

            switch (parameter.Scope)
            {
                case QueueScope.Server:
                    InternalQueueManager.DeleteAllQueue();
                    break;
                case QueueScope.Queues:
                    InternalQueueManager.DeleteQueue(parameter.QueueNames);
                    break;
            }
        }

        /// <summary>
        /// Enqueue
        /// </summary>
        /// <param name="parameter">Enqueue parameter</param>
        /// <returns></returns>
        public Task EnqueueAsync(EnqueueParameter parameter)
        {
            Enqueue(parameter);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Enqueue
        /// </summary>
        /// <param name="parameter">Enqueue parameter</param>
        /// <returns></returns>
        public void Enqueue(EnqueueParameter parameter)
        {
            if ((parameter?.Endpoint?.QueueNames.IsNullOrEmpty() ?? true)
            || (parameter?.Message == null))
            {
                return;
            }
            foreach (var queueName in parameter.Endpoint.QueueNames)
            {
                var queue = InternalQueueManager.GetQueue(queueName);
                if (queue == null && SixnetMQ.GetMessageQueueOptions().AutoCreateInternalQueue)
                {
                    InternalQueueManager.AddQueue(queueName);
                    queue = InternalQueueManager.GetQueue(queueName);
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
        public Task ConsumeAsync(ConsumeParameter parameter)
        {
            Consume(parameter);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Consume
        /// </summary>
        /// <param name="parameter">Consume parameter</param>
        /// <returns></returns>
        public void Consume(ConsumeParameter parameter)
        {
            if (parameter?.Queues.IsNullOrEmpty() ?? true)
            {
                return;
            }
            foreach (var queueEntry in parameter.Queues)
            {
                var queue = InternalQueueManager.GetQueue(queueEntry.QueueName);
                queue?.Consume(queueEntry.Count);
            }
        }

        /// <summary>
        /// Abort consume
        /// </summary>
        /// <param name="parameter">Abort consume parameter</param>
        public Task AbortConsumeAsync(AbortConsumeParameter parameter)
        {
            AbortConsume(parameter);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Abort consume
        /// </summary>
        /// <param name="parameter">Abort consume parameter</param>
        public void AbortConsume(AbortConsumeParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter == null, nameof(parameter));

            switch (parameter.Scope)
            {
                case QueueScope.Server:
                    InternalQueueManager.AbortAllConsume();
                    break;
                case QueueScope.Queues:
                    InternalQueueManager.AbortConsume(parameter.QueueNames);
                    break;
            }
        }
    }
}
