using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Development.Data.Field
{
    public class SixnetFieldComparer : IEqualityComparer<ISixnetField>
    {
        public static SixnetFieldComparer DefaultComparer => new();

        public bool Equals(ISixnetField x, ISixnetField y)
        {
            return x != null && y != null &&
                (x == y || x.Equals(y));
        }

        public int GetHashCode(ISixnetField obj)
        {
            return 0;
        }
    }
}
