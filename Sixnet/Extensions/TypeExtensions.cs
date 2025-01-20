using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using AutoMapper;
using Sixnet.Code;
using Sixnet.DependencyInjection;
using Sixnet.Exceptions;
using Sixnet.Extensions;

namespace System
{
    /// <summary>
    /// Type extensions
    /// </summary>
    public static class TypeExtensions
    {
        #region Fields

        /// <summary>
        /// Allow null type
        /// </summary>
        static readonly Type AllowNullType = typeof(Nullable<>);

        /// <summary>
        /// Enum value and names
        /// </summary>
        static readonly ConcurrentDictionary<string, Dictionary<int, string>> CacheEnumValueAndNames = new();

        /// <summary>
        /// DbType mapping
        /// </summary>
        static Dictionary<Type, DbType> dbTypeMapping;

        #endregion

        static TypeExtensions()
        {
            dbTypeMapping = new Dictionary<Type, DbType>(37)
            {
                [typeof(byte)] = DbType.Byte,
                [typeof(sbyte)] = DbType.SByte,
                [typeof(short)] = DbType.Int16,
                [typeof(ushort)] = DbType.UInt16,
                [typeof(int)] = DbType.Int32,
                [typeof(uint)] = DbType.UInt32,
                [typeof(long)] = DbType.Int64,
                [typeof(ulong)] = DbType.UInt64,
                [typeof(float)] = DbType.Single,
                [typeof(double)] = DbType.Double,
                [typeof(decimal)] = DbType.Decimal,
                [typeof(bool)] = DbType.Boolean,
                [typeof(string)] = DbType.String,
                [typeof(char)] = DbType.StringFixedLength,
                [typeof(Guid)] = DbType.Guid,
                [typeof(DateTime)] = DbType.DateTime,
                [typeof(DateTimeOffset)] = DbType.DateTimeOffset,
                [typeof(TimeSpan)] = DbType.Time,
                [typeof(byte[])] = DbType.Binary,
                [typeof(object)] = DbType.Object
            };
        }

        #region Generate dictionary by enum

        /// <summary>
        /// Generate dictionary by enum
        /// </summary>
        /// <param name="enumType">Enum type</param>
        /// <param name="options">Options</param>
        /// <returns>Return the dictionary value</returns>
        public static Dictionary<int, string> GetEnumValueAndNames(this Type enumType, SixnetEnumOptions options)
        {
            if (enumType == null)
            {
                return new Dictionary<int, string>(0);
            }
            options ??= SixnetContainer.GetOptions<SixnetEnumOptions>();
            var formatedKey = $"{enumType.GUID}_{options.GetOptionsIdentityKey()}";
            if (CacheEnumValueAndNames.TryGetValue(formatedKey, out var valueAndNames))
            {
                return valueAndNames ?? new Dictionary<int, string>(0);
            }
            var values = Enum.GetValues(enumType);
            var enumValues = new Dictionary<int, string>();
            var displayAttrType = typeof(DisplayAttribute);
            foreach (int val in values)
            {
                var enumName = Enum.GetName(enumType, val);
                // display
                if (!options.NotOutputDisplayName)
                {
                    var enumField = enumType.GetField(enumName);

                    if (enumField != null && enumField.IsDefined(displayAttrType, false))
                    {
                        var displayName = (enumField.GetCustomAttributes(displayAttrType, false).First() as DisplayAttribute)?.Name;
                        enumName = string.IsNullOrWhiteSpace(displayName) ? enumName : displayName;
                    }
                }
                // type name
                enumName = options.NotStartByTypeName ? enumName : $"{enumType.Name}{enumName}";
                // separate
                if (!options.NotSeparateName)
                {
                    enumName = enumName.ToSeparatorCase(options.NameSeparateChar, options.UppercaseName);
                }
                // upper case
                else if (!options.UppercaseName)
                {
                    enumName = enumName.ToLower();
                }
                enumValues.Add(val, enumName);
            }
            CacheEnumValueAndNames[formatedKey] = enumValues;
            return enumValues;
        }

        #endregion

        #region Allow set null value

        /// <summary>
        /// Allow set null value
        /// </summary>
        /// <param name="type">Type object</param>
        /// <returns>Return whether the type instance allow to set null</returns>
        public static bool AllowNull(this Type type)
        {
            return !type.IsValueType || (type.IsGenericType && AllowNullType.Equals(type.GetGenericTypeDefinition()));
        }

