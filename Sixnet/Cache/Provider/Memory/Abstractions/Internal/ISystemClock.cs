// "Company © 2025. All rights reserved."

namespace Sixnet.Cache.Provider.Memory.Abstractions.Internal
{
    /// <summary>
    /// Abstracts the system clock to facilitate testing.
    /// </summary>
    internal interface ISystemClock
    {
        /// <summary>
        /// Retrieves the current system time in UTC.
        /// </summary>
        DateTimeOffset UtcNow { get; }
    }
}
