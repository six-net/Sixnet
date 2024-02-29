using System;

namespace Sixnet.App
{
    /// <summary>
    /// Application info
    /// </summary>
    [Serializable]
    public class ApplicationInfo
    {
        /// <summary>
        /// Gets or sets the application code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the application secret
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// Gets or sets the application name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the application title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the application type
        /// </summary>
        public SixnetApplicationType Type { get; set; }

        /// <summary>
        /// Gets or sets the application version
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the application status
        /// </summary>
        public SixnetApplicationStatus Status { get; set; } = SixnetApplicationStatus.Running;
    }
}
