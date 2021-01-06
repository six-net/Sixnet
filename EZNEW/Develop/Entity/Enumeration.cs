using System;

namespace EZNEW.Develop.Entity
{
    /// <summary>
    /// Defines entity field cache role
    /// </summary>
    [Flags]
    [Serializable]
    public enum FieldCacheRole
    {
        None = 0,
        CacheKey = 2,
        Ignore = 4,
        CacheKeyPrefix = 8
    }

    /// <summary>
    /// Defines entity structure
    /// </summary>
    [Serializable]
    public enum EntityStructure
    {
        Normal = 2,
        Tree = 4,
        Relation = 8
    }

    /// <summary>
    /// Defines entity field role
    /// </summary>
    [Flags]
    [Serializable]
    public enum FieldRole
    {
        None = 0,
        PrimaryKey = 2,
        ForeignKey = 4,
        Version = 8,
        RefreshDate = 16,
        Sort = 32,
        Level = 64,
        Parent = 128,
        Display = 256
    }
}
