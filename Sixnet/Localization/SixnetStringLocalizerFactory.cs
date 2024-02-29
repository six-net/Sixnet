using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Localization;
using Sixnet.Exceptions;

namespace Sixnet.Localization
{
    public class SixnetStringLocalizerFactory : ISixnetStringLocalizerFactory
    {
        readonly IStringLocalizerFactory _factory;

        public SixnetStringLocalizerFactory(IStringLocalizerFactory factory)
        {
            SixnetDirectThrower.ThrowArgNullIf(factory == null, nameof(factory));
            _factory = factory;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return _factory.Create(resourceSource);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return _factory.Create(baseName, location);
        }

        public ISixnetStringLocalizer CreateLocalizer(Type resourceSource)
        {
            if (_factory is ISixnetStringLocalizerFactory sixnetStringLocalizerFactory)
            {
                return sixnetStringLocalizerFactory.CreateLocalizer(resourceSource);
            }
            return null;
        }

        public ISixnetStringLocalizer CreateLocalizer(string baseName, string location)
        {
            if (_factory is ISixnetStringLocalizerFactory sixnetStringLocalizerFactory)
            {
                return sixnetStringLocalizerFactory.CreateLocalizer(baseName, location);
            }
            return null;
        }
    }
}
