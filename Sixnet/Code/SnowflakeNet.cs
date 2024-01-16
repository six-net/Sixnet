using System;
using Sixnet.Exceptions;

namespace Sixnet.Code
{
    /// <summary>
    /// Snowflake algorithm implement by dotnet
    /// </summary>
    internal class SnowflakeNet
    {
        readonly long twepoch = 106686661L;
        readonly DateTimeOffset startDate = new(2023, 01, 01, 0, 0, 0, TimeSpan.Zero);
        const int workerIdBits = 5;
        const int datacenterIdBits = 5;
        const int sequenceBits = 12;
        const long maxWorkerId = -1L ^ (-1L << workerIdBits);
        const long maxDatacenterId = -1L ^ (-1L << datacenterIdBits);
        private const int workerIdShift = sequenceBits;
        private const int datacenterIdShift = sequenceBits + workerIdBits;
        public const int timestampLeftShift = sequenceBits + workerIdBits + datacenterIdBits;
        private const long sequenceMask = -1L ^ (-1L << sequenceBits);
        private long lastTimestamp = -1L;
        readonly object lockObj = new();

        /// <summary>
        /// Initializes a new instance of the EZNEW.Code.SnowflakeNet class
        /// </summary>
        /// <param name="dataCenterId">Data center id(1-31)</param>
        /// <param name="workerId">Worker id(1-31)</param>
        /// <param name="sequence">Sequence</param>
        /// <param name="startDate">Start date</param>
        public SnowflakeNet(long dataCenterId, long workerId, long sequence = 0L, DateTimeOffset? startDate = null)
        {
            WorkerId = workerId;
            DataCenterId = dataCenterId;
            Sequence = sequence;

            ThrowHelper.ThrowArgErrorIf(workerId > maxWorkerId || workerId < 0, string.Format("Worker Id can't be greater than {0} or less than 0", maxWorkerId));
            ThrowHelper.ThrowArgErrorIf(dataCenterId > maxDatacenterId || dataCenterId < 0, string.Format("Data Center Id can't be greater than {0} or less than 0", maxDatacenterId));

            if (startDate.HasValue)
            {
                var nowDate = DateTimeOffset.UtcNow;

                ThrowHelper.ThrowArgErrorIf(nowDate <= startDate, $"{nameof(startDate)} must less than DateTimeOffset.UtcNow");

                this.startDate = startDate.Value;
                twepoch = (long)(nowDate - startDate.Value).TotalMilliseconds;
            }
        }

        /// <summary>
        /// Gets the worker id
        /// </summary>
        public long WorkerId { get; } = 0L;

        /// <summary>
        /// Gets the data center id
        /// </summary>
        public long DataCenterId { get; } = 0L;

        /// <summary>
        /// Gets the sequence
        /// </summary>
        public long Sequence { get; private set; } = 0L;

        /// <summary>
        /// Generates a new id
        /// </summary>
        /// <returns>Return a new id</returns>
        public virtual long GenerateId()
        {
            lock (lockObj)
            {
                var timestamp = TimeGen();

                ThrowHelper.ThrowInvalidOperationIf(timestamp < lastTimestamp, "Time is return back,generated fail");

                if (lastTimestamp == timestamp)
                {
                    Sequence = (Sequence + 1) & sequenceMask;
                    if (Sequence == 0)
                    {
                        timestamp = TilNextMillis(lastTimestamp);
                    }
                }
                else
                {
                    Sequence = 0;
                }
                lastTimestamp = timestamp;
                var id = ((timestamp - twepoch) << timestampLeftShift) |
                         (DataCenterId << datacenterIdShift) |
                         (WorkerId << workerIdShift) | Sequence;

                return id;
            }
        }

        /// <summary>
        /// Generates a new id by timestamp
        /// </summary>
        /// <param name="timestamp">Timestamp</param>
        /// <returns>Return a new id</returns>
        public virtual long GenerateIdByTime(long timestamp)
        {
            lock (lockObj)
            {
                ThrowHelper.ThrowInvalidOperationIf(timestamp < lastTimestamp, "Time is return back,generated fail");

                if (lastTimestamp == timestamp)
                {
                    Sequence = (Sequence + 1) & sequenceMask;
                    if (Sequence == 0)
                    {
                        timestamp = TilNextMillis(lastTimestamp);
                    }
                }
                else
                {
                    Sequence = 0;
                }
                lastTimestamp = timestamp;
                var id = ((timestamp - twepoch) << timestampLeftShift) |
                         (DataCenterId << datacenterIdShift) |
                         (WorkerId << workerIdShift) | Sequence;

                return id;
            }
        }

        /// <summary>
        /// Wait next millis
        /// </summary>
        /// <param name="lastTimestamp">Last timestamp</param>
        /// <returns>Return the next mills</returns>
        protected virtual long TilNextMillis(long lastTimestamp)
        {
            var timestamp = TimeGen();
            while (timestamp <= lastTimestamp)
            {
                timestamp = TimeGen();
            }
            return timestamp;
        }

        /// <summary>
        /// Return time
        /// </summary>
        /// <returns></returns>
        public virtual long TimeGen()
        {
            return (long)(DateTimeOffset.UtcNow - startDate).TotalMilliseconds;
        }
    }
}
