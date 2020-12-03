using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Data.Cache
{
    /// <summary>
    /// Defines the data operation type
    /// </summary>
    public enum DataOperationType
    {
        Add = 1000,
        ModifyData = 1005,
        ModifyByCondition = 1010,
        Get = 1015,
        RemoveData = 1020,
        RemoveByCondition = 1025
    }

    /// <summary>
    /// Defines cache operation trigger time
    /// </summary>
    [Flags]
    public enum DataCacheOperationTriggerTime
    {
        Before = 2,
        After = 4
    }

    /// <summary>
    /// Defines cache operation
    /// </summary>
    [Flags]
    public enum DataCacheOperation
    {
        AddData = 2,
        RemoveData = 4,
        QueryData = 8
    }

    /// <summary>
    /// Defines cache exception handling
    /// </summary>
    [Flags]
    public enum DataCacheExceptionHandling
    {
        Continue = 2,
        Break = 4,
        ThrowException = 8
    }
}
