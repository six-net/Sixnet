using System;

namespace EZNEW.Cache
{
    /// <summary>
    /// Defines cache operation type
    /// </summary>
    public enum CacheOperationType
    {
        Data = 110,
        Command = 120
    }

    /// <summary>
    /// Defines cache server type
    /// </summary>
    public enum CacheServerType
    {
        InMemory = 310,
        Redis = 320,
        Memcached = 330
    }

    /// <summary>
    /// Defines remove target type
    /// </summary>
    public enum RemoveTarget
    {
        Object = 110,
        Key = 120
    }

    /// <summary>
    /// Defines list insert type
    /// </summary>
    public enum ListInsertType
    {
        Before = 210,
        After = 220
    }

    /// <summary>
    /// Defines set operation type
    /// </summary>
    public enum SetOperationType
    {
        Union = 0,
        Intersect = 1,
        Difference = 2
    }

    /// <summary>
    /// Defines sorted set exclude
    /// </summary>
    public enum SortedSetExclude
    {
        None = 0,
        Start = 1,
        Stop = 2,
        Both = 3
    }

    /// <summary>
    /// Defines sorted order
    /// </summary>
    public enum SortedOrder
    {
        Ascending = 0,
        Descending = 1
    }

    /// <summary>
    /// Defines key type
    /// </summary>
    public enum CacheKeyType
    {
        String = 1,
        Hash = 2,
        List = 3,
        Set = 4,
        SortedSet = 5
    }

    /// <summary>
    /// Defines log level
    /// </summary>
    public enum LogLevel
    {
        Debug = 101,
        Verbose = 105,
        Notice = 110,
        Warning = 115
    }

    /// <summary>
    /// Defines appendf sync
    /// </summary>
    public enum AppendfSync
    {
        No = 101,
        Always = 105,
        EverySecond = 110
    }

    /// <summary>
    /// Defines cache command flags
    /// </summary>
    [Flags]
    public enum CacheCommandFlags
    {
        None = 0,
        PreferMaster = 0,
        HighPriority = 1,
        FireAndForget = 2,
        DemandMaster = 4,
        PreferSlave = 8,
        DemandSlave = 12,
        NoRedirect = 64,
        NoScriptCache = 512
    }

    /// <summary>
    /// Defines cache set when
    /// </summary>
    [Flags]
    public enum CacheSetWhen
    {
        Always = 0,
        Exists = 1,
        NotExists = 2
    }

    /// <summary>
    /// Defines cache bit wise
    /// </summary>
    public enum CacheBitwise
    {
        And = 0,
        Or = 1,
        Xor = 2,
        Not = 3
    }

    /// <summary>
    /// Defines set aggregate
    /// </summary>
    public enum SetAggregate
    {
        Sum = 0,
        Min = 1,
        Max = 2
    }

    /// <summary>
    /// Defines cache sort type
    /// </summary>
    public enum CacheSortType
    {
        Numeric = 0,
        Alphabetic = 1
    }

    /// <summary>
    /// Defines cache migrate options
    /// </summary>
    public enum CacheMigrateOptions
    {
        None = 0,
        Copy = 1,
        Replace = 2
    }

    /// <summary>
    /// Defines key match pattern
    /// </summary>
    public enum PatternType
    {
        Include = 2,
        StartWith = 4,
        EndWith = 8
    }
}
