namespace EZNEW.Application
{
    /// <summary>
    /// Provides access to and management of application information
    /// </summary>
    public static class ApplicationManager
    {
        /// <summary>
        /// Gets or sets the information about the currently running application
        /// </summary>
        public static ApplicationInfo Current
        {
            get; set;
        }
    }
}
