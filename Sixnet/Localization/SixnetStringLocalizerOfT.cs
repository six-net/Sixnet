using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Extensions.Localization;
using Sixnet.Exceptions;

namespace Sixnet.Localization
{
    public class SixnetStringLocalizer<TResourceSource> : ISixnetStringLocalizer<TResourceSource>
    {
        private readonly ISixnetStringLocalizer _localizer;

        public LocalizedString this[string cultureName, string name, params object[] arguments] => _localizer[cultureName, name, arguments];

        public LocalizedString this[CultureInfo culture, string name, params object[] arguments] => _localizer[culture, name, arguments];

        public LocalizedString this[string cultureName, string name] => _localizer[cultureName, name];

        public LocalizedString this[CultureInfo culture, string name] => _localizer[culture, name];

        public virtual LocalizedString this[string name] => _localizer[name];

        public virtual LocalizedString this[string name, params object[] arguments] => _localizer[name, arguments];

        public SixnetStringLocalizer(ISixnetStringLocalizerFactory factory)
        {
            SixnetDirectThrower.ThrowArgErrorIf(factory == null, nameof(factory));
            _localizer = factory.CreateLocalizer(typeof(TResourceSource));
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return _localizer.GetAllStrings(includeParentCultures);
        }
    }
}
