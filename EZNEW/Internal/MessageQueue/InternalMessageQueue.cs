using EZNEW.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace EZNEW.Internal.MessageQueue
{
    /// <summary>
    /// Internal message queue
    /// </summary>
    public static class InternalMessageQueue
    {
        #region Fields

        /// <summary>
        /// Message command collection
        /// </summary>
        private static readonly BlockingCollection<IInternalMessageQueueCommand> MessageCommandCollection = null;

        /// <summary>
        /// Add consumer lock
        /// </summary>
        private static readonly object AddConsumerLock = new object();

        /// <summary>
        /// Start default consumer lock
        /// </summary>
        private static readonly object StartDefaultConsumerLock = new object();

        /// <summary>
        /// Consume action
        /// </summary>
        private static readonly Action<CancellationToken> ConsumeAction = null;

        /// <summary>
        /// Current consumer count
        /// </summary>
        private static int CurrentConsumerCount = 0;

        /// <summary>
        /// Default cancellation token source
        /// </summary>
        private static readonly CancellationTokenSource DefaultCancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Whether is stop consume
        /// </summary>
        private static bool StopConsume = false;

        /// <summary>
        /// Whether is started default consumer
        /// </summary>
        private static bool StartedDefaultConsumer = false;

        /// <summary>
        /// Max queued messages
        /// </summary>
        private const int MaxQueuedMessages = int.MaxValue;

        /// <summary>
        /// Gets or sets the maximum consumer count
        /// Default value is 1
        /// </summary>
        public static int ConsumerMaximum { get; set; } = 1;

        /// <summary>
        /// Gets or sets the number of milliseconds to wait for add, or System.Threading.Timeout.Infinite (-1)
        /// to wait indefinitely
        /// </summary>
        public static int AddMillisecondsTimeout { get; set; } = -1;

        /// <summary>
        /// Gets or sets the number of milliseconds to wait for take, or System.Threading.Timeout.Infinite (-1)
        /// to wait indefinitely
        /// </summary>
        public static int TakeMillisecondsTimeout { get; set; } = -1;

        #endregion

        static InternalMessageQueue()
        {
            MessageCommandCollection = new BlockingCollection<IInternalMessageQueueCommand>(MaxQueuedMessages);
            ConsumeAction = token =>
            {
                bool IsStopConsume() => StopConsume || token.IsCancellationRequested || MessageCommandCollection.IsCompleted;
                Type commandType = null;
                while (!IsStopConsume())
                {
                    try
                    {
                        if (MessageCommandCollection.TryTake(out var cmd, TakeMillisecondsTimeout) && cmd != null)
                        {
                            commandType = cmd.GetType();
                            cmd.Run();
                        }
                    }
                    catch (Exception ex)
                    {
                        LogManager.LogError(commandType, ex, ex.Message);
                    }
                }
                Interlocked.Decrement(ref CurrentConsumerCount);
            };
        }

        #region Enqueue

        /// <summary>
        /// Enqueue message command
        /// </summary>
        /// <param name="commands">Commands</param>
        public static void Enqueue(params IInternalMessageQueueCommand[] commands)
        {
            if (commands.IsNullOrEmpty())
            {
                return;
            }
            AddDefaultConsumer();
            foreach (var cmd in commands)
            {
                try
                {
                    MessageCommandCollection.TryAdd(cmd, AddMillisecondsTimeout);
                }
                catch (Exception ex)
                {
                    LogManager.LogError(ex, ex.Message);
                }
            }
        }

        #endregion

        #region Add consumer

        /// <summary>
        /// Add message consumer
        /// </summary>
        /// <param name="consumerCount">Start new consumer count(limited by 'ConsumerMaximum')</param>
        public static void AddConsumer(int consumerCount = 1)
        {
            AddConsumer(DefaultCancellationTokenSource.Token, consumerCount);
        }

        /// <summary>
        /// Add message consumer
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="consumerCount">Start new consumer count(limited by 'ConsumerMaximum')</param>
        public static void AddConsumer(CancellationToken cancellationToken, int consumerCount = 1)
        {
            lock (AddConsumerLock)
            {
                var minConsumerCount = Math.Min(consumerCount, ConsumerMaximum - CurrentConsumerCount);
                if (minConsumerCount < 1)
                {
                    return;
                }
                StopConsume = false;
                Interlocked.Add(ref CurrentConsumerCount, minConsumerCount);
                for (var i = 0; i < minConsumerCount; i++)
                {
                    var consumeThread = new Thread(() => ConsumeAction(cancellationToken))
                    {
                        IsBackground = true,
                        Name = $"Internal message queue cnsume thread:{Guid.NewGuid().ToInt64()}"
                    };
                    consumeThread.Start();
                }
            }
        }

        /// <summary>
        /// Add a default consumeer
        /// </summary>
        static void AddDefaultConsumer()
        {
            if (!StartedDefaultConsumer)
            {
                lock (StartDefaultConsumerLock)
                {
                    if (!StartedDefaultConsumer)
                    {
                        AddConsumer();
                        StartedDefaultConsumer = true;
                    }
                }
            }
        }

        #endregion

        #region Stop

        /// <summary>
        /// Stop all consumer
        /// </summary>
        public static void StopAllConsumer()
        {
            StopConsume = true;
        }

        #endregion
    }
}
