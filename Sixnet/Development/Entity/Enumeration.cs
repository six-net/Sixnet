using System;

namespace Sixnet.Development.Entity
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
        Increment = 4,
        Revision = 8,
        Sequence = 16,
        Archived = 32,
        Isolation = 64,
        CreatedDate = 128,
        CreatedUserId = 256,
        CreatedUserName = 512,
        UpdatedDate = 1024,
        UpdatedUserId = 2048,
        UpdatedUserName = 4096,
        SplitValue = 8192,
    }

    /// <summary>
    /// Defines field db feature
    /// </summary>
    [Flags]
    [Serializable]
    public enum FieldDbFeature
    {
        None = 0,
        NotFixedLength = 2,
        NotNull = 4,
        Default = 8
    }

    /// <summary>
    /// Field behavior
    /// </summary>
    [Flags]
    [Serializable]
    public enum FieldBehavior
    {
        None = 0,
        NotQuery = 2,
        NotInsert = 4,
        NotUpdate = 8,
    }

    /// <summary>
    /// Defines field type fragment type
    /// </summary>
    public enum FieldTypeFragmentType
    {
        None = 0,
        DbType = 1,
        DefaultValue = 2
    }

    /// <summary>
    /// Defines relation behavior
    /// </summary>
    [Flags]
    [Serializable]
    public enum RelationBehavior
    {
        None = 0,
        CascadingDelete = 2
    }

    /// <summary>
    /// Defines entity style
    /// </summary>
    [Flags]
    [Serializable]
    public enum EntityStyle
    {
        Physical = 0,
        Virtual = 2
    }

    /// <summary>
    /// Defines entity split table type
    /// </summary>
    [Flags]
    [Serializable]
    public enum SplitTableType
    {
        None = 0,
        Year = 1,
        Season = 2,
        Month = 3,
        Week = 4,
        Day = 5,
        Custom = 10000
    }
}
