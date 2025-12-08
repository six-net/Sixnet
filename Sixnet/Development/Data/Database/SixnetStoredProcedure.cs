// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Procedu
    /// </summary>
    public class SixnetStoredProcedure
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Is system database
        /// </summary>
        public bool IsSystem { get; set; }
    }
}
