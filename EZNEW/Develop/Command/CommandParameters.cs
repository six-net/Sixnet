using System;
using System.Collections.Generic;
using System.Data;
using EZNEW.Develop.Command.Modify;

namespace EZNEW.Develop.Command
{
    /// <summary>
    /// Command parameters
    /// </summary>
    [Serializable]
    public class CommandParameters
    {
        /// <summary>
        /// Parameters
        /// </summary>
        public Dictionary<string, ParameterItem> Parameters { get; private set; } = new Dictionary<string, ParameterItem>();

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
            var item = new ParameterItem()
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
        public void Add(ParameterItem parameter)
        {
            if (parameter == null)
            {
                return;
            }
            if (Parameters.ContainsKey(parameter.Name))
            {
                Parameters[parameter.Name] = parameter;
            }
            else
            {
                Parameters.Add(parameter.Name, parameter);
            }
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
        /// Add parameters
        /// </summary>
        /// <param name="parameters">Parameters</param>
        public void Add(IEnumerable<KeyValuePair<string, IModifyValue>> parameters)
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
        public CommandParameters Union(params CommandParameters[] parameters)
        {
            if (parameters.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var cmdParameter in parameters)
            {
                if (cmdParameter == null || cmdParameter.Parameters.IsNullOrEmpty())
                {
                    continue;
                }
                foreach (var para in cmdParameter.Parameters)
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
            Parameters.TryGetValue(parameterName, out var item);
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
            if (Parameters.TryGetValue(originalParameterName, out var parameter) && parameter != null)
            {
                Parameters.Remove(originalParameterName);
                parameter.Name = newParameterName;
                Parameters.Add(newParameterName, parameter);
            }
        }

        /// <summary>
        /// Modify value
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="newValue">New value</param>
        public void ModifyValue(string parameterName, object newValue)
        {
            if (Parameters.TryGetValue(parameterName, out var item) && item != null)
            {
                item.Value = newValue;
            }
        }
    }

    /// <summary>
    /// Parameter item
    /// </summary>
    [Serializable]
    public class ParameterItem
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
    }
}
