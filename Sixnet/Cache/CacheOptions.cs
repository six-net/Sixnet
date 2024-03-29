﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sixnet.Cache.Command.Result;
using Sixnet.Cache.Constant;
using Sixnet.Exceptions;
using Sixnet.Logging;

namespace Sixnet.Cache
{
    /// <summary>
    /// Cache request option
    /// </summary>
    public abstract class CacheOptions<TResponse> : ICacheOptions where TResponse : CacheResponse, new()
    {
        /// <summary>
        /// Gets or sets the cache object
        /// </summary>
        public CacheObject CacheObject { get; set; }

        /// <summary>
        /// Gets or sets the command flags
        /// </summary>
        public CacheCommandFlags CommandFlags { get; set; } = CacheCommandFlags.None;

        /// <summary>
        /// Gets or sets the cache structure pattern
        /// </summary>
        public CacheStructurePattern StructurePattern { get; set; } = CacheStructurePattern.Distribute;

        /// <summary>
        /// Whether in memory cache first
        /// </summary>
        public bool InMemoryFirst { get; set; } = false;

        /// <summary>
        /// Whether use in memory for default
        /// </summary>
        public bool UseInMemoryForDefault { get; set; } = false;

        /// <summary>
        /// Verify response
        /// </summary>
        public Func<TResponse, bool> VerifyResponse { get; set; } = res => res?.Success ?? false;

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <returns>Return cache result</returns>
        internal async Task<TResponse> ExecuteAsync()
        {
            var server = GetCacheServer();
            CacheResult<TResponse> result = new CacheResult<TResponse>();
            var provider = CacheManager.Configuration.GetCacheProvider(server.ServerType);
            if (provider == null)
            {
                throw new SixnetException($"【{server.ServerType}】Not set provider");
            }
            return await ExecuteCacheOperationAsync(provider, server).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <returns>Return cache result</returns>
        internal TResponse Execute()
        {
            var server = GetCacheServer();
            CacheResult<TResponse> result = new CacheResult<TResponse>();
            var provider = CacheManager.Configuration.GetCacheProvider(server.ServerType);
            if (provider == null)
            {
                throw new SixnetException($"【{server.ServerType}】Not set provider");
            }
            return ExecuteCacheOperation(provider, server);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <returns>Return cache result</returns>
        internal async Task<TResponse> ExecuteAsync(CacheServer server)
        {
            if (server == null)
            {
                throw new ArgumentNullException(nameof(server));
            }
            var provider = CacheManager.Configuration.GetCacheProvider(server.ServerType);
            if (provider == null)
            {
                throw new SixnetException($"【{server.ServerType}】Not set provider");
            }
            return await ExecuteCacheOperationAsync(provider, server).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <returns>Return cache result</returns>
        internal TResponse Execute(CacheServer server)
        {
            if (server == null)
            {
                throw new ArgumentNullException(nameof(server));
            }
            var provider = CacheManager.Configuration.GetCacheProvider(server.ServerType);
            if (provider == null)
            {
                throw new SixnetException($"【{server.ServerType}】Not set provider");
            }
            return ExecuteCacheOperation(provider, server);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return cache operation response</returns>
        protected abstract Task<TResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server);

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return cache operation response</returns>
        protected abstract TResponse ExecuteCacheOperation(ICacheProvider cacheProvider, CacheServer server);

        /// <summary>
        /// Get cache server
        /// </summary>
        /// <returns>Return cache server</returns>
        protected virtual CacheServer GetCacheServer()
        {
            var server = CacheManager.Configuration.GetCacheServer(this);
            if (server == null && UseInMemoryForDefault)
            {
                server = CacheManager.Configuration.DefaultInMemoryServer;
            }
            ThrowHelper.ThrowFrameworkErrorIf(server == null, $"Not config cache sever for {CacheObject?.ObjectName}");
            return server;
        }
    }
}
