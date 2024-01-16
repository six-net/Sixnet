using System.Collections.Generic;

namespace Sixnet.Model
{
    /// <summary>
    /// Defines key-value collection
    /// </summary>
    public class KeyValueCollection<TKey, TValue> : List<KeyValuePair<TKey, TValue>>
    {
        public KeyValueCollection() : base() { }

        public KeyValueCollection(IEnumerable<KeyValuePair<TKey, TValue>> collection) : base(collection) { }

        public KeyValueCollection(int capacity) : base(capacity) { }

        /// <summary>
        /// Gets an empty collection
        /// </summary>
        /// <returns></returns>
        public static KeyValueCollection<TKey, TValue> Empty()
        {
            return new KeyValueCollection<TKey, TValue>(0);
        }
    }
}
