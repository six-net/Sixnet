namespace EZNEW.Cache.Server
{
    /// <summary>
    /// Data change save options
    /// </summary>
    public class DataChangeSaveOptions
    {
        /// <summary>
        /// Gets or sets the time（seconds）
        /// </summary>
        public long Seconds { get; set; }

        /// <summary>
        /// Gets or sets the changes number
        /// </summary>
        public long Changes { get; set; }
    }
}
