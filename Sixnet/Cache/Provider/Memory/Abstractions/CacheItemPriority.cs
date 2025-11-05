// "Company © 2025. All rights reserved."

namespace Sixnet.Cache.Provider.Memory.Abstractions
{
    // TODO: Granularity?
    /// <summary>
    /// Specifies how items are prioritized for preservation during a memory pressure triggered cleanup.
    /// </summary>
    internal enum CacheItemPriority
    {
        Low,
        Normal,
        High,
        NeverRemove,
    }
}