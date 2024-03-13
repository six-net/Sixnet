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
    public interface ISixnetField : ISixnetCloneable<ISixnetField>
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
        FieldFormatSetting FormatSetting { get; set; }

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
        Type GetDataType();

        /// <summary>
        /// Get field name
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <returns></returns>
        string GetFieldName(DatabaseType databaseType);
    }
}
