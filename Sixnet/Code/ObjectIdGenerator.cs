using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Sixnet.Cache;
using Sixnet.Cache.String.Parameters;
using Sixnet.Exceptions;

namespace Sixnet.Code
{
    public class ObjectIdOptions
    {
        /// <summary>
        /// Gets or sets the object name
        /// </summary>
        public string ObjectName { get; set; }

        /// <summary>
        /// Gets or sets size
        /// </summary>
        public int Size { get; set; } = 1;
    }

    public static class ObjectIdGenerator
    {
        static readonly CacheObject _idCacheObject = new();

        #region Long

        /// <summary>
        /// Get long ids
        /// <returns></returns>
        public static async Task<List<long>> GetLongIdsAsync(Action<ObjectIdOptions> configure)
        {
            var objectIdOptions = new ObjectIdOptions();
            configure?.Invoke(objectIdOptions);
            var objectName = objectIdOptions.ObjectName;
            var size = objectIdOptions.Size;

            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(objectName), nameof(ObjectIdOptions.ObjectName));
            SixnetDirectThrower.ThrowArgErrorIf(size < 1, nameof(ObjectIdOptions.Size));

            var objectKey = GetObjectIdKey(objectIdOptions.ObjectName);
            var incrResponse = await SixnetCacher.String.IncrementAsync(new StringIncrementParameter()
            {
                CacheObject = _idCacheObject,
                Key = objectKey,
                Value = objectIdOptions.Size
            }).ConfigureAwait(false);

            SixnetDirectThrower.ThrowIf<SixnetApplicationException>(!(incrResponse?.Success ?? false), incrResponse?.Message);

            var newValue = incrResponse.NewValue;
            var newIds = new List<long>();
            var beginId = newValue - size;
            for (var i = beginId + 1; i <= newValue; i++)
            {
                newIds.Add(i);
            }
            return newIds;
        }

        /// <summary>
        /// Get long ids
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Task<List<long>> GetLongIdsAsync<TObject>(int size = 1)
        {
            return GetLongIdsAsync(options =>
            {
                options.Size = size;
                options.ObjectName = GetObjectNameByType(typeof(TObject));
            });
        }

        /// <summary>
        /// Get long id
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <returns></returns>
        public static async Task<long> GetLongIdAsync<TObject>()
        {
            return (await GetLongIdsAsync(options =>
            {
                options.Size = 1;
                options.ObjectName = GetObjectNameByType(typeof(TObject));
            }).ConfigureAwait(false)).FirstOrDefault();
        }

        /// <summary>
        /// Get long ids
        /// <returns></returns>
        public static List<long> GetLongIds(Action<ObjectIdOptions> configure)
        {
            var objectIdOptions = new ObjectIdOptions();
            configure?.Invoke(objectIdOptions);
            var objectName = objectIdOptions.ObjectName;
            var size = objectIdOptions.Size;

            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(objectName), nameof(ObjectIdOptions.ObjectName));
            SixnetDirectThrower.ThrowArgErrorIf(size < 1, nameof(ObjectIdOptions.Size));

            var objectKey = GetObjectIdKey(objectIdOptions.ObjectName);
            var incrResponse = SixnetCacher.String.Increment(new StringIncrementParameter()
            {
                CacheObject = _idCacheObject,
                Key = objectKey,
                Value = objectIdOptions.Size
            });

            SixnetDirectThrower.ThrowIf<SixnetApplicationException>(!(incrResponse?.Success ?? false), incrResponse?.Message);

