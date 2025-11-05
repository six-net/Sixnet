// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache;
using Sixnet.Cache.String.Parameters;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Data.Field.Formatting;
using Sixnet.Development.Entity;
using Sixnet.Development.Queryable;
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
        /// Gets or sets the field name
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets size
        /// </summary>
        public int Size { get; set; } = 1;
    }

    /// <summary>
    /// Object id entry
    /// </summary>
    public struct ObjectIdEntry
    {
        /// <summary>
        /// Gets or sets the object name
        /// </summary>
        public string ObjectName { get; set; }

        /// <summary>
        /// Gets or sets the field name
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the value 
        /// </summary>
        public long Value { get; set; }
    }

    public static class ObjectIdHelper
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

            var objectKey = GetObjectIdKey(objectIdOptions.ObjectName, objectIdOptions.FieldName);
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
        /// <param name="fieldName">Field name</param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Task<List<long>> GetLongIdsAsync<TObject>(int size = 1, string fieldName = "")
        {
            return GetLongIdsAsync(options =>
            {
                options.Size = size;
                options.FieldName = fieldName;
                options.ObjectName = GetObjectNameByType(typeof(TObject));
            });
        }

        /// <summary>
        /// Get long id
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="fieldName">Field name</param>
        /// <returns></returns>
        public static async Task<long> GetLongIdAsync<TObject>(string fieldName = "")
        {
            return (await GetLongIdsAsync(options =>
            {
                options.Size = 1;
                options.FieldName = fieldName;
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

            var objectKey = GetObjectIdKey(objectIdOptions.ObjectName, objectIdOptions.FieldName);
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
        /// <param name="fieldName">Field name</param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static List<long> GetLongIds<TObject>(int size = 1, string fieldName = "")
        {
            return GetLongIds(options =>
            {
                options.Size = size;
                options.FieldName = fieldName;
                options.ObjectName = GetObjectNameByType(typeof(TObject));
            });
        }

        /// <summary>
        /// Get long id
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="fieldName">Field name</param>
        /// <returns></returns>
        public static long GetLongId<TObject>(string fieldName = "")
        {
            return GetLongIds(options =>
            {
                options.Size = 1;
                options.FieldName = fieldName;
                options.ObjectName = GetObjectNameByType(typeof(TObject));
            }).FirstOrDefault();
        }

        #endregion

        #region Integer

        /// <summary>
        /// Get int ids
        /// <returns></returns>
        public static async Task<List<int>> GetIntIdsAsync(Action<ObjectIdOptions> configure)
        {
            var objectIdOptions = new ObjectIdOptions();
            configure?.Invoke(objectIdOptions);
            var objectName = objectIdOptions.ObjectName;
            var size = objectIdOptions.Size;

            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(objectName), nameof(ObjectIdOptions.ObjectName));
            SixnetDirectThrower.ThrowArgErrorIf(size < 1, nameof(ObjectIdOptions.Size));

            var objectKey = GetObjectIdKey(objectIdOptions.ObjectName, objectIdOptions.FieldName);
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
                newIds.Add(System.Convert.ToInt32(i));
            }
            return newIds;
        }

        /// <summary>
        /// Get int ids
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="fieldName">Field name</param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Task<List<int>> GetIntIdsAsync<TObject>(int size = 1, string fieldName = "")
        {
            return GetIntIdsAsync(options =>
            {
                options.Size = size;
                options.FieldName = fieldName;
                options.ObjectName = GetObjectNameByType(typeof(TObject));
            });
        }

        /// <summary>
        /// Get int id
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="fieldName">Field name</param>
        /// <returns></returns>
        public static async Task<int> GetIntIdAsync<TObject>(string fieldName = "")
        {
            return (await GetIntIdsAsync(options =>
            {
                options.Size = 1;
                options.FieldName = fieldName;
                options.ObjectName = GetObjectNameByType(typeof(TObject));
            }).ConfigureAwait(false)).FirstOrDefault();
        }

        /// <summary>
        /// Get int ids
        /// <returns></returns>
        public static List<int> GetIntIds(Action<ObjectIdOptions> configure)
        {
            var objectIdOptions = new ObjectIdOptions();
            configure?.Invoke(objectIdOptions);
            var objectName = objectIdOptions.ObjectName;
            var size = objectIdOptions.Size;

            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(objectName), nameof(ObjectIdOptions.ObjectName));
            SixnetDirectThrower.ThrowArgErrorIf(size < 1, nameof(ObjectIdOptions.Size));

            var objectKey = GetObjectIdKey(objectIdOptions.ObjectName, objectIdOptions.FieldName);
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
                newIds.Add(System.Convert.ToInt32(i));
            }
            return newIds;
        }

        /// <summary>
        /// Get int ids
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="fieldName">Field name</param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static List<int> GetIntIds<TObject>(int size = 1, string fieldName = "")
        {
            return GetIntIds(options =>
            {
                options.Size = size;
                options.FieldName = fieldName;
                options.ObjectName = GetObjectNameByType(typeof(TObject));
            });
        }

        /// <summary>
        /// Get int id
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="fieldName">Field name</param>
        /// <returns></returns>
        public static int GetIntId<TObject>(string fieldName = "")
        {
            return GetIntIds(options =>
            {
                options.Size = 1;
                options.FieldName = fieldName;
                options.ObjectName = GetObjectNameByType(typeof(TObject));
            }).FirstOrDefault();
        }

        #endregion

        #region Init

        /// <summary>
        /// Init object ids
        /// </summary>
        /// <param name="objectIds">Object ids</param>
        public static void InitObjectIds(List<ObjectIdEntry> entries)
        {
            if (entries.IsNullOrEmpty())
            {
                return;
            }
            var parameter = new StringSetParameter()
            {
                CacheObject = _idCacheObject,
                Items = entries.Select(c => new CacheEntry
                {
                    Key = GetObjectIdKey(c.ObjectName, c.FieldName),
                    Value = c.Value.ToString()
                }).ToList()
            };
            SixnetCacher.String.Set(parameter);
        }

        /// <summary>
        /// Init app object ids
        /// </summary>
        public static void InitAppObjectIds()
        {
            var entityConfigs = SixnetEntityManager.GetAllEntityConfigs();
            var objectIdEntries = new List<ObjectIdEntry>();
            foreach (var entityConfig in entityConfigs)
            {
                var generadeIdFields = entityConfig.AllFields?.Where(c => c.Value.InRole(FieldRole.GeneratedId)).ToList();
                if (generadeIdFields.IsNullOrEmpty())
                {
                    continue;
                }
                foreach (var fieldItem in generadeIdFields)
                {
                    var field = fieldItem.Value;
                    var dataField = DataField.Create(field.PropertyName, entityConfig.EntityType, 0, null, field.FieldName);
                    dataField.FormatSetting = FieldFormatSetting.Create(FieldFormatterNames.MAX);
                    var maxValue = SixnetQuerier.Create()
                        .SetModelType(entityConfig.EntityType)
                        .Select(dataField)
                        .IgnoreIsolation()
                        .IncludeArchived()
                        .Scalar<long>();
                    maxValue = maxValue < field.StartValue ? field.StartValue : maxValue;
                    objectIdEntries.Add(new ObjectIdEntry()
                    {
                        ObjectName = GetObjectNameByType(entityConfig.EntityType),
                        FieldName = field.PropertyName,
                        Value = maxValue
                    });
                }
            }
            InitObjectIds(objectIdEntries);
        }

        #endregion

        #region Utils

        static string GetObjectIdKey(string objecName, string fieldName = "")
        {
            var cacheOptions = SixnetCacher.Options;
            var splitChar = cacheOptions.KeyNameSplitChar;
            var idKey = $"{objecName}{splitChar}GId";
            if (!string.IsNullOrWhiteSpace(fieldName))
            {
                idKey = $"{idKey}{splitChar}{fieldName}";
            }
            return idKey;
        }

        static string GetObjectNameByType(Type objectType)
        {
            return objectType.FullName.Replace(".", SixnetCacher.Options.KeyNameSplitChar);
        }

        #endregion
    }
}
