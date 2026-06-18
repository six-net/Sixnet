// "Company © 2025. All rights reserved."

using System.Globalization;
using System.Threading;

using Sixnet.Cache;
using Sixnet.Cache.Keys.Parameters;
using Sixnet.Cache.String.Parameters;
using Sixnet.Code;
using Sixnet.DependencyInjection;
using Sixnet.Development.Data.Database;
using Sixnet.Exceptions;
using Sixnet.Security.Authorization;

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
        /// Create in process queue lock name
        /// </summary>
        public const string CreateInProcessQueueLockName = "SIXNET_LOAD_CREATE_IN_PROCESS_QUEUE_LOCK";

        /// <summary>
        /// Sixnet lock options
        /// </summary>
        static readonly SixnetLockOptions _options = new();

        #endregion

        #region Common lock

        static SixnetLockOptions GetLockOptions()
        {
            return SixnetContainer.GetOptions<SixnetLockOptions>() ?? _options;
        }

        #region Handle lock parameter

        /// <summary>
        /// Handle lock parameter
        /// </summary>
        /// <param name="parameter"></param>
        static void HandleLockParameter(ISixnetCacheParameter parameter)
        {
            var lockOptions = GetLockOptions();
            if ((_options?.LockObjects?.TryGetValue(parameter.CacheObject?.ObjectName ?? string.Empty, out var lockObjectSetting) ?? false) && (lockObjectSetting?.Remote ?? false))
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
            var lockOptions = GetLockOptions();
            if ((lockOptions?.LockObjects?.TryGetValue(lockObject, out var lockObjectSetting) ?? false) && (lockObjectSetting?.ExpirationSeconds > 0))
            {
                return lockObjectSetting.ExpirationSeconds.Value;
            }
            return lockOptions.DefaultExpirationSeconds;
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
        public static SixnetLockInstance? GetLock(string lockObject, string lockName, string lockValue, int? expirationSeconds = null)
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
                        Type = SixnetCacheKeyType.String,
                        When = SixnetCacheSetWhen.NotExists,
                    }
                }
            };
            HandleLockParameter(lockParameter);
            var expSeconds = GetExpirationSeconds(lockObject, expirationSeconds);

            if (SpinWait.SpinUntil(() =>
            {
                lockParameter.Items.ForEach(ce =>
                {
                    ce.Expiration = expSeconds > 0 ? new SixnetCacheExpiration()
                    {
                        SlidingExpiration = false,
                        AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(expSeconds)
                    } : null;
                });
                var setResponse = SixnetCacher.String.Set(lockParameter);
                return setResponse?.Results?.FirstOrDefault()?.Key == lockName;
            }, expSeconds < 1 ? -1 : (expSeconds + 1) * 1000))
            {
                return new SixnetLockInstance(lockObject, lockName, lockValue);
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
            var currentLockValue = SixnetCacher.String.Get(getLockParameter)?.Values?.FirstOrDefault()?.Value?.ToString();
            if (currentLockValue == lockValue)
            {
                var delLockParameter = new SixnetDeleteParameter()
                {
                    Keys = new List<SixnetCacheKey>() { lockName },
                    CacheObject = cacheObject
                };
                HandleLockParameter(delLockParameter);
                return SixnetCacher.Keys.Delete(delLockParameter)?.Success ?? false;
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
            return SixnetSerialNumber.GenerateSerialNumber().ToString();
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
        public static SixnetLockInstance? GetCreateTableLock(Type entityType, int? expirationSeconds = null)
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
        /// <param name="expirationSeconds">Expiration seconds</param>
        /// <returns></returns>
        public static SixnetLockInstance? GetCreateDatabaseConnectionLock(SixnetDatabaseServer server, int? expirationSeconds = null)
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
        internal static SixnetLockInstance? GetLoadLocalizationStringLock(CultureInfo culture, int? expirationSeconds = null)
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
        internal static SixnetLockInstance? GetLoadLocalizationStringLock(string resourceBaseName, CultureInfo culture, int? expirationSeconds = null)
        {
            var lockName = GetLoadLocalizationStringLockName(culture, resourceBaseName);
            return GetLock(LoadLocalizationStringLockName, lockName, GetLockValue(), expirationSeconds);
        }

        #endregion

        #region Create internal queue lock

        /// <summary>
        /// Get create internal queue lock name
        /// </summary>
        /// <param name="queueName">Queue name</param>
        /// <returns></returns>
        static string GetCreateInternalQueueLockName(string queueName)
        {
            return $"sixnet:lock:cipq:{queueName}".ToLower();
        }

        /// <summary>
        /// Get create internal queue lock
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        internal static SixnetLockInstance? GetCreateInternalQueueLock(string queueName, int? expirationSeconds = null)
        {
            var lockName = GetCreateInternalQueueLockName(queueName);
            return GetLock(CreateInProcessQueueLockName, lockName, GetLockValue(), expirationSeconds);
        }

        #endregion

        #region Execute

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="action"></param>
        /// <param name="lockObject"></param>>
        /// <param name="expirationSeconds"></param>
        public static void Execute(Action action, string lockObject, int? expirationSeconds = null)
        {
            var lockInstance = GetLock(lockObject, lockObject, SixnetGuidHelper.GetGuid().ToString(), expirationSeconds);
            try
            {
                action?.Invoke();
            }
            finally
            {
                if (lockInstance.HasValue)
                {
                    lockInstance.Value.Release();
                }
            }
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="action"></param>
        /// <param name="lockObject"></param>
        /// <param name="lockName"></param>
        /// <param name="expirationSeconds"></param>
        public static void Execute(Action action, string lockObject, string lockName, int? expirationSeconds = null)
        {
            var lockInstance = GetLock(lockObject, lockName, SixnetGuidHelper.GetGuid().ToString(), expirationSeconds);
            try
            {
                action?.Invoke();
            }
            finally
            {
                if (lockInstance.HasValue)
                {
                    lockInstance.Value.Release();
                }
            }
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="func"></param>
        /// <param name="lockObject"></param>>
        /// <param name="expirationSeconds"></param>
        public static TResult Execute<TResult>(Func<TResult> func, string lockObject, int? expirationSeconds = null)
        {
            var lockInstance = GetLock(lockObject, lockObject, SixnetGuidHelper.GetGuid().ToString(), expirationSeconds);
            try
            {
                return func.Invoke();
            }
            finally
            {
                if (lockInstance.HasValue)
                {
                    lockInstance.Value.Release();
                }
            }
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="func"></param>
        /// <param name="lockObject"></param>
        /// <param name="lockName"></param>
        /// <param name="expirationSeconds"></param>
        public static TResult Execute<TResult>(Func<TResult> func, string lockObject, string lockName, int? expirationSeconds = null)
        {
            var lockInstance = GetLock(lockObject, lockName, SixnetGuidHelper.GetGuid().ToString(), expirationSeconds);
            try
            {
                return func.Invoke();
            }
            finally
            {
                if (lockInstance.HasValue)
                {
                    lockInstance.Value.Release();
                }
            }
        }

        #endregion
    }
}