            var newValue = incrResponse.NewValue;
            var newIds = new List<long>();
            var beginId = newValue - size;
            for (var i = beginId + 1; i <= newValue; i++)
            {
                newIds.Add(i);
            }
            return newIds;
        }

        /// <summary>
        /// Get long ids
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="size"></param>
        /// <returns></returns>
        public static List<long> GetLongIds<TObject>(int size = 1)
        {
            return GetLongIds(options =>
            {
                options.Size = size;
                options.ObjectName = GetObjectNameByType(typeof(TObject));
            });
        }

        /// <summary>
        /// Get long id
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <returns></returns>
        public static long GetLongId<TObject>()
        {
            return GetLongIds(options =>
            {
                options.Size = 1;
                options.ObjectName = GetObjectNameByType(typeof(TObject));
            }).FirstOrDefault();
        }

        #endregion

        #region Integer

        /// <summary>
        /// Get long ids
        /// <returns></returns>
        public static async Task<List<int>> GetIntIdsAsync(Action<ObjectIdOptions> configure)
        {
            var objectIdOptions = new ObjectIdOptions();
            configure?.Invoke(objectIdOptions);
            var objectName = objectIdOptions.ObjectName;
            var size = objectIdOptions.Size;

            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(objectName), nameof(ObjectIdOptions.ObjectName));
            SixnetDirectThrower.ThrowArgErrorIf(size < 1, nameof(ObjectIdOptions.Size));

            var objectKey = GetObjectIdKey(objectIdOptions.ObjectName);
            var incrResponse = await SixnetCacher.String.IncrementAsync(new StringIncrementParameter()
            {
                CacheObject = _idCacheObject,
                Key = objectKey,
                Value = objectIdOptions.Size
            }).ConfigureAwait(false);

            SixnetDirectThrower.ThrowIf<SixnetApplicationException>(!(incrResponse?.Success ?? false), incrResponse?.Message);

            var newValue = incrResponse.NewValue;
            var newIds = new List<int>();
            var beginId = newValue - size;
            for (var i = beginId + 1; i <= newValue; i++)
            {
                SixnetThrower.ThrowIf<ArgumentOutOfRangeException>(i > int.MaxValue);
                newIds.Add(Convert.ToInt32(i));
            }
            return newIds;
        }

        /// <summary>
        /// Get long ids
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Task<List<int>> GetIntIdsAsync<TObject>(int size = 1)
        {
            return GetIntIdsAsync(options =>
            {
                options.Size = size;
                options.ObjectName = GetObjectNameByType(typeof(TObject));
            });
        }

        /// <summary>
        /// Get long id
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <returns></returns>
        public static async Task<int> GetIntIdAsync<TObject>()
        {
            return (await GetIntIdsAsync(options =>
            {
                options.Size = 1;
                options.ObjectName = GetObjectNameByType(typeof(TObject));
            }).ConfigureAwait(false)).FirstOrDefault();
        }

        /// <summary>
        /// Get long ids
        /// <returns></returns>
        public static List<int> GetIntIds(Action<ObjectIdOptions> configure)
        {
            var objectIdOptions = new ObjectIdOptions();
            configure?.Invoke(objectIdOptions);
            var objectName = objectIdOptions.ObjectName;
            var size = objectIdOptions.Size;

            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(objectName), nameof(ObjectIdOptions.ObjectName));
            SixnetDirectThrower.ThrowArgErrorIf(size < 1, nameof(ObjectIdOptions.Size));

            var objectKey = GetObjectIdKey(objectIdOptions.ObjectName);
            var incrResponse = SixnetCacher.String.Increment(new StringIncrementParameter()
            {
                CacheObject = _idCacheObject,
                Key = objectKey,
                Value = objectIdOptions.Size
            });

            SixnetDirectThrower.ThrowIf<SixnetApplicationException>(!(incrResponse?.Success ?? false), incrResponse?.Message);

            var newValue = incrResponse.NewValue;
            var newIds = new List<int>();
            var beginId = newValue - size;
            for (var i = beginId + 1; i <= newValue; i++)
            {
                SixnetThrower.ThrowIf<ArgumentOutOfRangeException>(i > int.MaxValue);
                newIds.Add(Convert.ToInt32(i));
            }
            return newIds;
        }

        /// <summary>
        /// Get long ids
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="size"></param>
        /// <returns></returns>
        public static List<int> GetIntIds<TObject>(int size = 1)
        {
            return GetIntIds(options =>
            {
                options.Size = size;
                options.ObjectName = GetObjectNameByType(typeof(TObject));
            });
        }

        /// <summary>
        /// Get long id
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <returns></returns>
        public static int GetIntId<TObject>()
        {
            return GetIntIds(options =>
            {
                options.Size = 1;
                options.ObjectName = GetObjectNameByType(typeof(TObject));
            }).FirstOrDefault();
        }

        #endregion

        #region Init

        /// <summary>
        /// Init object ids
        /// </summary>
        /// <param name="objectIds">Object ids</param>
        public static void InitObjectIds(Dictionary<string, long> objectIds)
        {
            if (objectIds.IsNullOrEmpty())
            {
                return;
            }
            var parameter = new StringSetParameter()
            {
                CacheObject = _idCacheObject,
                Items = objectIds.Select(c => new CacheEntry
                {
                    Key = GetObjectIdKey(c.Key),
                    Value = c.Value.ToString()
                }).ToList()
            };
            SixnetCacher.String.Set(parameter);
        }

        /// <summary>
        /// Init object ids
        /// </summary>
        /// <param name="objectIds">Object ids</param>
        public static void InitObjectIds(Dictionary<Type, long> objectIds)
        {
            InitObjectIds(objectIds?.ToDictionary(c => GetObjectNameByType(c.Key), c => c.Value));
        }

        #endregion

        #region Utils

        static string GetObjectIdKey(string objecName)
        {
            var cacheOptions = SixnetCacher.Options;
            var splitChar = cacheOptions.KeyNameSplitChar;
            return $"{objecName}{splitChar}Id";
        }

        static string GetObjectNameByType(Type objectType)
        {
            return objectType.FullName.Replace(".", SixnetCacher.Options.KeyNameSplitChar);
        }

        #endregion
    }
}
