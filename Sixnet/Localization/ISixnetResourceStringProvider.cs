using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Sixnet.Localization
{
    public interface ISixnetResourceStringProvider
    {
        IList<string> GetAllResourceStrings(CultureInfo culture, bool throwOnMissing);
    }
}
