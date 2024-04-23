using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using AutoMapper;
using Sixnet.Code;
using Sixnet.Exceptions;

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
                //[typeof(byte?)] = DbType.Byte,
                //[typeof(sbyte?)] = DbType.SByte,
                //[typeof(short?)] = DbType.Int16,
                //[typeof(ushort?)] = DbType.UInt16,
                //[typeof(int?)] = DbType.Int32,
                //[typeof(uint?)] = DbType.UInt32,
                //[typeof(long?)] = DbType.Int64,
                //[typeof(ulong?)] = DbType.UInt64,
                //[typeof(float?)] = DbType.Single,
                //[typeof(double?)] = DbType.Double,
                //[typeof(decimal?)] = DbType.Decimal,
                //[typeof(bool?)] = DbType.Boolean,
                //[typeof(char?)] = DbType.StringFixedLength,
                //[typeof(Guid?)] = DbType.Guid,
                //[typeof(DateTime?)] = DbType.DateTime,
                //[typeof(DateTimeOffset?)] = DbType.DateTimeOffset,
                //[typeof(TimeSpan?)] = DbType.Time,
                [typeof(object)] = DbType.Object
            };
        }

        #region Generate dictionary by enum

        /// <summary>
        /// Generate dictionary by enum
        /// </summary>
        /// <param name="enumType">Enum type</param>
        /// <param name="startWithEnumName">Whether start with enum name</param>
        /// <param name="displayFriendly">Display friendly</param>
        /// <returns>Return the dictionary value</returns>
        public static Dictionary<int, string> GetEnumValueAndNames(this Type enumType, bool startWithEnumName = false, bool displayFriendly = true)
        {
            if (enumType == null)
            {
                return new Dictionary<int, string>(0);
            }

            string formatedKey = $"{enumType.GUID}_{startWithEnumName}_{displayFriendly}";
            if (CacheEnumValueAndNames.TryGetValue(formatedKey, out var valueAndNames))
            {
                return valueAndNames ?? new Dictionary<int, string>(0);
            }
            var values = Enum.GetValues(enumType);
            Dictionary<int, string> enumValues = new Dictionary<int, string>();
            var displayAttrType = typeof(DisplayAttribute);
            foreach (int val in values)
            {
                var enumName = Enum.GetName(enumType, val);
                if (displayFriendly)
                {
                    var enumField = enumType.GetField(enumName);

                    if (enumField != null && enumField.IsDefined(displayAttrType, false))
                    {
                        var displayName = (enumField.GetCustomAttributes(displayAttrType, false).First() as DisplayAttribute)?.Name;
                        enumName = string.IsNullOrWhiteSpace(displayName) ? enumName : displayName;
                    }
                }
                enumValues.Add(val, startWithEnumName ? $"{enumType.Name}{enumName}" : enumName);
            }
            CacheEnumValueAndNames[formatedKey] = enumValues;
            return enumValues;
        }

        #endregion

        #region Get enum display name

        /// <summary>
        /// Get enum display name
        /// </summary>
        /// <param name="enumValue">Enum value</param>
        /// <returns></returns>
        public static string GetEnumDisplayName(this Enum enumValue)
        {
            var enumType = enumValue.GetType();
            var valueAndNames = GetEnumValueAndNames(enumType, false, true);
            valueAndNames.TryGetValue(Convert.ToInt32(enumValue), out string displayName);
            return displayName ?? enumValue.ToString();
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
            dynamic defaultValue;
            switch (dbType)
            {
                case DbType.Byte:
                    defaultValue = default(byte);
                    break;
                case DbType.SByte:
                    defaultValue = default(sbyte);
                    break;
                case DbType.Int16:
                    defaultValue = default(short);
                    break;
                case DbType.UInt16:
                    defaultValue = default(ushort);
                    break;
                case DbType.Int32:
                case DbType.VarNumeric:
                    defaultValue = default(int);
                    break;
                case DbType.UInt32:
                    defaultValue = default(uint);
                    break;
                case DbType.Int64:
                    defaultValue = default(long);
                    break;
                case DbType.UInt64:
                    defaultValue = default(ulong);
                    break;
                case DbType.Double:
                    defaultValue = default(double);
                    break;
                case DbType.Single:
                    defaultValue = default(float);
                    break;
                case DbType.Decimal:
                case DbType.Currency:
                    defaultValue = default(decimal);
                    break;
                case DbType.String:
                case DbType.AnsiString:
                case DbType.Xml:
                    defaultValue = default(string);
                    break;
                case DbType.StringFixedLength:
                case DbType.AnsiStringFixedLength:
                    defaultValue = default(char);
                    break;
                case DbType.Boolean:
                    defaultValue = default(bool);
                    break;
                case DbType.DateTime:
                case DbType.Date:
                case DbType.DateTime2:
                    defaultValue = default(DateTime);
                    break;
                case DbType.DateTimeOffset:
                    defaultValue = default(DateTimeOffset);
                    break;
                case DbType.Guid:
                    defaultValue = default(Guid);
                    break;
                case DbType.Time:
                    defaultValue = default(TimeSpan);
                    break;
                case DbType.Binary:
                    defaultValue = default(byte[]);
                    break;
                default:
                    defaultValue = null;
                    break;
            }
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
            if (dataType == typeof(DateTimeOffset))
            {
                return DateTimeOffset.Now;
            }
            if (dataType == typeof(DateTime))
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
    }
}
