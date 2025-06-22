using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Threading.Locking
{
    /// <summary>
    /// Defines lock instance
    /// </summary>
    public struct LockInstance
    {
        readonly string lockObject;
        readonly string lockName;
        readonly string lockValue;

        internal LockInstance(string lockObject, string lockName, string lockValue)
        {
            this.lockObject = lockObject;
            this.lockName = lockName;
            this.lockValue = lockValue;
        }

        /// <summary>
        /// Release lock
        /// </summary>
        /// <returns></returns>
        public readonly bool Release()
        {
            return SixnetLocker.ReleaseLock(lockObject, lockName, lockValue);
        }
    }
}
