using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;

namespace Sixnet.Development.Data.Event
{
    /// <summary>
    /// Checked data event
    /// </summary>
    public class CheckedDataEvent : BaseDataEvent
    {
        public CheckedDataEvent()
        {
            EventType = DataEventType.Checked;
        }

        /// <summary>
        /// Whether has value
        /// </summary>
        public bool HasValue { get; set; }

        public static CheckedDataEvent Create(IDataClient dataClient, DataCommand command, bool hasValue)
        {
            var dataEvent = Create<CheckedDataEvent>(dataClient, command);
            dataEvent.HasValue = hasValue;
            return dataEvent;
        }
    }
}
