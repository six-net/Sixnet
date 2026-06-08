// "Company © 2025. All rights reserved."

namespace Sixnet.Model
{
    /// <summary>
    /// Defines key-value collection
    /// </summary>
    public class SixnetKeyValueCollection<TKey, TValue> : List<KeyValuePair<TKey, TValue>>
    {
        public SixnetKeyValueCollection() : base() { }

        public SixnetKeyValueCollection(IEnumerable<KeyValuePair<TKey, TValue>> collection) : base(collection) { }

        public SixnetKeyValueCollection(int capacity) : base(capacity) { }

        /// <summary>
        /// Gets an empty collection
        /// </summary>
        /// <returns></returns>
        public static SixnetKeyValueCollection<TKey, TValue> Empty()
        {
            return new SixnetKeyValueCollection<TKey, TValue>(0);
        }
    }
}
