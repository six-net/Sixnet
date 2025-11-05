// "Company © 2025. All rights reserved."

namespace Sixnet.Cache.Provider.Memory.Abstractions
{
    /// <summary>
    /// Signature of the callback which gets called when a cache entry expires.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="reason">The <see cref="EvictionReason"/>.</param>
    /// <param name="state">The information that was passed when registering the callback.</param>
    internal delegate void PostEvictionDelegate(object key, object value, EvictionReason reason, object state);
}