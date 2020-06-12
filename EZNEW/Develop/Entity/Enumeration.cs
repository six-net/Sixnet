using System;

namespace EZNEW.Develop.Entity
{
    /// <summary>
    /// Defines entity field cache option
    /// </summary>
    [Flags]
    [Serializable]
    public enum EntityFieldCacheOption
    {
        None = 0,
        CacheKey = 2,
        Ignore = 4,
        CacheKeyPrefix = 8
    }
}
