using System;

namespace EZNEW.Development.Domain.Repository.Event
{
    /// <summary>
    /// Defines repository event type
    /// </summary>
    [Serializable]
    public enum EventType
    {
        SaveObject = 2,
        RemoveObject = 4,
        RemoveByCondition = 8,
        QueryData = 16,
        ModifyExpression = 32
    }
}
