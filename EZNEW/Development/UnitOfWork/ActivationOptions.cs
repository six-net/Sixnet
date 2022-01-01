namespace EZNEW.Development.UnitOfWork
{
    /// <summary>
    /// Activation options
    /// </summary>
    public class ActivationOptions
    {
        public ActivationOptions(bool forceExecution = false)
        {
            ForceExecution = forceExecution;
        }

        /// <summary>
        /// Default activation options
        /// </summary>
        public static ActivationOptions Default => new ActivationOptions();

        /// <summary>
        /// Indicates whether force execution the activation record
        /// </summary>
        public bool ForceExecution { get; set; }
    }
}
