// "Company © 2025. All rights reserved."

namespace Sixnet.Threading.Locking
{
    /// <summary>
    /// Sixnet lock options
    /// </summary>
    public class SixnetLockOptions
    {
        /// <summary>
        /// Lock object settings
        /// </summary>
        public Dictionary<string, SixnetLockObjectSetting> LockObjects { get; set; }

        /// <summary>
        /// Default expiration seconds.
        /// Default is 60s
        /// </summary>
        public int DefaultExpirationSeconds { get; set; } = 60;
    }

    public class SixnetLockObjectSetting
    {
        /// <summary>
        /// Lock object name
        /// </summary>
        public string LockObjectName {  get; set; }

        /// <summary>
        /// Whether is remote
        /// </summary>
        public bool Remote {  get; set; }

        /// <summary>
        /// Expiration seconds
        /// </summary>
        public int? ExpirationSeconds {  get; set; }
    }
}
