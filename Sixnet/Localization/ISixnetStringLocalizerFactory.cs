using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Localization;

namespace Sixnet.Localization
{
#pragma warning disable CS3027
    public interface ISixnetStringLocalizerFactory : IStringLocalizerFactory
#pragma warning restore CS3027
    {
        ISixnetStringLocalizer CreateLocalizer(Type resourceSource);

        ISixnetStringLocalizer CreateLocalizer(string baseName, string location);
    }
}
