// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Entity
{
    /// <summary>
    /// Defines entity field cache role
    /// </summary>
    [Flags]
    [Serializable]
    public enum SixnetFieldCacheRole
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
    public enum SixnetFieldRole
    {
        None = 0,
        PrimaryKey = 2,
        Increment = 4,
        Revision = 8,
        Sequence = 16,
        Archive = 32,
        Isolation = 64,
        CreateDate = 128,
        CreateUserId = 256,
        CreateUserName = 512,
        CreateUserDisplayName = 1024,
        UpdateDate = 2048,
        UpdateUserId = 4096,
        UpdateUserName = 8192,
        UpdateUserDisplayName = 16384,
        SplitValue = 32768,
        GeneratedId = 65536,
        UploadPath = 131072
    }

    /// <summary>
    /// Defines field db feature
    /// </summary>
    [Flags]
    [Serializable]
    public enum SixnetFieldDbFeature
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
    public enum SixnetFieldBehavior
    {
        None = 0,
        NotQuery = 2,
        NotInsert = 4,
        NotUpdate = 8,
        NotMoveUploadPath = 16
    }

    /// <summary>
    /// Defines field type fragment type
    /// </summary>
    public enum SixnetFieldTypeFragmentType
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
    public enum SixnetRelationBehavior
    {
        None = 0,
        CascadingDelete = 2
    }

    /// <summary>
    /// Defines entity style
    /// </summary>
    [Flags]
    [Serializable]
    public enum SixnetEntityStyle
    {
        Physical = 0,
        Virtual = 2
    }

    /// <summary>
    /// Defines entity split table type
    /// </summary>
    [Flags]
    [Serializable]
    public enum SixnetSplitTableType
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
