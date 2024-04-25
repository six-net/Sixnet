using System;
using System.Collections.Generic;
using System.Linq;
using Sixnet.Code;
using Sixnet.Validation;
using Sixnet.Development.Data.Field;
using Sixnet.Exceptions;
using static Sixnet.Validation.ValidationConstants;
using Sixnet.Serialization.Binary;

namespace Sixnet.Development.Entity
{
    /// <summary>
    /// Sixnet base entity
    /// </summary>
    public abstract class SixnetBaseEntity<T> : ISixnetEntity<T> where T : class, ISixnetEntity<T>
    {
        #region Fields

        [NotEntityField]
        protected string identityRealValue = string.Empty;
        [NotEntityField]
        protected bool loadedIdentityValue = false;
        [NotEntityField]
        protected static Type entityType = typeof(T);

        #endregion

        #region Methods

        #region Save

        /// <summary>
        /// Whether allow to save
        /// </summary>
        /// <returns></returns>
        public bool AllowToSave()
        {
            return SaveValidation();
        }

        /// <summary>
        /// Save validation
        /// </summary>
        /// <returns>Return whether allow to save</returns>
        protected virtual bool SaveValidation()
        {
            this.Validate("", false, UseCaseNames.Domain);
            return true;
        }

        #endregion

        #region Delete

        /// <summary>
        /// Whether allow to delete
        /// </summary>
        /// <returns></returns>
        public bool AllowToDelete()
        {
            return DeleteValidation();
        }

        /// <summary>
        /// Delete validation
        /// </summary>
        /// <returns>Whether allow to remove</returns>
        protected virtual bool DeleteValidation()
        {
            return !IdentityValueIsNull();
        }

        #endregion

        #region Identity

