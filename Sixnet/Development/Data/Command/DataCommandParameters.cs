using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Dapper;

namespace Sixnet.Development.Data.Command
{
    /// <summary>
    /// Data command parameters
    /// </summary>
    [Serializable]
    public class DataCommandParameters
    {
        /// <summary>
        /// Items
        /// </summary>
        public Dictionary<string, DataCommandParameterItem> Items { get; private set; } = new Dictionary<string, DataCommandParameterItem>();

        /// <summary>
        /// Add parameter
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="value">Parameter value</param>
        /// <param name="dbType">DbType</param>
        /// <param name="direction">Direction</param>
        /// <param name="size">Size</param>
        /// <param name="precision">Precision</param>
        /// <param name="scale">Scale</param>
        public void Add(string name, object value = null, DbType? dbType = null, ParameterDirection? direction = null, int? size = null, byte? precision = null, byte? scale = null)
        {
            var item = new DataCommandParameterItem()
            {
                Name = name,
                Value = value,
                ParameterDirection = direction ?? ParameterDirection.Input,
                DbType = dbType,
                Size = size,
                Precision = precision,
                Scale = scale
            };
            Add(item);
        }

        /// <summary>
        /// Add parameter
        /// </summary>
        /// <param name="parameter">Parameter item</param>
        public void Add(DataCommandParameterItem parameter)
        {
            if (parameter == null)
            {
                return;
            }
            Items[parameter.Name] = parameter;
        }

        /// <summary>
        /// Add parameters
        /// </summary>
        /// <param name="parameters">Parameters</param>
        public void Add(IEnumerable<KeyValuePair<string, object>> parameters)
        {
            if (parameters == null)
            {
                return;
            }
            foreach (var parameter in parameters)
            {
                Add(parameter.Key, parameter.Value);
            }
        }

        /// <summary>
        /// Add parameters
        /// </summary>
        /// <param name="parameters">Parameters</param>
        public void Add(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            if (parameters == null)
            {
                return;
            }
            foreach (var parameter in parameters)
            {
                Add(parameter.Key, parameter.Value);
            }
        }

        /// <summary>
        /// Union parameters
        /// </summary>
        /// <param name="parameters">Parameters</param>
        public DataCommandParameters Union(params DataCommandParameters[] parameters)
        {
            if (parameters.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var cmdParameter in parameters)
            {
                if (cmdParameter == null || cmdParameter.Items.IsNullOrEmpty())
                {
                    continue;
                }
                foreach (var para in cmdParameter.Items)
                {
                    Add(para.Value);
                }
            }
            return this;
        }

        /// <summary>
        /// Gets the parameter value
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <returns>Return parameter value</returns>
        public object GetParameterValue(string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                return null;
            }
            Items.TryGetValue(parameterName, out var item);
            return item?.Value;
        }

        /// <summary>
        /// Rename parameter rename
        /// </summary>
        /// <param name="originalParameterName">Original name</param>
        /// <param name="newParameterName">New parameter name</param>
        public void Rename(string originalParameterName, string newParameterName)
        {
            if (string.IsNullOrWhiteSpace(originalParameterName) || string.IsNullOrWhiteSpace(newParameterName))
            {
                return;
            }
            if (Items.TryGetValue(originalParameterName, out var parameter) && parameter != null)
            {
                Items.Remove(originalParameterName);
                parameter.Name = newParameterName;
                Items.Add(newParameterName, parameter);
            }
        }

        /// <summary>
        /// Modify value
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="newValue">New value</param>
        public void ModifyValue(string parameterName, object newValue)
        {
            if (Items.TryGetValue(parameterName, out var item) && item != null)
            {
                item.Value = newValue;
            }
        }

        /// <summary>
        /// Clone a command parameters
        /// </summary>
        /// <returns></returns>
        public DataCommandParameters Clone()
        {
            return new DataCommandParameters()
            {
                Items = Items.ToDictionary(c => c.Key, c => c.Value.Clone())
            };
        }

