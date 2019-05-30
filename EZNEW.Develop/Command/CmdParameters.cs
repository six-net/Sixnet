using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Command
{
    /// <summary>
    /// Command Parameters
    /// </summary>
    public class CmdParameters
    {
        /// <summary>
        /// parameters
        /// </summary>
        public Dictionary<string, ParameterItem> Parameters { get; } = new Dictionary<string, ParameterItem>();

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
            Parameters[name] = new ParameterItem()
            {
                Name = name,
                Value = value,
                ParameterDirection = direction ?? ParameterDirection.Input,
                DbType = dbType,
                Size = size,
                Precision = precision,
                Scale = scale
            };
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
