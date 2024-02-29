using Sixnet.Logging;
using Sixnet.Serialization;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Sixnet.MQ
{
    /// <summary>
    /// In process queue
    /// </summary>
    public class InProcessQueue
    {
        #region Fields

        private readonly Channel<InProcessQueueMessage> Queue = null;

        private readonly ChannelWriter<InProcessQueueMessage> Writer = null;

        private readonly ChannelReader<InProcessQueueMessage> Reader = null;

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

        #region Constructor

        private InProcessQueue(string name)
        {
            Queue = Channel.CreateUnbounded<InProcessQueueMessage>();
            Writer = Queue.Writer;
            Reader = Queue.Reader;
            Name = name;
        }

        #endregion

        #region Create

        /// <summary>
        /// Create a new internal queue
        /// </summary>
        /// <param name="name">Queue name</param>
        /// <returns></returns>
        public static InProcessQueue Create(string name)
        {
            return new InProcessQueue(name);
        }

        #endregion

        #region Enqueue

        /// <summary>
        /// Enqueue messages
        /// </summary>
        /// <param name="messages">Messages</param>
        public void Enqueue(params InProcessQueueMessage[] messages)
        {
            IEnumerable<InProcessQueueMessage> itemCollection = messages;
            Enqueue(itemCollection);
        }

        /// <summary>
        /// Enqueue messages
        /// </summary>
        /// <param name="messages">Messages</param>
        public void Enqueue(IEnumerable<InProcessQueueMessage> messages)
        {
            if (messages.IsNullOrEmpty())
            {
                return;
            }
            if (InProcessQueueManager.AutoOpenConsumer)
            {
                Start();
            }
            foreach (var item in messages)
            {
                try
                {
                    if (item == null)
                    {
                        return;
                    }
                    if (!Writer.TryWrite(item))
                    {
                        SixnetLogger.LogError<InProcessQueue>(SixnetLogEvents.Framework.InternalQueueEnqueueError, $"Internal queue failure: {SixnetJsonSerializer.Serialize(item)}");
                    }
                }
                catch (Exception ex)
                {
                    SixnetLogger.LogError<InProcessQueue>(SixnetLogEvents.Framework.InternalQueueEnqueueError, ex, ex.Message);
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
                            SixnetLogger.LogDebug<InProcessQueue>(SixnetLogEvents.Framework.InternalQueueStartConsumer, $"[Internal Queue {Name}] Opened a new consumer at {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss}");
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
                    SixnetLogger.LogError<InProcessQueue>(SixnetLogEvents.Framework.InternalQueueConsumeError, ex, ex.Message);
                }
            }
            StartConsume = false;
            SixnetLogger.LogDebug<InProcessQueue>(SixnetLogEvents.Framework.InternalQueueCloseConsumer, $"[Internal Queue {Name}] Closed a consumer at {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss}");
        }

        #endregion
    }
}
