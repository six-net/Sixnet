using System;

namespace Sixnet.App
{
    /// <summary>
    /// Defines application type
    /// </summary>
    [Serializable]
    public enum ApplicationType
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Web site
        /// </summary>
        WebSite = 110,
        /// <summary>
        /// Web api
        /// </summary>
        WebApi = 120,
        /// <summary>
        /// Windows service
        /// </summary>
        WindowsService = 130,
        /// <summary>
        /// Console
        /// </summary>
        Console = 140,
        /// <summary>
        /// Windows form
        /// </summary>
        WindowsForm = 150,
        /// <summary>
        /// Application service
        /// </summary>
        ApplicationService = 160
    }

    /// <summary>
    /// Defines application status
    /// </summary>
    [Serializable]
    public enum ApplicationStatus
    {
        /// <summary>
        /// Ready
        /// </summary>
        Ready = 200,
        /// <summary>
        /// Stating
        /// </summary>
        Starting = 205,
        /// <summary>
        /// Running
        /// </summary>
        Running = 210,
        /// <summary>
        /// Paused
        /// </summary>
        Paused = 215,
        /// <summary>
        /// Stoped
        /// </summary>
        Stoped = 220,
        /// <summary>
        /// Closed
        /// </summary>
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
