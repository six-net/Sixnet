using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Configuration
{
    /// <summary>
    /// Defines default configure file match pattern
    /// </summary>
    [Serializable]
    public enum DefaultConfigureFileMatchPattern
    {
        /// <summary>
        /// Match all files
        /// </summary>
        None = 0,
        /// <summary>
        /// Match files without default exclude files
        /// </summary>
        Default = 2,
        /// <summary>
        /// Match files by convention
        /// </summary>
        Convention = 4,
        /// <summary>
        /// Match files by file name words
        /// </summary>
        IncludeFileName = 8,
        /// <summary>
        /// Mathch files without file name words
        /// </summary>
        ExcludeFileName = 16
    }
}
