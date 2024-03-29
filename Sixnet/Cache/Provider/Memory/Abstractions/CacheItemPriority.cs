// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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