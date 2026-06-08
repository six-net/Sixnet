// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;

namespace Sixnet.Development.Events.Data
{
    /// <summary>
    /// Checking data event
    /// </summary>
    [Serializable]
    public class SixnetCheckingDataEvent : SixnetBaseDataEvent
    {
        public SixnetCheckingDataEvent()
        {
            EventType = SixnetDataEventType.Checking;
        }

        public static SixnetCheckingDataEvent Create(ISixnetDataClient dataClient, SixnetDataCommand command)
        {
            return Create<SixnetCheckingDataEvent>(dataClient, command);
        }
    }
}
