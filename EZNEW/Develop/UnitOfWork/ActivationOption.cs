namespace EZNEW.Develop.UnitOfWork
{
    /// <summary>
    /// Activation option
    /// </summary>
    public class ActivationOption
    {
        /// <summary>
        /// Default activation option
        /// </summary>
        public static readonly ActivationOption Default = new ActivationOption();

        /// <summary>
        /// Gets or sets whether force execute the activation record
        /// </summary>
        public bool ForceExecute
        {
            get; set;
        }
    }
}
