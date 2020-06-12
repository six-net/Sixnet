using System;

namespace EZNEW.Application
{
    /// <summary>
    /// Provides properties of application
    /// </summary>
    [Serializable]
    public class ApplicationInfo
    {
        /// <summary>
        /// Gets or sets application code
        /// </summary>
        public string Code
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets application secret
        /// </summary>
        public string Secret
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets application name
        /// </summary>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets application type
        /// </summary>
        public ApplicationType Type
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets application status
        /// </summary>
        public ApplicationStatus Status
        {
            get; set;
        }
    }
}
