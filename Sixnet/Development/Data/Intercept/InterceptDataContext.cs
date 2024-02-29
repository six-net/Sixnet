using System;
using System.Collections.Generic;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Data.Field;
using Sixnet.Exceptions;

namespace Sixnet.Development.Data.Intercept
{
    /// <summary>
    /// Defines intercept data context
    /// </summary>
    public class InterceptDataContext
    {
        /// <summary>
        /// Get or set the data command
        /// </summary>
        internal SixnetDataCommand DataCommand { get; set; }

        /// <summary>
        /// Get the entity type
        /// </summary>
        /// <returns></returns>
        public Type GetEntityType()
        {
            return DataCommand?.GetEntityType();
        }

        /// <summary>
        /// Get the operation type
        /// </summary>
        /// <returns></returns>
        public DataOperationType GetDataOperationType()
        {
            return DataCommand.OperationType;
        }

        /// <summary>
        /// Whether has new value
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <returns></returns>
        public bool HasNewValue(string fieldName)
        {
            return DataCommand.FieldsAssignment?.HasNewValue(fieldName) ?? false;
        }

        /// <summary>
        /// Whether has old value
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <returns></returns>
        public bool HasOldValue(string fieldName)
        {
            return DataCommand.FieldsAssignment?.HasOldValue(fieldName) ?? false;
        }

        /// <summary>
        /// Get new value
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <returns></returns>
        public dynamic GetNewValue(string fieldName)
        {
            SixnetDirectThrower.ThrowSixnetExceptionIf(!HasNewValue(fieldName), $"Not set new value for {fieldName}");
            return DataCommand.FieldsAssignment?.GetNewValue(fieldName);
        }

        /// <summary>
        /// Get original value
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <returns></returns>
        public dynamic GetOldValue(string fieldName)
        {
            SixnetDirectThrower.ThrowSixnetExceptionIf(!HasOldValue(fieldName), $"Not has original value for {fieldName}");
            return DataCommand.FieldsAssignment?.GetOldValue(fieldName);
        }

        /// <summary>
        /// Set new value
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        public void SetNewValue(string fieldName, dynamic value)
        {
            if (!string.IsNullOrWhiteSpace(fieldName))
            {
                DataCommand.FieldsAssignment?.SetNewValue(fieldName, value);
            }
        }

        /// <summary>
        /// Whether allow update new value
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        internal bool AllowUpdateNewValue(Type valueType, string fieldName)
        {
            var hasValue = HasNewValue(fieldName);
            if (hasValue)
            {
                var value = GetNewValue(fieldName);
                var realValue = value;
                if (value is ISixnetDataField dataField)
                {
                    if (dataField is ConstantField constantField && constantField.IsSimpleConstant)
                    {
                        realValue = constantField.Value;
                    }
                    else
                    {
                        return false;
                    }
                }
                return TypeExtensions.IsDefaultValue(valueType, realValue);
            }
            return true;
        }
    }
}
