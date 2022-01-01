using System;

namespace EZNEW.Development.Domain.Repository.Warehouse
{
    /// <summary>
    /// Defines data record operation
    /// </summary>
    [Serializable]
    public enum DataRecordOperation
    {
        None = 2,
        Save = 4,
        Remove = 8
    }

    /// <summary>
    /// Defines data source
    /// </summary>
    [Serializable]
    public enum DataSource
    {
        Storage = 2,
        New = 4
    }

    /// <summary>
    /// Defines conditional operation type
    /// </summary>
    internal enum ConditionalOperationType
    {
        Remove = 2,
        Update = 4
    }
}
