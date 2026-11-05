// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sixnet.Development.Entity;

namespace Sixnet.Development.Domain.Parameter
{
    public class SaveDataParameter<T>
    {
        /// <summary>
        /// Gets or sets the new data
        /// </summary>
        public T NewData { get; set; }
    }
}
