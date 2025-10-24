namespace Sixnet.App
{
    /// <summary>
    /// Module contract
    /// </summary>
    public interface ISixnetModule
    {
        /// <summary>
        /// Configure module
        /// </summary>
        void Configure();

        /// <summary>
        /// Init module
        /// </summary>
        void Init();
    }
}
