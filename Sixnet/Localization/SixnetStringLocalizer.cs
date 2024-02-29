using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Localization;
using Sixnet.Exceptions;

namespace Sixnet.Localization
{
    /// <summary>
    /// Sixnet string localizer
    /// </summary>
    public class SixnetStringLocalizer : ISixnetStringLocalizer
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
            SixnetDirectThrower.ThrowArgNullIf(factory == null, nameof(factory));
            var assemblyName = new AssemblyName(typeof(SixnetStringLocalizer).GetTypeInfo().Assembly.FullName);
            _localizer = factory.CreateLocalizer(string.Empty, assemblyName.FullName);
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return _localizer.GetAllStrings(includeParentCultures);
        }
    }
}
