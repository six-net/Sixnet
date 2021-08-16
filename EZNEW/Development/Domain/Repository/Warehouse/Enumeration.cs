using System;

namespace EZNEW.Development.Domain.Repository.Warehouse
{
    /// <summary>
    /// Defines data operate
    /// </summary>
    [Serializable]
    public enum WarehouseDataOperate
    {
        None = 2,
        Save = 4,
        Remove = 8
    }

    /// <summary>
    /// Defines warehouse life source
    /// </summary>
    [Serializable]
    public enum DataLifeSource
    {
        DataSource = 2,
        New = 4
    }
}
