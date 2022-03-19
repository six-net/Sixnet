using System;

namespace EZNEW.Development.Domain.Event
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

    /// <summary>
    /// Defines domain event result code
    /// </summary>
    [Serializable]
    public enum DomainEventExecuteResultCode
    {
        Empty = 0,
        Success = 20000
    }
}
