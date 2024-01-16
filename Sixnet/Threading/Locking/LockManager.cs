using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Sixnet.Cache;
using Sixnet.Cache.Keys;
using Sixnet.Cache.Keys.Options;
using Sixnet.Cache.String;
using Sixnet.Code;
using Sixnet.Development.Data.Database;
using Sixnet.Exceptions;
using static Sixnet.Cache.CacheManager;

namespace Sixnet.Threading.Locking
{
    /// <summary>
    /// Lock manager
    /// </summary>
    public static partial class LockManager
    {
        #region Fields

        /// <summary>
        /// Create table lock name
        /// </summary>
        public const string CreateTableLockObjectName = "SIXNET_CREATE_TABLE_LOCK";

        /// <summary>
        /// Create database connection lock name
        /// </summary>
        public const string CreateDatabaseConnectionLockName = "SIXNET_CREATE_DATABASE_CONNECTION_LOCK";

        /// <summary>
        /// Load localization string lock name
        /// </summary>
        public const string LoadLocalizationStringLockName = "SIXNET_LOAD_LOCALIZATION_STRING_LOCK";

        /// <summary>
        /// Lock object expiration seconds
        /// </summary>
        static readonly Dictionary<string, int> LockObjectExpirationSeconds = new()
        {
            { CreateTableLockObjectName, 60},
            { CreateDatabaseConnectionLockName, 60},
            { LoadLocalizationStringLockName, 60}
        };

        #endregion

        #region Common lock

        #region Handle lock options

        /// <summary>
        /// Handle lock options
        /// </summary>
        /// <param name="cacheOptions"></param>
        static void HandleLockOptions(ICacheOptions cacheOptions)
        {
            cacheOptions.UseInMemoryForDefault = true;
        }

        #endregion

        #region Get lock expiration seconds

        /// <summary>
        /// Get lock expiration seconds
        /// </summary>
        /// <param name="lockObject">Lock object</param>
        /// <param name="expirationSeconds">Expiration seconds</param>
        /// <returns></returns>
        static int GetExpirationSeconds(string lockObject, int? expirationSeconds)
        {
            if (expirationSeconds.HasValue && expirationSeconds.Value > 0)
            {
                return expirationSeconds.Value;
            }
            if (LockObjectExpirationSeconds.TryGetValue(lockObject, out var lockExpSeconds) && lockExpSeconds > 0)
            {
                return lockExpSeconds;
            }
            return -1;
        }

        #endregion

        #region Enter lock

