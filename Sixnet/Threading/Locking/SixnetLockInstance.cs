// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

namespace Sixnet.Threading.Locking
{
    /// <summary>
    /// Defines lock instance
    /// </summary>
    public struct SixnetLockInstance
    {
        readonly string lockObject;
        readonly string lockName;
        readonly string lockValue;

        internal SixnetLockInstance(string lockObject, string lockName, string lockValue)
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

        /// <summary>
        /// Release lock
        /// </summary>
        /// <returns></returns>
        public readonly Task<bool> ReleaseAsync()
        {
            return SixnetLocker.ReleaseLockAsync(lockObject, lockName, lockValue);
        }
    }
}
