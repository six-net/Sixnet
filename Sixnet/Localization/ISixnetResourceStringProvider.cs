// "Company © 2025. All rights reserved."

using System.Globalization;

namespace Sixnet.Localization
{
    public interface ISixnetResourceStringProvider
    {
        IList<string> GetAllResourceStrings(CultureInfo culture, bool throwOnMissing);
    }
}
