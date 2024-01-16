using System;

namespace Sixnet.App
{
    /// <summary>
    /// Defines application info
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
        public ApplicationType Type { get; set; }

        /// <summary>
        /// Gets or sets the application version
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the application status
        /// </summary>
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Running;
    }
}
