using Microsoft.Extensions.Logging;
using Sixnet.Logging;
using Sixnet.Serialization.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Sixnet.MQ.InProcess
{
    /// <summary>
    /// Internal queue
    /// </summary>
    public class InternalQueue
    {
        #region Fields

        readonly Channel<SixnetQueueMessage> _queue = null;
        readonly ChannelWriter<SixnetQueueMessage> _writer = null;
        readonly ChannelReader<SixnetQueueMessage> _reader = null;
        CancellationTokenSource _consumeTokenSource = null;
        int _consumerCount = 0;

        /// <summary>
        /// Gets the queue name
        /// </summary>
        public string Name { get; private set; }

        #endregion

        #region Constructor

        private InternalQueue(string name)
        {
            _queue = Channel.CreateUnbounded<SixnetQueueMessage>();
            _writer = _queue.Writer;
            _reader = _queue.Reader;
            Name = name;
        }

        #endregion

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
        /// Enqueue messages
        /// </summary>
        /// <param name="messages">Messages</param>
        public void Enqueue(params SixnetQueueMessage[] messages)
        {
            IEnumerable<SixnetQueueMessage> itemCollection = messages;
            Enqueue(itemCollection);
        }

        /// <summary>
        /// Enqueue messages
        /// </summary>
        /// <param name="messages">Messages</param>
        public void Enqueue(IEnumerable<SixnetQueueMessage> messages)
        {
            if (messages.IsNullOrEmpty())
            {
                return;
            }
            foreach (var item in messages)
            {
                try
                {
                    if (item == null)
                    {
                        return;
                    }
                    if (!_writer.TryWrite(item))
                    {
                        SixnetLogger.LogProvider?.WriteLog(typeof(InternalQueue).FullName, LogLevel.Information, SixnetLogEvents.Framework.InternalQueueEnqueueError
                            , null, $"Internal queue write failure: {SixnetJsonSerializer.Serialize(item)}");
                    }
                }
                catch (Exception ex)
                {
                    SixnetLogger.LogProvider?.WriteLog(typeof(InternalQueue).FullName, LogLevel.Error, SixnetLogEvents.Framework.InternalQueueEnqueueError, ex, ex.Message);
                }
            }
        }

        #endregion

        #region Consume

        /// <summary>
        /// Abort consume
        /// </summary>
        public void AbortConsume()
        {
            if (_consumerCount > 0)
            {
                lock (this)
                {
                    if (_consumerCount > 0)
                    {
                        _consumerCount = 0;
                        _consumeTokenSource?.Cancel();
                    }
                }
            }
        }

        /// <summary>
        /// Consume
        /// </summary>
        /// <param name="count">Consumer count</param>
        public void Consume(int count = 1)
        {
            if (count > 0)
            {
                lock (this)
                {
                    _consumeTokenSource ??= new CancellationTokenSource();
                    for (int i = 0; i < count; i++)
                    {
                        ThreadPool.QueueUserWorkItem(async s => await ConsumeAction(_consumeTokenSource.Token).ConfigureAwait(false));
                        SixnetLogger.LogProvider?.WriteLog(typeof(InternalQueue).FullName, LogLevel.Information, SixnetLogEvents.Framework.InternalQueueStartConsumer
                            , null, $"Open a new consumer at {DateTimeOffset.Now} for: {Name}");
                        _consumerCount++;
                    }
                }
            }
        }

        /// <summary>
        /// Consume action
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        async Task ConsumeAction(CancellationToken token)
        {
            bool IsStopConsume() => token.IsCancellationRequested;
            while (!IsStopConsume())
            {
                try
                {
                    var item = await _reader.ReadAsync(token).ConfigureAwait(false);
                    await SixnetMQ.HandleMessageAsync(item).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    SixnetLogger.LogError<InternalQueue>(SixnetLogEvents.Framework.InternalQueueConsumeError, ex, ex.Message);
                }
            }
            SixnetLogger.LogProvider?.WriteLog(typeof(InternalQueue).FullName, LogLevel.Information, SixnetLogEvents.Framework.InternalQueueCloseConsumer
                , null, $"[Internal Queue {Name}] Closed a consumer at {DateTimeOffset.Now} for: {Name}");
        }

        #endregion

        #region Release

        /// <summary>
        /// Release
        /// </summary>
        public void Release()
        {
            AbortConsume();
            _writer?.TryComplete();
        }

        #endregion
    }
}
