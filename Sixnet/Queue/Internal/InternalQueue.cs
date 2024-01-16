using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Sixnet.Logging;
using Sixnet.Serialization;

namespace Sixnet.Queue.Internal
{
    /// <summary>
    /// Internal queue manager
    /// </summary>
    public class InternalQueue
    {
        #region Fields

        private readonly Channel<IInternalQueueTask> Queue = null;

        private readonly ChannelWriter<IInternalQueueTask> Writer = null;

        private readonly ChannelReader<IInternalQueueTask> Reader = null;

        /// <summary>
        /// Gets the queue name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Indicates whether is start consume
        /// </summary>
        private bool StartConsume = false;

        /// <summary>
        /// Consume token source
        /// </summary>
        private CancellationTokenSource consumeTokenSource = null; 

        #endregion

        private InternalQueue(string name)
        {
            Queue = Channel.CreateUnbounded<IInternalQueueTask>();
            Writer = Queue.Writer;
            Reader = Queue.Reader;
            Name = name;
        }

        #region Create

        /// <summary>
        /// Create a new internal queue
        /// </summary>
        /// <param name="name">Queue name</param>
        /// <returns></returns>
        public static InternalQueue Create(string name)
        {
            return new InternalQueue(name);
        } 

        #endregion

        #region Enqueue

        /// <summary>
        /// Enqueue items
        /// </summary>
        /// <param name="items">Items</param>
        public void Enqueue(params IInternalQueueTask[] items)
        {
            IEnumerable<IInternalQueueTask> itemCollection = items;
            Enqueue(itemCollection);
        }

        /// <summary>
        /// Enqueue items
        /// </summary>
        /// <param name="items">Items</param>
        public void Enqueue(IEnumerable<IInternalQueueTask> items)
        {
            if (items.IsNullOrEmpty())
            {
                return;
            }
            if (InternalQueueManager.AutoOpenConsumer)
            {
                Start();
            }
            foreach (var item in items)
            {
                try
                {
                    if (item == null)
                    {
                        return;
                    }
                    if (!Writer.TryWrite(item))
                    {
                        LogManager.LogError<InternalQueue>(FrameworkLogEvents.Framework.InternalQueueEnqueueError, $"Internal queue failure: {JsonSerializer.Serialize(item)}");
                    }
                }
                catch (Exception ex)
                {
                    LogManager.LogError<InternalQueue>(FrameworkLogEvents.Framework.InternalQueueEnqueueError, ex, ex.Message);
                }
            }
        }

        #endregion

        #region Consume

        /// <summary>
        /// Stop consume
        /// </summary>
        public void Stop()
        {
            StartConsume = false;
            consumeTokenSource?.Cancel();
        }

        /// <summary>
        /// Start consume
        /// </summary>
        public void Start()
        {
            if (!StartConsume)
            {
                lock (this)
                {
                    if (!StartConsume)
                    {
                        StartConsume = true;
                        consumeTokenSource = new CancellationTokenSource();
                        StartConsume = ThreadPool.QueueUserWorkItem(async s => await ConsumeAction(consumeTokenSource.Token));
                        if (StartConsume)
                        {
                            LogManager.LogDebug<InternalQueue>(FrameworkLogEvents.Framework.InternalQueueStartConsumer, $"[Internal Queue {Name}] Opened a new consumer at {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss}");
                        }
                    }
                }
            }
        }

        async Task ConsumeAction(CancellationToken token)
        {
            bool IsStopConsume() => !StartConsume || token.IsCancellationRequested;
            while (!IsStopConsume())
            {
                try
                {
                    var item = await Reader.ReadAsync(token).ConfigureAwait(false);
                    if (item != null)
                    {
                        var itemType = item.GetType();
                        item.Execute();
                    }
                }
                catch (Exception ex)
                {
                    LogManager.LogError<InternalQueue>(FrameworkLogEvents.Framework.InternalQueueConsumeError, ex, ex.Message);
                }
            }
            StartConsume = false;
            LogManager.LogDebug<InternalQueue>(FrameworkLogEvents.Framework.InternalQueueCloseConsumer, $"[Internal Queue {Name}] Closed a consumer at {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss}");
        }

        #endregion
    }
}