        /// <summary>
        /// Get lock
        /// </summary>
        /// <param name="lockObject">Lock object</param>
        /// <param name="lockName">Lock name</param>
        /// <param name="lockValue">Lock value</param>
        /// <param name="expirationSeconds">Expiration seconds</param>
        /// <returns></returns>
        public static SixnetLock? GetLock(string lockObject, string lockName, string lockValue, int? expirationSeconds = null)
        {
            ThrowHelper.ThrowArgNullIf(string.IsNullOrWhiteSpace(lockName), nameof(lockName));
            ThrowHelper.ThrowArgNullIf(string.IsNullOrWhiteSpace(lockValue), nameof(lockValue));
            var lockOptions = new StringSetOptions()
            {
                CacheObject = new CacheObject()
                {
                    ObjectName = lockObject
                },
                Items = new List<CacheEntry>()
                {
                    new CacheEntry()
                    {
                        Value = lockValue,
                        Key = lockName,
                        Type = CacheKeyType.String,
                        When = CacheSetWhen.NotExists,
                    }
                }
            };
            HandleLockOptions(lockOptions);
            var expSeconds = GetExpirationSeconds(lockObject, expirationSeconds);

            if (SpinWait.SpinUntil(() =>
            {
                lockOptions.Items.ForEach(ce =>
                {
                    ce.Expiration = expSeconds > 0 ? new CacheExpiration()
                    {
                        SlidingExpiration = false,
                        AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(expSeconds)
                    } : null;
                });
                var setResponse = CacheManager.String.Set(lockOptions);
                return setResponse?.Results?.FirstOrDefault()?.Key == lockName;
            }, expSeconds < 1 ? -1 : (expSeconds + 1) * 1000))
            {
                return new SixnetLock(lockObject, lockName, lockValue);
            }
            return null;
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
        public static bool ReleaseLock(string lockObject, string lockName, string lockValue)
        {
            var cacheObject = new CacheObject()
            {
                ObjectName = lockObject
            };
            var getLockOptions = new StringGetOptions()
            {
                Keys = new List<CacheKey>() { lockName },
                CacheObject = cacheObject
            };
            HandleLockOptions(getLockOptions);
            var currentLockValue = CacheManager.String.Get(getLockOptions)?.Values?.FirstOrDefault()?.Value?.ToString();
            if (currentLockValue == lockValue)
            {
                var delLockOptions = new DeleteOptions()
                {
                    Keys = new List<CacheKey>() { lockName },
                    CacheObject = cacheObject
                };
                HandleLockOptions(delLockOptions);
                return CacheManager.Keys.Delete(delLockOptions)?.Success ?? false;
            }
            return false;
        }

        #endregion

        #region Get lock value

        /// <summary>
        /// Get a lock value
        /// </summary>
        /// <returns></returns>
        public static string GetLockValue()
        {
            return SerialNumber.GenerateSerialNumber().ToString();
        }

        #endregion

        #endregion

        #region Create table lock

        /// <summary>
        /// Get create table lock name
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        static string GetCreateTableLockName(Type entityType)
        {
            ThrowHelper.ThrowArgNullIf(entityType == null, nameof(entityType));
            return $"sixnet:lock:ctb:{entityType.FullName}".ToLower();
        }

        /// <summary>
        /// Get create table lock
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="expirationSeconds">Expiration seconds</param>
        /// <returns></returns>
        public static SixnetLock? GetCreateTableLock(Type entityType, int? expirationSeconds = null)
        {
            var lockName = GetCreateTableLockName(entityType);
            return GetLock(CreateTableLockObjectName, lockName, GetLockValue(), expirationSeconds);
        }

        #endregion

        #region Create database connection lock

        /// <summary>
        /// Get create database connection lock name
        /// </summary>
        /// <param name="server">Database server</param>
        /// <returns></returns>
        static string GetCreateDatabaseConnectionLockName(DatabaseServer server)
        {
            ThrowHelper.ThrowArgNullIf(server == null, nameof(server));
            return $"sixnet:lock:cdc:{server.GetServerIdentityValue()}".ToLower();
        }

        /// <summary>
        /// Get create database connection lock
        /// </summary>
        /// <param name="server">Database server</param>
        /// <param name="lockValue">Lock value</param>
        /// <param name="expirationSeconds">Expiration seconds</param>
        /// <returns></returns>
        public static SixnetLock? GetCreateDatabaseConnectionLock(DatabaseServer server, int? expirationSeconds = null)
        {
            var lockName = GetCreateDatabaseConnectionLockName(server);
            return GetLock(CreateDatabaseConnectionLockName, lockName, GetLockValue(), expirationSeconds);
        }

        #endregion

        #region Load localization string lock

        static string GetLoadLocalizationStringLockName(CultureInfo culture)
        {
            ThrowHelper.ThrowArgNullIf(culture == null, nameof(culture));
            return $"sixnet:lock:lls:{culture.Name}".ToLower();
        }

        /// <summary>
        /// Get load localization string lock
        /// </summary>
        /// <param name="culture">Culture</param>
        /// <param name="expirationSeconds">Expiration seconds</param>
        /// <returns></returns>
        public static SixnetLock? GetLoadLocalizationStringLock(CultureInfo culture, int? expirationSeconds = null)
        {
            var lockName = GetLoadLocalizationStringLockName(culture);
            return GetLock(LoadLocalizationStringLockName, lockName, GetLockValue(), expirationSeconds);
        }

        #endregion
    }
}
