// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Sixnet.Cache.Provider.Memory.Abstractions
{
    internal enum EvictionReason
    {
        None,

        /// <summary>
        /// Manually
        /// </summary>
        Removed,

        /// <summary>
        /// Overwritten
        /// </summary>
        Replaced,

        /// <summary>
        /// Timed out
        /// </summary>
        Expired,

        /// <summary>
        /// Event
        /// </summary>
        TokenExpired,

        /// <summary>
        /// Overflow
        /// </summary>
        Capacity,
    }
}