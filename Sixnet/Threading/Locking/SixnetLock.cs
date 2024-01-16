using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Threading.Locking
{
    /// <summary>
    /// Defines sixnet lock
    /// </summary>
    public struct SixnetLock
    {
        readonly string lockObject;
        readonly string lockName;
        readonly string lockValue;

        internal SixnetLock(string lockObject, string lockName, string lockValue)
        {
            this.lockObject = lockObject;
            this.lockName = lockName;
            this.lockValue = lockValue;
        }

        public bool Release()
        {
            return LockManager.ReleaseLock(lockObject, lockName, lockValue);
        }
    }
}
