using System;
using System.Collections.Generic;
using System.Text;

namespace MicBeach.Develop.Config.Domain
{
    /// <summary>
    /// repository config
    /// </summary>
    public class RepositoryConfig
    {
        /// <summary>
        /// enable model warehouse(default is true)
        /// </summary>
        public bool EnableModelWarehouse
        {
            get; set;
        } = true;
    }
}
