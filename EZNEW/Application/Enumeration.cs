using System;

namespace EZNEW.Application
{
    /// <summary>
    /// Defines application type
    /// </summary>
    [Serializable]
    public enum ApplicationType
    {
        Unknown = 0,
        WebSite = 110,
        WebAPI = 120,
        WindowsService = 130,
        Console = 140,
        WindowsForm = 150,
        ApplicationService = 160
    }

    /// <summary>
    /// Defines application status
    /// </summary>
    [Serializable]
    public enum ApplicationStatus
    {
        Ready = 200,
        Starting = 205,
        Running = 210,
        Paused = 215,
        Stoped = 220,
        Closed = 225
    }

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