        /// <summary>
        /// Parse an object to command parameters
        /// </summary>
        /// <param name="originalParameters">Original parameters</param>
        /// <returns>Return a command parameters</returns>
        public static DataCommandParameters Parse(object originalParameters)
        {
            if (originalParameters == null)
            {
                return null;
            }
            if (originalParameters is DataCommandParameters commandParameters)
            {
                return commandParameters;
            }
            commandParameters = new DataCommandParameters();
            if (originalParameters is IEnumerable<KeyValuePair<string, string>> stringParametersDict)
            {
                commandParameters.Add(stringParametersDict);
            }
            else if (originalParameters is IEnumerable<KeyValuePair<string, dynamic>> dynamicParametersDict)
            {
                commandParameters.Add(dynamicParametersDict);
            }
            else if (originalParameters is IEnumerable<KeyValuePair<string, object>> objectParametersDict)
            {
                commandParameters.Add(objectParametersDict);
            }
            //else if (originalParameters is IEnumerable<KeyValuePair<string, IModificationValue>> modifyParametersDict)
            //{
            //    commandParameters.Add(modifyParametersDict);
            //}
            else
            {
                objectParametersDict = originalParameters.ToObjectDictionary();
                commandParameters.Add(objectParametersDict);
            }
            return commandParameters;
        }

        /// <summary>
        /// Convert to dynamic parameters
        /// </summary>
        /// <param name="databaseServerType">Database server type</param>
        /// <returns></returns>
        public DynamicParameters ConvertToDynamicParameters(DatabaseServerType databaseServerType)
        {
            if (Items.IsNullOrEmpty())
            {
                return null;
            }
            DynamicParameters dynamicParameters = new DynamicParameters();
            foreach (var item in Items)
            {
                var parameter = SixnetDataManager.HandleParameter(databaseServerType, item.Value);
                dynamicParameters.Add(parameter.Name, parameter.Value
                                    , parameter.DbType, parameter.ParameterDirection
                                    , parameter.Size, parameter.Precision
                                    , parameter.Scale);
            }
            return dynamicParameters;
        }

        /// <summary>
        /// Clear all parameters
        /// </summary>
        public void Clear()
        {
            Items?.Clear();
        }
    }
}

/// <summary>
/// Data command parameter item
/// </summary>
[Serializable]
public class DataCommandParameterItem
{
    /// <summary>
    /// Gets or sets the parameter name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the parameter value
    /// </summary>
    public object Value { get; set; }

    /// <summary>
    /// Gets or sets the parameter direction
    /// </summary>
    public ParameterDirection ParameterDirection { get; set; }

    /// <summary>
    /// Gets or sets the parameter database type
    /// </summary>
    public DbType? DbType { get; set; }

    /// <summary>
    /// Gets or sets the parameter size
    /// </summary>
    public int? Size { get; set; }

    /// <summary>
    /// Gets or sets the parameter attached parameter
    /// </summary>
    public IDbDataParameter AttachedParam { get; set; }

    /// <summary>
    /// Gets or sets the parameter output target
    /// </summary>
    internal object OutputTarget { get; set; }

    /// <summary>
    /// Gets or sets whether the parameter is came from template
    /// </summary>
    internal bool CameFromTemplate { get; set; }

    /// <summary>
    /// Gets or sets the parameter precision
    /// </summary>
    public byte? Precision { get; set; }

    /// <summary>
    /// Gets or sets the parameter scale
    /// </summary>
    public byte? Scale { get; set; }

    /// <summary>
    /// Clone a parameter item
    /// </summary>
    /// <returns></returns>
    public DataCommandParameterItem Clone()
    {
        return new DataCommandParameterItem()
        {
            Name = Name,
            Value = Value,
            ParameterDirection = ParameterDirection,
            DbType = DbType,
            Size = Size,
            AttachedParam = AttachedParam,
            OutputTarget = OutputTarget,
            CameFromTemplate = CameFromTemplate,
            Precision = Precision,
            Scale = Scale
        };
    }
}
