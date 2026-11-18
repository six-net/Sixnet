// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sixnet.Development.Entity;

namespace Sixnet.Development.Domain.Model
{
    public class SixnetSaveDataInfo<T>
    {
        /// <summary>
        /// Gets or sets the data
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Whether allow editing archived data
        /// </summary>
        public bool IncludeArchived {  get; set; }
    }
}
