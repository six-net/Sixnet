using Sixnet.Cache;
using Sixnet.Cache.Keys.Parameters;
using Sixnet.Cache.String;
using Sixnet.Cache.String.Parameters;
using Sixnet.Code;
using Sixnet.Development.Data.Database;
using Sixnet.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Sixnet.Threading.Locking
{
    /// <summary>
    /// Lock manager
    /// </summary>
    public static partial class SixnetLocker
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

        /// <summary>
        /// Sixnet lock options
        /// </summary>
        static readonly SixnetLockOptions _options = new();

        #endregion

        #region Configure

        /// <summary>
        /// Configure lock
        /// </summary>
        /// <param name="configure"></param>
        public static void Configure(Action<SixnetLockOptions> configure)
        {
            configure?.Invoke(_options);
        }

        #endregion

        #region Common lock

        #region Handle lock parameter

        /// <summary>
        /// Handle lock parameter
        /// </summary>
        /// <param name="parameter"></param>
        static void HandleLockParameter(ISixnetCacheParameter parameter)
        {
            if (_options.DistributeLockObjectNames?.Contains(parameter.CacheObject?.ObjectName ?? string.Empty) ?? false)
            {
                parameter.UseInMemoryForDefault = false;
            }
            else
            {
                parameter.UseInMemoryForDefault = true;
            }
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
        public static LockInstance? GetLock(string lockObject, string lockName, string lockValue, int? expirationSeconds = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(lockName), nameof(lockName));
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(lockValue), nameof(lockValue));
            var lockOptions = new StringSetParameter()
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
                        Key = ConstantCacheKey.Create(lockName),
                        Type = CacheKeyType.String,
                        When = CacheSetWhen.NotExists,
                    }
                }
            };
            HandleLockParameter(lockOptions);
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
                var setResponse = SixnetCacher.String.Set(lockOptions);
                return setResponse?.Results?.FirstOrDefault()?.Key == lockName;
            }, expSeconds < 1 ? -1 : (expSeconds + 1) * 1000))
            {
                return new LockInstance(lockObject, lockName, lockValue);
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
            var getLockParameter = new StringGetParameter()
            {
                Keys = new List<CacheKey>() { lockName },
                CacheObject = cacheObject
            };
            HandleLockParameter(getLockParameter);
            var currentLockValue = SixnetCacher.String.Get(getLockParameter)?.Values?.FirstOrDefault()?.Value?.ToString();
            if (currentLockValue == lockValue)
            {
                var delLockOptions = new DeleteParameter()
                {
                    Keys = new List<CacheKey>() { lockName },
                    CacheObject = cacheObject
                };
                HandleLockParameter(delLockOptions);
                return SixnetCacher.Keys.Delete(delLockOptions)?.Success ?? false;
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
            SixnetDirectThrower.ThrowArgNullIf(entityType == null, nameof(entityType));
            return $"sixnet:lock:ctb:{entityType.FullName}".ToLower();
        }

        /// <summary>
        /// Get create table lock
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="expirationSeconds">Expiration seconds</param>
        /// <returns></returns>
        public static LockInstance? GetCreateTableLock(Type entityType, int? expirationSeconds = null)
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
        static string GetCreateDatabaseConnectionLockName(SixnetDatabaseServer server)
        {
            SixnetDirectThrower.ThrowArgNullIf(server == null, nameof(server));
            return $"sixnet:lock:cdc:{server.GetServerIdentityValue()}".ToLower();
        }

        /// <summary>
        /// Get create database connection lock
        /// </summary>
        /// <param name="server">Database server</param>
        /// <param name="lockValue">Lock value</param>
        /// <param name="expirationSeconds">Expiration seconds</param>
        /// <returns></returns>
        public static LockInstance? GetCreateDatabaseConnectionLock(SixnetDatabaseServer server, int? expirationSeconds = null)
        {
            var lockName = GetCreateDatabaseConnectionLockName(server);
            return GetLock(CreateDatabaseConnectionLockName, lockName, GetLockValue(), expirationSeconds);
        }

        #endregion

        #region Load localization string lock

        static string GetLoadLocalizationStringLockName(CultureInfo culture, string resourceBaseName = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(culture == null, nameof(culture));
            return (string.IsNullOrWhiteSpace(resourceBaseName)
                 ? $"sixnet:lock:lls:{culture.Name}".ToLower()
                 : $"sixnet:lock:lls:{resourceBaseName}:{culture.Name}".ToLower())?.Trim(':');
        }

        /// <summary>
        /// Get load localization string lock
        /// </summary>
        /// <param name="culture">Culture</param>
        /// <param name="expirationSeconds">Expiration seconds</param>
        /// <returns></returns>
        public static LockInstance? GetLoadLocalizationStringLock(CultureInfo culture, int? expirationSeconds = null)
        {
            var lockName = GetLoadLocalizationStringLockName(culture);
            return GetLock(LoadLocalizationStringLockName, lockName, GetLockValue(), expirationSeconds);
        }

        /// <summary>
        /// Get load localization string lock
        /// </summary>
        /// <param name="resourceBaseName">Resource base name</param>
        /// <param name="culture">Culture</param>
        /// <param name="expirationSeconds">Expiration seconds</param>
        /// <returns></returns>
        public static LockInstance? GetLoadLocalizationStringLock(string resourceBaseName, CultureInfo culture, int? expirationSeconds = null)
        {
            var lockName = GetLoadLocalizationStringLockName(culture, resourceBaseName);
            return GetLock(LoadLocalizationStringLockName, lockName, GetLockValue(), expirationSeconds);
        }

        #endregion
    }
}
