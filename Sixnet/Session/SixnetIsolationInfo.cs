// "Company © 2025. All rights reserved."

namespace Sixnet.Session
{
    /// <summary>
    /// Defines isolation info
    /// </summary>
    public class SixnetIsolationInfo
    {
        /// <summary>
        /// Gets or sets tenant id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the tenant code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the tenant name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get user id
        /// </summary>
        /// <typeparam name="TId">Id data type</typeparam>
        /// <returns></returns>
        public TId GetId<TId>()
        {
            return GetIdValue<TId>(Id);
        }

        static TId GetIdValue<TId>(string originalId)
        {
            if (string.IsNullOrWhiteSpace(originalId))
            {
                return default;
            }
            object idValue;
            if (typeof(TId) == typeof(Guid))
            {
                idValue = Guid.Parse(originalId);
            }
            else
            {
                idValue = originalId.ConvertTo<TId>();
            }
            return (TId)idValue;
        }
    }
}
