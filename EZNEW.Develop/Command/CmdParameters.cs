using EZNEW.Develop.Command.Modify;
using EZNEW.Framework.Extension;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Command
{
    /// <summary>
    /// command parameters
    /// </summary>
    public class CmdParameters
    {
        /// <summary>
        /// parameters
        /// </summary>
        public Dictionary<string, ParameterItem> Parameters { get; private set; } = new Dictionary<string, ParameterItem>();

        /// <summary>
        /// add parameter
        /// </summary>
        /// <param name="name">parameter name</param>
        /// <param name="value">parameter value</param>
        /// <param name="dbType">dbtype</param>
        /// <param name="direction">direction</param>
        /// <param name="size">size</param>
        /// <param name="precision">precision</param>
        /// <param name="scale">scale</param>
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
        /// add parameter
        /// </summary>
        /// <param name="parameter"></param>
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
        /// add parameters
        /// </summary>
        /// <param name="parameters">parameters</param>
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
        /// add parameters
        /// </summary>
        /// <param name="parameters">parameters</param>
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
        /// add parameters
        /// </summary>
        /// <param name="parameters">parameters</param>
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
        /// union parameters
        /// </summary>
        /// <param name="parameters"></param>
        public CmdParameters Union(params CmdParameters[] parameters)
        {
            if (parameters.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var cmdParameter in parameters)
            {
                if (cmdParameter.Parameters.IsNullOrEmpty())
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
        /// get parameter value
        /// </summary>
        /// <param name="parameterName">parameter name</param>
        /// <returns></returns>
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
        /// parameter rename
        /// </summary>
        /// <param name="originParameterName">origin name</param>
        /// <param name="newParameterName">new name</param>
        public void Rename(string originParameterName, string newParameterName)
        {
            if (string.IsNullOrWhiteSpace(originParameterName) || string.IsNullOrWhiteSpace(newParameterName))
            {
                return;
            }
            if (Parameters.TryGetValue(originParameterName, out var parameter) && parameter != null)
            {
                Parameters.Remove(originParameterName);
                parameter.Name = newParameterName;
                Parameters.Add(newParameterName, parameter);
            }
        }

        /// <summary>
        /// modify value
        /// </summary>
        /// <param name="parameterName">parameter name</param>
        /// <param name="newValue">new value</param>
        public void ModifyValue(string parameterName, object newValue)
        {
            if (Parameters.TryGetValue(parameterName, out var item) && item != null)
            {
                item.Value = newValue;
            }
        }
    }

    /// <summary>
    /// parameter item
    /// </summary>
    public class ParameterItem
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public ParameterDirection ParameterDirection { get; set; }
        public DbType? DbType { get; set; }
        public int? Size { get; set; }
        public IDbDataParameter AttachedParam { get; set; }
        internal object OutputTarget { get; set; }
        internal bool CameFromTemplate { get; set; }

        public byte? Precision { get; set; }
        public byte? Scale { get; set; }
    }
}
