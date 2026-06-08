// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;

namespace Sixnet.Development.Events.Data
{
    /// <summary>
    /// Queried data event
    /// </summary>
    public class SixnetQueriedDataEvent<TData> : SixnetBaseDataEvent
    {
        public SixnetQueriedDataEvent()
        {
            EventType = SixnetDataEventType.Queried;
        }

        /// <summary>
        /// Gets or sets the queried data
        /// </summary>
        public IEnumerable<TData> ResultDatas { get; set; }

        public static SixnetQueriedDataEvent<TData> Create(ISixnetDataClient dataClient, SixnetDataCommand command, IEnumerable<TData> datas)
        {
            var dataEvent = Create<SixnetQueriedDataEvent<TData>>(dataClient, command);
            dataEvent.ResultDatas = datas;
            return dataEvent;
        }
    }
}
