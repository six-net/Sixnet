using System;
using System.Globalization;
using Microsoft.Extensions.Logging;

namespace Sixnet.Localization
{
    internal static class SixnetResourceStringLocalizerLoggerExtensions
    {
        private static readonly Action<ILogger, string, string, CultureInfo, Exception> _searchedLocation;

        static SixnetResourceStringLocalizerLoggerExtensions()
        {
            _searchedLocation = LoggerMessage.Define<string, string, CultureInfo>(
                LogLevel.Debug,
                new EventId(1, "SearchedLocation"),
                $"{nameof(SixnetResourceStringLocalizer)} searched for '{{Key}}' in '{{LocationSearched}}' with culture '{{Culture}}'.");
        }

        public static void SearchedLocation(this ILogger logger, string key, string searchedLocation, CultureInfo culture)
        {
            _searchedLocation(logger, key, searchedLocation, culture, null);
        }
    }
}
