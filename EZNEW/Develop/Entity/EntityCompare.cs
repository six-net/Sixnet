using System.Collections.Generic;

namespace EZNEW.Develop.Entity
{
    /// <summary>
    /// Entity compare
    /// </summary>
    public class EntityCompare<T> : IEqualityComparer<T>
    {
        public bool Equals(T x, T y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(T obj)
        {
            return 0;
        }
    }
}
