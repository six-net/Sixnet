using System;

namespace EZNEW.Development.DataAccess.Event
{
    /// <summary>
    /// Defines data access event type
    /// </summary>
    [Serializable]
    public enum DataAccessEventType
    {
        AddData = 2,
        ModifyData = 3,
        ModifyExpression = 4,
        DeleteData = 5,
        DeleteByCondition = 6,
        QueryData = 7,
        AggregateFunction = 8,
        CheckData = 9
    }
}