        /// <summary>
        /// Init identity value
        /// </summary>
        public virtual void InitIdentityValue()
        {
            var primaryKeys = SixnetEntityManager.GetPrimaryKeyNames<T>();
            if (primaryKeys.IsNullOrEmpty())
            {
                return;
            }
            foreach (var pk in primaryKeys)
            {
                var field = SixnetEntityManager.GetField(typeof(T), pk);
                if (field != null)
                {
                    var valueType = field.DataType?.GetRealValueType();
                    var typeCode = Type.GetTypeCode(valueType);
                    switch (typeCode)
                    {
                        case TypeCode.Byte:
                            SetValue(pk, RandomNumberHelper.GetRandomNumber(byte.MaxValue, 1));
                            break;
                        case TypeCode.SByte:
                            SetValue(pk, RandomNumberHelper.GetRandomNumber(sbyte.MaxValue, 1));
                            break;
                        case TypeCode.Int16:
                            SetValue(pk, RandomNumberHelper.GetRandomNumber(short.MaxValue, 1));
                            break;
                        case TypeCode.UInt16:
                            SetValue(pk, RandomNumberHelper.GetRandomNumber(ushort.MaxValue, 1));
                            break;
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                            SetValue(pk, RandomNumberHelper.GetRandomNumber(int.MaxValue, 1));
                            break;
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Double:
                        case TypeCode.Single:
                        case TypeCode.Decimal:
                            SetValue(pk, SerialNumber.GenerateSerialNumber<T>());
                            break;
                        case TypeCode.String:
                            SetValue(pk, SerialNumber.GenerateSerialNumber<T>().ToString());
                            break;
                        case TypeCode.DateTime:
                            SetValue(pk, DateTime.Now);
                            break;
                        default:
                            if (valueType == typeof(Guid))
                            {
                                SetValue(pk, Guid.NewGuid());
                            }
                            else if (valueType == typeof(DateTimeOffset))
                            {
                                SetValue(pk, DateTimeOffset.Now);
                            }
                            else
                            {
                                throw new InvalidOperationException($"Initialization values are not supported for {field.DataType}.");
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Whether identity value is null
        /// </summary>
        /// <returns>Return identity value whether has value</returns>
        public virtual bool IdentityValueIsNull()
        {
            var primaryKeys = SixnetEntityManager.GetPrimaryKeyNames<T>();
            if (primaryKeys.IsNullOrEmpty())
            {
                return true;
            }
            bool identityValueIsNull = false;
            foreach (var pk in primaryKeys)
            {
                var field = SixnetEntityManager.GetField(typeof(T), pk);
                identityValueIsNull = field == null;
                if (!identityValueIsNull)
                {
                    var valueType = field.DataType?.GetRealValueType();
                    var typeCode = Type.GetTypeCode(valueType);
                    var fieldValue = field.ValueProvider.Get(this);
                    switch (typeCode)
                    {
                        case TypeCode.Byte:
                            identityValueIsNull = !byte.TryParse(fieldValue?.ToString(), out var byteValue) || byteValue < 1;
                            break;
                        case TypeCode.SByte:
                            identityValueIsNull = !sbyte.TryParse(fieldValue?.ToString(), out var sbyteValue) || sbyteValue < 1;
                            break;
                        case TypeCode.Int16:
                            identityValueIsNull = !short.TryParse(fieldValue?.ToString(), out var shortValue) || shortValue < 1;
                            break;
                        case TypeCode.UInt16:
                            identityValueIsNull = !ushort.TryParse(fieldValue?.ToString(), out var ushortValue) || ushortValue < 1;
                            break;
                        case TypeCode.Int32:
                            identityValueIsNull = !int.TryParse(fieldValue?.ToString(), out var intValue) || intValue < 1;
                            break;
                        case TypeCode.UInt32:
                            identityValueIsNull = !uint.TryParse(fieldValue?.ToString(), out var uintValue) || uintValue < 1;
                            break;
                        case TypeCode.Int64:
                            identityValueIsNull = !long.TryParse(fieldValue?.ToString(), out var longValue) || longValue < 1;
                            break;
                        case TypeCode.UInt64:
                            identityValueIsNull = !ulong.TryParse(fieldValue?.ToString(), out var ulongValue) || ulongValue < 1;
                            break;
                        case TypeCode.Double:
                        case TypeCode.Single:
                            identityValueIsNull = !double.TryParse(fieldValue?.ToString(), out var doubleValue) || doubleValue < 1;
                            break;
                        case TypeCode.Decimal:
                            identityValueIsNull = !decimal.TryParse(fieldValue?.ToString(), out var decimalValue) || decimalValue < 1;
                            break;
                        case TypeCode.String:
                            identityValueIsNull = string.IsNullOrWhiteSpace(fieldValue?.ToString());
                            break;
                        case TypeCode.DateTime:
                            identityValueIsNull = !DateTime.TryParse(fieldValue?.ToString(), out var dateTimeValue) || dateTimeValue <= DateTime.MinValue;
                            break;
                        default:
                            if (valueType == typeof(Guid))
                            {
                                identityValueIsNull = !Guid.TryParse(fieldValue?.ToString(), out var guidValue) || guidValue.Equals(Guid.Empty);
                            }
                            else if (valueType == typeof(DateTimeOffset))
                            {
                                identityValueIsNull = !DateTimeOffset.TryParse(fieldValue?.ToString(), out var dateTimeOffsetValue) && dateTimeOffsetValue <= DateTimeOffset.MinValue;
                            }
                            else
                            {
                                throw new InvalidOperationException($"Not supported check value for {field.DataType}.");
                            }
                            break;
                    }
                }
                if (identityValueIsNull)
                {
                    break;
                }
            }
            return identityValueIsNull;
        }

        /// <summary>
        /// Gets the identity value
        /// </summary>
        /// <returns>Return identity value</returns>
        public virtual string GetIdentityValue()
        {
            if (loadedIdentityValue)
            {
                return identityRealValue;
            }
            var primaryValues = GetPrimaryKeyValues();
            identityRealValue = primaryValues.IsNullOrEmpty() ? Guid.NewGuid().ToString() : string.Join("_", primaryValues.Values.OrderBy(c => c?.ToString() ?? string.Empty));
            loadedIdentityValue = true;
            return identityRealValue;
        }

        #endregion

        #region Updating

        /// <summary>
        /// Update data
        /// </summary>
        /// <returns></returns>
        public void OnDataUpdating()
        {
            OnUpdating();
        }

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="newEntity">New entity</param>
        /// <returns></returns>
        internal protected virtual void OnUpdating()
        {
        }

        /// <summary>
        /// Modify from new data
        /// </summary>
        /// <param name="newData">New data</param>
        /// <param name="configure">Configure</param>
        public virtual void ModifyFrom(T newData, Action<ModifyEntityOptions> configure = null)
        {
            if (newData != null && newData != this)
            {
                var modificationValues = GetModificationValues(newData.GetAllValues(), configure, false, out _);
                foreach (var valueItem in modificationValues)
                {
                    SetValue(valueItem.Key, valueItem.Value);
                }
            }
        }

        /// <summary>
        /// Get modification assignment
        /// </summary>
        /// <param name="newData">New data</param>
        /// <param name="configure">Configure</param>
        public virtual FieldsAssignment GetModificationAssignment(T newData, Action<ModifyEntityOptions> configure = null)
        {
            var fieldsAssignment = FieldsAssignment.Create();
            var modificationValues = GetModificationValues(newData?.GetAllValues(), configure, newData == this, out var oldValues);
            foreach (var valueItem in modificationValues)
            {
                fieldsAssignment.SetNewValue(valueItem.Key, valueItem.Value);
            }
            fieldsAssignment.OldValues = oldValues;
            return fieldsAssignment;
        }

        /// <summary>
        /// Get modification values
        /// </summary>
        /// <param name="newValues"></param>
        /// <param name="configure"></param>
        /// <param name="selfData"></param>
        /// <param name="oldValues"></param>
        /// <returns></returns>
        Dictionary<string, dynamic> GetModificationValues(Dictionary<string, dynamic> newValues, Action<ModifyEntityOptions> configure
            , bool selfData, out Dictionary<string, dynamic> oldValues)
        {
            oldValues = selfData ? newValues : GetAllValues();
            if (newValues.IsNullOrEmpty())
            {
                return new Dictionary<string, dynamic>(0);
            }
            var modifyOptions = new ModifyEntityOptions();
            configure?.Invoke(modifyOptions);
            var modificationValues = new Dictionary<string, dynamic>();
            foreach (var valueItem in newValues)
            {
                var field = SixnetEntityManager.GetField(entityType, valueItem.Key);
                if (!modifyOptions.IsIgnoreField(field))
                {
                    if (selfData)
                    {
                        modificationValues[valueItem.Key] = valueItem.Value;
                    }
                    else
                    {
                        if (!(oldValues?.ContainsKey(valueItem.Key) ?? false))
                        {
                            modificationValues[valueItem.Key] = valueItem.Value;
                        }
                        else
                        {
                            var oldValue = oldValues[valueItem.Key];
                            var newValue = valueItem.Value;
                            bool isValueType = field.DataType.IsValueType || typeof(string).IsAssignableFrom(field.DataType);
                            bool isUpdated = isValueType
                                ? oldValue != newValue
                                : field.DataType.IsSerializable
                                                ? SixnetBinarySerializer.SerializeObjectToString(oldValue) != SixnetBinarySerializer.SerializeObjectToString(newValue)
                                                : oldValue != newValue;
                            if (isUpdated)
                            {
                                modificationValues[valueItem.Key] = valueItem.Value;
                            }
                        }
                    }
                }
            }
            return modificationValues;
        }

        #endregion

        #region Adding

        /// <summary>
        /// Add data
        /// </summary>
        /// <returns></returns>
        public void OnDataAdding()
        {
            if (IdentityValueIsNull())
            {
                InitIdentityValue();
            }
        }

        /// <summary>
        /// Add data
        /// </summary>
        /// <returns></returns>
        internal protected virtual void OnAdding()
        {
        }

        #endregion

        #region Equal

        public virtual bool Equals(T targetObj)
        {
            return targetObj?.GetIdentityValue() == GetIdentityValue();
        }

        public override bool Equals(object data)
        {
            return Equals(data as T);
        }

        /// <summary>
        /// Get hash code
        /// </summary>
        /// <returns>Return hash code</returns>
        public override int GetHashCode()
        {
            return GetIdentityValue()?.GetHashCode() ?? 0;
        }

        #endregion

        #region Value

        /// <summary>
        /// Get value provider
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <returns></returns>
        ISixnetEntityPropertyValueProvider GetValueProvider(string propertyName)
        {
            var valueProvider = SixnetEntityManager.GetField(entityType, propertyName)?.ValueProvider;
            return valueProvider;
        }

        /// <summary>
        /// Gets the object primary key value
        /// </summary>
        /// <returns>Return the primary key values</returns>
        internal Dictionary<string, dynamic> GetPrimaryKeyValues()
        {
            var primaryKeys = SixnetEntityManager.GetPrimaryKeyNames(typeof(T));
            var keysCount = primaryKeys.GetCount();
            if (primaryKeys == null || keysCount <= 0)
            {
                return new Dictionary<string, dynamic>(0);
            }
            Dictionary<string, dynamic> values = new Dictionary<string, dynamic>(keysCount);
            foreach (var key in primaryKeys)
            {
                values.Add(key, GetValue(key));
            }
            return values;
        }

        /// <summary>
        /// Gets the property or field name
        /// </summary>
        /// <param name="name">Property or field name</param>
        /// <returns>Return the value</returns>
        public dynamic GetValue(string name)
        {
            var valueProvider = GetValueProvider(name);
            return valueProvider?.Get(this);
        }

        /// <summary>
        /// Gets the property or field name
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="name">Property or field name</param>
        /// <returns>Return the value</returns>
        public TValue GetValue<TValue>(string name)
        {
            var value = GetValue(name);
            if (value is TValue)
            {
                return value;
            }
            return value.ConvertTo<TValue>();
        }

        /// <summary>
        /// Sets the property or field value
        /// </summary>
        /// <param name="name">Property or field name</param>
        /// <param name="value">Value</param>
        public void SetValue(string name, dynamic value)
        {
            var valueProvider = GetValueProvider(name);
            valueProvider?.Set(this, value);
            if (loadedIdentityValue && SixnetEntityManager.IsPrimaryKey(entityType, name))
            {
                loadedIdentityValue = false;
            }
        }

        /// <summary>
        /// Gets all property or field values
        /// </summary>
        /// <returns>Return all property values</returns>
        public Dictionary<string, dynamic> GetAllValues()
        {
            var entityConfig = SixnetEntityManager.GetEntityConfig(entityType);

            SixnetDirectThrower.ThrowSixnetExceptionIf(entityConfig == null, $"{entityType.FullName}'s configuration is null");

            var allValues = new Dictionary<string, dynamic>(entityConfig.AllFields.Count);
            foreach (var field in entityConfig.AllFields)
            {
                SixnetDirectThrower.ThrowSixnetExceptionIf(field.Value?.ValueProvider == null, $"{entityType.FullName} => {field.Key}'s value provider is null");

                allValues[field.Key] = field.Value.ValueProvider.Get(this);
            }
            return allValues;
        }

        #endregion

        #endregion
    }
}
