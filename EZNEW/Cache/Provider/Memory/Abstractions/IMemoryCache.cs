// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace EZNEW.Cache.Provider.Memory.Abstractions
{
    /// <summary>
    /// Represents a local in-memory cache whose values are not serialized.
    /// </summary>
    internal interface IMemoryCache : IDisposable
    {
        /// <summary>
        /// Gets the item associated with this key if present.
        /// </summary>
        /// <param name="key">An object identifying the requested entry.</param>
        /// <param name="value">The located value or null.</param>
        /// <returns>True if the key was found.</returns>
        bool TryGetValue(object key, out object value);

        /// <summary>
        /// Gets the ICacheEntry associated with this key if present.
        /// </summary>
        /// <param name="key">An object identifying the requested entry.</param>
        /// <param name="value">The located ICacheEntry or null.</param>
        /// <returns>True if the key was found.</returns>
        bool TryGetEntry(object key, out ICacheEntry value);

        /// <summary>
        /// Get a random cache key
        /// </summary>
        /// <returns></returns>
        string GetRandomKey();

        /// <summary>
        /// get all keys
        /// </summary>
        /// <returns></returns>
        List<string> GetAllKeys();

        /// <summary>
        /// Create or overwrite an entry in the cache.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        /// <returns>The newly created <see cref="ICacheEntry"/> instance.</returns>
        ICacheEntry CreateEntry(object key);

        /// <summary>
        /// Removes the object associated with the given key.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        void Remove(object key);
    }
}