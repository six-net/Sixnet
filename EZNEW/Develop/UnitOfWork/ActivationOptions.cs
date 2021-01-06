namespace EZNEW.Develop.UnitOfWork
{
    /// <summary>
    /// Activation options
    /// </summary>
    public class ActivationOptions
    {
        /// <summary>
        /// Default activation options
        /// </summary>
        public static readonly ActivationOptions Default = new ActivationOptions();

        public static readonly ActivationOptions ForceExecuteActivation = new ActivationOptions()
        {
            ForceExecute = true
        };

        /// <summary>
        /// Gets or sets whether force execute the activation record
        /// </summary>
        public bool ForceExecute { get; set; }
    }
}
