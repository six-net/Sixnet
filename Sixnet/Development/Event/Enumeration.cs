using System;

namespace Sixnet.Development.Event
{
    /// <summary>
    /// Defines event trigger time
    /// </summary>
    [Serializable]
    public enum EventTriggerTime
    {
        Immediately = 2,
        WorkCompleted = 4
    }
}
