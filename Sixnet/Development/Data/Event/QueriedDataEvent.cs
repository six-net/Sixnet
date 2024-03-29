﻿using System.Collections.Generic;
using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;

namespace Sixnet.Development.Data.Event
{
    /// <summary>
    /// Queried data event
    /// </summary>
    public class QueriedDataEvent<TData> : BaseDataEvent
    {
        public QueriedDataEvent()
        {
            EventType = DataEventType.Queried;
        }

        /// <summary>
        /// Gets or sets the queried data
        /// </summary>
        public IEnumerable<TData> ResultDatas { get; set; }

        public static QueriedDataEvent<TData> Create(IDataClient dataClient, DataCommand command, IEnumerable<TData> datas)
        {
            var dataEvent = Create<QueriedDataEvent<TData>>(dataClient, command);
            dataEvent.ResultDatas = datas;
            return dataEvent;
        }
    }
}
