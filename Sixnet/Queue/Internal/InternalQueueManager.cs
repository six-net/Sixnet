using System.Collections.Generic;
using Sixnet.Constants;

namespace Sixnet.Queue.Internal
{
    /// <summary>
    /// Internal queue manager
    /// </summary>
    public static class InternalQueueManager
    {
        static Dictionary<string, InternalQueue> Queues = null;

        /// <summary>
        /// Gets the default internal queue
        /// </summary>
        public static readonly InternalQueue Default = InternalQueue.Create("SIXNET_DEFAULT");

        /// <summary>
        /// Whether to turn on the consumer by default
        /// Default is true
        /// </summary>
        public static bool AutoOpenConsumer = true;

        static InternalQueueManager()
        {
            Reset();
        }

        /// <summary>
        /// Add a new internal queue
        /// No new queues are created when they already exist
        /// </summary>
        /// <param name="name">Queue name</param>
        public static void AddQueue(string name)
        {
            if (!Queues.ContainsKey(name))
            {
                Queues[name] = InternalQueue.Create(name);
            }
        }

        /// <summary>
        /// Add a internal queue for data cache
        /// </summary>
        public static void AddDataCacheQueue()
        {
            AddQueue(InternalQueueNames.DataCache);
        }

        /// <summary>
        /// Add a internal queue for logging
        /// </summary>
        public static void AddLoggingQueue()
        {
            AddQueue(InternalQueueNames.Logging);
        }

        /// <summary>
        /// Add a interal queue for template queue
        /// </summary>
        public static void AddTemplateMessageQueue()
        {
            AddQueue(InternalQueueNames.Message);
        }

        /// <summary>
        /// Remove internal queue
        /// </summary>
        /// <param name="names"></param>
        public static void RemoveQueue(params string[] names)
        {
            if (names.IsNullOrEmpty())
            {
                return;
            }
            foreach (var name in names)
            {
                if (Queues.ContainsKey(name))
                {
                    Queues.Remove(name);
                }
            }
        }

        /// <summary>
        /// Reset internal queue configuration
        /// </summary>
        public static void Reset()
        {
            Queues = new Dictionary<string, InternalQueue>();
        }

        /// <summary>
        /// Gets the queue with the specified name
        /// Returnt the default queue if not add the queue with the specified name
        /// </summary>
        /// <param name="name">Queue name</param>
        /// <returns>Return the internal queue</returns>
        public static InternalQueue GetQueue(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || !Queues.ContainsKey(name))
            {
                return Default;
            }
            return Queues[name] ?? Default;
        }
    }
}
