using EZNEW.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace EZNEW.Queue
{
    /// <summary>
    /// Internal queue manager
    /// </summary>
    public class InternalQueue
    {
        private readonly Channel<IInternalQueueItem> Queue = null;

        private readonly ChannelWriter<IInternalQueueItem> Writer = null;

        private readonly ChannelReader<IInternalQueueItem> Reader = null;

        /// <summary>
        /// Gets the queue name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Whether is start consume
        /// </summary>
        private bool StartConsume = false;

        /// <summary>
        /// Consume token source
        /// </summary>
        private CancellationTokenSource consumeTokenSource = null;

        private InternalQueue(string name)
        {
            Queue = Channel.CreateUnbounded<IInternalQueueItem>();
            Writer = Queue.Writer;
            Reader = Queue.Reader;
            Name = name;
        }

        public static InternalQueue Create(string name)
        {
            return new InternalQueue(name);
        }

        #region Enqueue

        /// <summary>
        /// Enqueue items
        /// </summary>
        /// <param name="items">Items</param>
        public void Enqueue(params IInternalQueueItem[] items)
        {
            IEnumerable<IInternalQueueItem> itemCollection = items;
            Enqueue(itemCollection);
        }

        /// <summary>
        /// Enqueue items
        /// </summary>
        /// <param name="items">Items</param>
        public void Enqueue(IEnumerable<IInternalQueueItem> items)
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
                    Writer.TryWrite(item);
                }
                catch (Exception ex)
                {
                    LogManager.LogError(ex, ex.Message);
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
                            LogManager.LogInformation(typeof(InternalQueue), $"[Internal Queue {Name}] Opened a new consumer at {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss}");
                        }
                    }
                }
            }
        }

        async Task ConsumeAction(CancellationToken token)
        {
            bool IsStopConsume() => !StartConsume || token.IsCancellationRequested;
            Type itemType = null;
            while (!IsStopConsume())
            {
                try
                {
                    var item = await Reader.ReadAsync(token).ConfigureAwait(false);
                    if (item != null)
                    {
                        itemType = item.GetType();
                        item.Execute();
                    }
                }
                catch (Exception ex)
                {
                    LogManager.LogError(itemType, ex, ex.Message);
                }
            }
            StartConsume = false;
            LogManager.LogInformation(typeof(InternalQueue), $"[Internal Queue {Name}] Closed a consumer at {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss}");
        }

        #endregion
    }
}
