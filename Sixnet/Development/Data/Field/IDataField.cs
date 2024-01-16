using System;
using System.Collections.Generic;
using System.Text;
using Sixnet.Development.Data.Field.Formatting;
using Sixnet.Development.Entity;
using Sixnet.Model;

namespace Sixnet.Development.Data.Field
{
    /// <summary>
    /// Data field contract
    /// </summary>
    public interface IDataField : IInnerClone<IDataField>
    {
        /// <summary>
        /// Whether is a constant field and no formatter
        /// </summary>
        bool IsSimpleConstant { get; }

        /// <summary>
        /// Whether has field formatter
        /// </summary>
        bool HasFormatter { get; }

        /// <summary>
        /// Get the field identity
        /// </summary>
        string FieldIdentity { get; }

        /// <summary>
        /// Get or set the field output name
        /// </summary>
        string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the field format options
        /// </summary>
        FieldFormatOption FormatOption { get; set; }

        /// <summary>
        /// Whether in field role
        /// </summary>
        /// <param name="fieldRole">Field role</param>
        /// <returns></returns>
        bool InRole(FieldRole fieldRole);

        /// <summary>
        /// Get model type
        /// </summary>
        /// <returns></returns>
        Type GetModelType();

        /// <summary>
        /// Get field data type
        /// </summary>
        /// <returns></returns>
        Type GetFieldDataType();

        /// <summary>
        /// Get field name
        /// </summary>
        /// <returns></returns>
        string GetFieldName();
    }

    public class DataFieldComparer : IEqualityComparer<IDataField>
    {

        public static DataFieldComparer DefaultComparer => new();

        public bool Equals(IDataField x, IDataField y)
        {
            if (x != y)
            {
                if (x is PropertyField xField && y is PropertyField yField)
                {
                    return string.Equals(xField.PropertyName, yField.PropertyName);
                }
                return false;
            }
            return true;
        }

        public int GetHashCode(IDataField obj)
        {
            return 0;
        }
    }
}