        #endregion

        #region Get real value type

        /// <summary>
        /// Get real value type
        /// </summary>
        /// <param name="originalType">Original type</param>
        /// <returns>Return the real value type</returns>
        public static Type GetRealValueType(this Type originalType)
        {
            if (originalType.IsGenericType && typeof(Nullable<>).Equals(originalType.GetGenericTypeDefinition()))
            {
                return originalType.GenericTypeArguments[0];
            }
            return originalType;
        }

        #endregion

        #region Get type default value

        /// <summary>
        /// Get type default value
        /// </summary>
        /// <param name="dataType">Data type</param>
        /// <returns></returns>
        public static dynamic GetDefaultValue(this Type dataType)
        {
            if (dataType == null)
            {
                throw new ArgumentNullException(nameof(dataType));
            }
            var dbType = dataType.GetDbType();
            dynamic defaultValue = dbType switch
            {
                DbType.Byte => default(byte),
                DbType.SByte => default(sbyte),
                DbType.Int16 => default(short),
                DbType.UInt16 => default(ushort),
                DbType.Int32 or DbType.VarNumeric => default(int),
                DbType.UInt32 => default(uint),
                DbType.Int64 => default(long),
                DbType.UInt64 => default(ulong),
                DbType.Double => default(double),
                DbType.Single => default(float),
                DbType.Decimal or DbType.Currency => default(decimal),
                DbType.String or DbType.AnsiString or DbType.Xml => default(string),
                DbType.StringFixedLength or DbType.AnsiStringFixedLength => default(char),
                DbType.Boolean => default(bool),
                DbType.DateTime or DbType.Date or DbType.DateTime2 => default(DateTime),
                DbType.DateTimeOffset => default(DateTimeOffset),
                DbType.Guid => default(Guid),
                DbType.Time => default(TimeSpan),
                DbType.Binary => default(byte[]),
                _ => null,
            };
            return defaultValue;
        }

        /// <summary>
        /// Whether is default value
        /// </summary>
        /// <param name="dataType">Data type</param>
        /// <param name="value">Value</param>
        /// <returns></returns>
        internal static bool IsDefaultValue(this Type dataType, dynamic value)
        {
            var defaultValue = dataType.GetDefaultValue();
            return defaultValue == value;
        }

        #endregion

        #region Get now datetime

        /// <summary>
        /// Get now datetime
        /// </summary>
        /// <param name="dataType">Data type</param>
        /// <returns></returns>
        internal static dynamic GetNowDateTime(this Type dataType)
        {
            if (dataType == typeof(DateTimeOffset) || dataType == typeof(DateTimeOffset?))
            {
                return DateTimeOffset.Now;
            }
            if (dataType == typeof(DateTime) || dataType == typeof(DateTime?))
            {
                return DateTime.Now;
            }
            throw new NotSupportedException(dataType.FullName);
        }

        #endregion

        #region Get dbtype

        /// <summary>
        /// Get db type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DbType GetDbType(this Type type)
        {
            SixnetDirectThrower.ThrowArgNullIf(type == null, nameof(type));
            var valueType = type.GetRealValueType();
            if (valueType.IsEnum)
            {
                return DbType.Int32;
            }
            SixnetDirectThrower.ThrowNotSupportIf(!dbTypeMapping.ContainsKey(valueType), valueType.FullName);
            return dbTypeMapping[valueType];
        }

        #endregion

        #region Get type identity key

        /// <summary>
        /// Get type identity key
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTypeIdentityKey(this Type type)
        {
            if (type == null)
            {
                return string.Empty;
            }
            if (type.IsGenericType)
            {
                return string.Join("-", ResolveGenericTypeKeys(type));
            }
            else
            {
                return type.GUID.ToString();
            }
        }

        static List<string> ResolveGenericTypeKeys(Type type)
        {
            var keys = new List<string>() { type.GUID.ToString() };
            foreach (var argType in type.GenericTypeArguments)
            {
                if (argType.IsGenericType)
                {
                    var argTypeKeys = ResolveGenericTypeKeys(argType);
                    keys.AddRange(argTypeKeys);
                }
                else
                {
                    keys.Add(argType.GUID.ToString());
                }
            }
            return keys;
        }

        #endregion
    }
}
