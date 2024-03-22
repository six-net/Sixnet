using Sixnet.Threading.Locking;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sixnet.MQ.InProcess
{
    /// <summary>
    /// Internal queue manager
    /// </summary>
    internal static class InternalQueueManager
    {
        /// <summary>
        /// Queues
        /// </summary>
        static readonly ConcurrentDictionary<string, InternalQueue> _queues = new();

        /// <summary>
        /// Add a new internal queue
        /// No new queues are created when they already exist
        /// </summary>
        /// <param name="name">Queue name</param>
        public static void AddQueue(string name)
        {
            if (!_queues.ContainsKey(name))
            {
                var queueLock = SixnetLocker.GetCreateInternalQueueLock(name);
                try
                {
                    if (!_queues.ContainsKey(name))
                    {
                        var newQueue = InternalQueue.Create(name);
                        var mqOptions = SixnetMQ.GetMessageQueueOptions();
                        if(mqOptions.AutoConsumeInternalQueue)
                        {
                            newQueue.Consume();
                        }
                        _queues[name] = newQueue;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    queueLock?.Release();
                }
            }
        }

        /// <summary>
        /// Add queues
        /// </summary>
        /// <param name="names"></param>
        public static void AddQueues(IEnumerable<string> names)
        {
            if (!names.IsNullOrEmpty())
            {
                foreach (var name in names)
                {
                    AddQueue(name);
                }
            }
        }

        /// <summary>
        /// Add a internal queue for data cache
        /// </summary>
        public static void AddDataCacheQueue()
        {
            AddQueue(SixnetMQ.InternalQueueNames.DataCache);
        }

        /// <summary>
        /// Add a interal queue for template queue
        /// </summary>
        public static void AddTemplateMessageQueue()
        {
            AddQueue(SixnetMQ.InternalQueueNames.DomainMessage);
        }

        /// <summary>
        /// Delete all queue
        /// </summary>
        public static void DeleteAllQueue()
        {
            if (!_queues.IsNullOrEmpty())
            {
                foreach (var queue in _queues.Values)
                {
                    queue?.Release();
                }
                _queues.Clear();
            }
        }

        /// <summary>
        /// Delete queue
        /// </summary>
        /// <param name="queueNames"></param>
        public static void DeleteQueue(IEnumerable<string> queueNames)
        {
            if (!queueNames.IsNullOrEmpty())
            {
                foreach (var queueName in queueNames)
                {
                    if (_queues.ContainsKey(queueName))
                    {
                        _queues.TryRemove(queueName, out _);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the queue with the specified name
        /// Returnt the default queue if not add the queue with the specified name
        /// </summary>
        /// <param name="name">Queue name</param>
        /// <returns>Return the internal queue</returns>
        public static InternalQueue GetQueue(string name)
        {
            _queues.TryGetValue(name, out var queue);
            return queue;
        }

        /// <summary>
        /// Consume all
        /// </summary>
        /// <param name="count"></param>
        public static void ConsumeAll(int count = 1)
        {
            foreach (var queue in _queues.Values)
            {
                queue?.Consume(count);
            }
        }

        /// <summary>
        /// Consume queue
        /// </summary>
        /// <param name="queueNames"></param>
        /// <param name="count"></param>
        public static void Consume(IEnumerable<string> queueNames, int count = 1)
        {
            if (!queueNames.IsNullOrEmpty())
            {
                foreach (var queueName in queueNames)
                {
                    _queues.TryGetValue(queueName, out var queue);
                    queue?.Consume(count);
                }
            }

        }

        /// <summary>
        /// Abort all consume
        /// </summary>
        public static void AbortAllConsume()
        {
            foreach (var queue in _queues.Values)
            {
                queue?.AbortConsume();
            }
        }

        /// <summary>
        /// Abort queue consume
        /// </summary>
        /// <param name="queueNames"></param>
        public static void AbortConsume(IEnumerable<string> queueNames)
        {
            if (!queueNames.IsNullOrEmpty())
            {
                foreach (var queueName in queueNames)
                {
                    _queues.TryGetValue(queueName, out var queue);
                    queue?.AbortConsume();
                }
            }
        }
    }
}
