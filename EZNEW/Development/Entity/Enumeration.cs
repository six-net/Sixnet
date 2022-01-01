using System;

namespace EZNEW.Development.Entity
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
        UpdateTime = 16,
        Sort = 32,
        Level = 64,
        Parent = 128,
        Display = 256,
        CreationTime = 512,
        ObsoleteTag = 1024
    }

    /// <summary>
    /// Defines relation field options
    /// </summary>
    [Flags]
    [Serializable]
    public enum RelationBehavior
    {
        None = 0,
        CascadingRemove = 2
    }
}
