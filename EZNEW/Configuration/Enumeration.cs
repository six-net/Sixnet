using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Configuration
{
    /// <summary>
    /// Defines file match pattern
    /// </summary>
    [Serializable]
    public enum FileMatchPattern
    {
        /// <summary>
        /// Match all files
        /// </summary>
        None = 0,
        /// <summary>
        /// Match files by convention
        /// </summary>
        Convention = 2,
        /// <summary>
        /// Match files with the file name prefix
        /// </summary>
        FileNamePrefix = 4,
        /// <summary>
        /// Match files with the file name suffix
        /// </summary>
        FileNameSuffix = 8,
        /// <summary>
        /// Match files by file name words
        /// </summary>
        IncludeFileName = 16,
        /// <summary>
        /// Match files without file name words
        /// </summary>
        ExcludeFileName = 32,
        /// <summary>
        /// Match files by regex
        /// </summary>
        ExcludeByRegex = 64,
        /// <summary>
        /// Match files without regex
        /// </summary>
        IncludeByRegex = 128,
    }
}
