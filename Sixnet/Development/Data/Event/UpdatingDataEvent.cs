﻿using System;
using System.Collections.Generic;
using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;

namespace Sixnet.Development.Data.Event
{
    /// <summary>
    /// Updating data event
    /// </summary>
    [Serializable]
    public class UpdatingDataEvent : BaseDataEvent
    {
        public UpdatingDataEvent()
        {
            EventType = DataEventType.Updating;
        }

        public static UpdatingDataEvent Create(IDataClient dataClient, DataCommand command)
        {
            return Create<UpdatingDataEvent>(dataClient, command);
        }
    }
}
