using System.Collections.Generic;

namespace Sixnet.MQ
{
    /// <summary>
    /// Internal queue manager
    /// </summary>
    internal static class InProcessQueueManager
    {
        static Dictionary<string, InProcessQueue> Queues = null;

        /// <summary>
        /// Gets the default internal queue
        /// </summary>
        public static readonly InProcessQueue Default = InProcessQueue.Create("SIXNET_DEFAULT_IN_PROCESS_QUEUE");

        /// <summary>
        /// Whether to turn on the consumer by default
        /// Default is true
        /// </summary>
        public static bool AutoOpenConsumer = true;

        static InProcessQueueManager()
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
                Queues[name] = InProcessQueue.Create(name);
            }
        }

        /// <summary>
        /// Add a internal queue for data cache
        /// </summary>
        public static void AddDataCacheQueue()
        {
            AddQueue(SixnetMQ.InProcessQueueNames.DataCache);
        }

        /// <summary>
        /// Add a internal queue for logging
        /// </summary>
        public static void AddLoggingQueue()
        {
            AddQueue(SixnetMQ.InProcessQueueNames.Logging);
        }

        /// <summary>
        /// Add a interal queue for template queue
        /// </summary>
        public static void AddTemplateMessageQueue()
        {
            AddQueue(SixnetMQ.InProcessQueueNames.Message);
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
            Queues = new Dictionary<string, InProcessQueue>();
        }

        /// <summary>
        /// Gets the queue with the specified name
        /// Returnt the default queue if not add the queue with the specified name
        /// </summary>
        /// <param name="name">Queue name</param>
        /// <returns>Return the internal queue</returns>
        public static InProcessQueue GetQueue(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || !Queues.ContainsKey(name))
            {
                return Default;
            }
            return Queues[name] ?? Default;
        }
    }
}
