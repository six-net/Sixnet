// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Development.Data.Database
{
    public struct DatabaseConnectionMeta
    {
        /// <summary>
        /// Gets or sets the user name
        /// </summary>
        public string UserName {  get; set; }

        /// <summary>
        /// Gets or sets the password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the data souce
        /// </summary>
        public string DataSource {  get; set; }

        /// <summary>
        /// Gets or sets the database name
        /// </summary>
        public string DatabaseName {  get; set; }
    }
}
