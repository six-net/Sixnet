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
        /// 分布式锁名称
        /// </summary>
        public List<string> DistributeLockObjectNames { get; set; }
    }
}
