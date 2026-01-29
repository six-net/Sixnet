// "Company © 2025. All rights reserved."

using System.Threading;
using System.Threading.Tasks;

using Sixnet.Cache;
using Sixnet.Cache.Keys.Parameters;
using Sixnet.Cache.String.Parameters;
using Sixnet.Development.Data.Database;
using Sixnet.Exceptions;

namespace Sixnet.Threading.Locking
{
    public static partial class SixnetLocker
    {
        #region Enter lock

        /// <summary>
        /// Get lock
        /// </summary>
        /// <param name="lockObject">Lock object</param>
        /// <param name="lockName">Lock name</param>
        /// <param name="lockValue">Lock value</param>
        /// <param name="expirationSeconds">Expiration seconds</param>
        /// <returns></returns>
        public static Task<LockInstance?> GetLockAsync(string lockObject, string lockName, string lockValue, int? expirationSeconds = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(lockName), nameof(lockName));
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(lockValue), nameof(lockValue));
            var lockParameter = new SixnetStringSetParameter()
            {
                CacheObject = new SixnetCacheObject()
                {
                    ObjectName = lockObject
                },
                Items = new List<SixnetCacheEntry>()
                {
                    new SixnetCacheEntry()
                    {
                        Value = lockValue,
                        Key = ConstantCacheKey.Create(lockName),
                        Type = CacheKeyType.String,
                        When = CacheSetWhen.NotExists,
                    }
                }
            };
            HandleLockParameter(lockParameter);
            var expSeconds = GetExpirationSeconds(lockObject, expirationSeconds);

            async Task<bool> setLockFunc()
            {
                lockParameter.Items.ForEach(ce =>
                {
                    ce.Expiration = expSeconds > 0 ? new SixnetCacheExpiration()
                    {
                        SlidingExpiration = false,
                        AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(expSeconds)
                    } : null;
                });
                var setResponse = await SixnetCacher.String.SetAsync(lockParameter).ConfigureAwait(false);
                return setResponse?.Results?.FirstOrDefault()?.Key == lockName;
            }
            var setLockTask = setLockFunc();
            LockInstance? lockObj = null;
            if (SpinWait.SpinUntil(() =>
            {
                if (setLockTask.IsCompleted)
                {
                    if (!setLockTask.Result)
                    {
                        setLockTask = setLockFunc();
                        return false;
                    }
                    return true;
                }
                return false;
            }, expSeconds < 1 ? -1 : (expSeconds + 1) * 1000))
            {
                lockObj = new LockInstance(lockObject, lockName, lockValue);
            }
            return Task.FromResult(lockObj);
        }

        #endregion

        #region Release lock

        /// <summary>
        /// Release lock
        /// </summary>
        /// <param name="lockObject">Lock object</param>
        /// <param name="lockName">Lock name</param>
        /// <param name="lockValue">Lock value</param>
        /// <returns></returns>
        public static async Task<bool> ReleaseLockAsync(string lockObject, string lockName, string lockValue)
        {
            var cacheObject = new SixnetCacheObject()
            {
                ObjectName = lockObject
            };
            var getLockParameter = new SixnetStringGetParameter()
            {
                Keys = new List<SixnetCacheKey>() { lockName },
                CacheObject = cacheObject
            };
            HandleLockParameter(getLockParameter);
            var currentLockValue = (await SixnetCacher.String.GetAsync(getLockParameter).ConfigureAwait(false))?.Values?.FirstOrDefault()?.Value?.ToString();
            if (currentLockValue == lockValue)
            {
                var delLockParameter = new SixnetDeleteParameter()
                {
                    Keys = new List<SixnetCacheKey>() { lockName },
                    CacheObject = cacheObject
                };
                HandleLockParameter(delLockParameter);
                return (await SixnetCacher.Keys.DeleteAsync(delLockParameter).ConfigureAwait(false))?.Success ?? false;
            }
            return false;
        }

        #endregion

        #region Create table lock

        /// <summary>
        /// Get create table lock
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="expirationSeconds">Expiration seconds</param>
        /// <returns></returns>
        public static Task<LockInstance?> GetCreateTableLockAsync(Type entityType, int? expirationSeconds = null)
        {
            var lockName = GetCreateTableLockName(entityType);
            return GetLockAsync(CreateTableLockObjectName, lockName, GetLockValue(), expirationSeconds);
        }

        #endregion

        #region Create database connection lock

        /// <summary>
        /// Get create database connection lock
        /// </summary>
        /// <param name="server">Database server</param>
        /// <param name="expirationSeconds">Expiration seconds</param>
        /// <returns></returns>
        public static Task<LockInstance?> GetCreateDatabaseConnectionLockAsync(DatabaseServer server, int? expirationSeconds = null)
        {
            var lockName = GetCreateDatabaseConnectionLockName(server);
            return GetLockAsync(CreateDatabaseConnectionLockName, lockName, GetLockValue(), expirationSeconds);
        }

        #endregion
    }
}
