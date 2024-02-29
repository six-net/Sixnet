using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
using Sixnet.Development.Data.Field.Formatting;
using Sixnet.Development.Entity;
using Sixnet.Exceptions;
using Sixnet.Expressions.Linq;

namespace Sixnet.Development.Data.Field
{
    /// <summary>
    /// Property field
    /// </summary>
    public class PropertyField : ISixnetDataField
    {
        string identityValue = string.Empty;

        /// <summary>
        /// Gets the field identity
        /// </summary>
        public string FieldIdentity => identityValue;

        /// <summary>
        /// Gets or sets the model type
        /// </summary>
        public Type ModelType { get; set; }

        /// <summary>
        /// Gets or sets the data type
        /// </summary>
        public Type DataType { get; set; }

        /// <summary>
        /// Field name
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Get or set the field property name
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Get or set the model type index
        /// </summary>
        public int ModelTypeIndex { get; set; }

        /// <summary>
        /// Gets or sets the field format options
        /// </summary>
        public FieldFormatSetting FormatOption { get; set; }

        /// <summary>
        /// Whether has field formatter
        /// </summary>
        public bool HasFormatter => FormatOption != null;

        /// <summary>
        /// Whether is a constant field and no formatter
        /// </summary>
        public bool IsSimpleConstant => false;

        /// <summary>
        /// Create a regular field
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="modelType">Model type</param>
        /// <param name="modelTypeIndex">Model type index</param>
        /// <param name="fieldFormatOptions">Field conversion options</param>
        /// <param name="fieldName">Field name</param>
        /// <returns></returns>
        public static PropertyField Create(string propertyName, Type modelType = null, int modelTypeIndex = 0, FieldFormatSetting fieldFormatOption = null, string fieldName = "")
        {
            SixnetDirectThrower.ThrowArgErrorIf(string.IsNullOrWhiteSpace(propertyName), "Property name is null or empty");
            var field = new PropertyField()
            {
                PropertyName = propertyName,
                FieldName = string.IsNullOrWhiteSpace(fieldName) ? propertyName : fieldName,
                FormatOption = fieldFormatOption,
                ModelType = modelType,
                ModelTypeIndex = modelTypeIndex
            };
            field.identityValue = field.GetIdentity();
            return field;
        }

        /// <summary>
        /// Create a regular field
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="modelTypeIndex">Model type index</param>
        /// <param name="fieldFormatOptions">Field format options</param>
        /// <param name="fieldName">Field name</param>
        /// <returns></returns>
        public static PropertyField Create<TModel>(Expression<Func<TModel, dynamic>> field, int modelTypeIndex = 0, FieldFormatSetting fieldFormatOption = null, string fieldName = "")
        {
            var propertyName = SixnetExpressionHelper.GetExpressionLastPropertyName(field);
            return Create(propertyName, typeof(TModel), modelTypeIndex, fieldFormatOption, fieldName);
        }

        /// <summary>
        /// Create a regular field
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="modelTypeIndex">Model type index</param>
        /// <param name="formatterName">Field formatter name</param>
        /// <param name="fieldName">Field name</param>
        /// <returns></returns>
        public static PropertyField Create<TModel>(Expression<Func<TModel, dynamic>> field, int modelTypeIndex = 0, string formatterName = "", string fieldName = "")
        {
            var propertyName = SixnetExpressionHelper.GetExpressionLastPropertyName(field);
            return Create(propertyName, typeof(TModel), modelTypeIndex, FieldFormatSetting.Create(formatterName), fieldName);
        }

        string GetIdentity()
        {
            return $"{ModelType?.GUID}_{FieldName}_{PropertyName}";
        }

        /// <summary>
        /// Clone a field
        /// </summary>
        /// <returns></returns>
        public ISixnetDataField Clone()
        {
            return new PropertyField()
            {
                PropertyName = PropertyName,
                ModelTypeIndex = ModelTypeIndex,
                identityValue = identityValue,
                FieldName = FieldName,
                FormatOption = FormatOption?.Clone(),
                ModelType = ModelType,
            };
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
            {
                return obj is PropertyField targetField && (FieldName == targetField.FieldName && !string.IsNullOrEmpty(FieldName)) && ModelType == targetField.ModelType && PropertyName == targetField.PropertyName;
            }
            return true;
        }

        /// <summary>
        /// Get hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return $"{ModelType?.GUID}{FieldName}{PropertyName}".GetHashCode();
        }

        /// <summary>
        /// Indicates field has the specified role
        /// </summary>
        /// <param name="role">Field role</param>
        /// <returns></returns>
        public virtual bool InRole(FieldRole role)
        {
            return false;
        }

        /// <summary>
        /// Get model type
        /// </summary>
        /// <returns></returns>
        public Type GetModelType()
        {
            return ModelType;
        }

        /// <summary>
        /// Get field data type
        /// </summary>
        /// <returns></returns>
        public Type GetFieldDataType()
        {
            return DataType;
        }

        /// <summary>
        /// Get field name
        /// </summary>
        /// <returns></returns>
        public string GetFieldName()
        {
            return FieldName;
        }
    }
}
