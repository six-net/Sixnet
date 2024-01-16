using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Sixnet.Exceptions;

namespace Sixnet.Localization
{
    /// <summary>
    /// Defines resource manager
    /// </summary>
    public interface ISixnetResourceManager
    {
        /// <summary>
        /// Gets or sets base name
        /// </summary>
        string BaseName { get; }

        /// <summary>
        /// Get resource set
        /// </summary>
        /// <param name="culture">Culture</param>
        /// <param name="tryParents">Whether try parents</param>
        /// <returns></returns>
        ConcurrentDictionary<string, string> GetResourceSet(CultureInfo culture, bool tryParents);

        /// <summary>
        /// Get string
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns></returns>
        string GetString(string name);

        /// <summary>
        /// Get string
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="culture">Culture</param>
        /// <returns></returns>
        string GetString(string name, CultureInfo culture);

        /// <summary>
        /// Get resource name
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        string GetResourceName(CultureInfo culture);
    }
}
