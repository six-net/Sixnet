using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Threading.Locking
{
    /// <summary>
    /// Sixnet lock options
    /// </summary>
    public class SixnetLockOptions
    {
        /// <summary>
        /// Lock names
        /// </summary>
        public List<string> DistributeLockObjectNames { get; set; }
    }
}
