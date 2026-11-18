using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Model
{
    /// <summary>
    /// Save data parameter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SixnetSaveDataParameter<T>
    {
        /// <summary>
        /// Gets or sets the data
        /// </summary>
        public T Data { get; set; }
    }
}
