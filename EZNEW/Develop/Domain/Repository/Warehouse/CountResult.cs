namespace EZNEW.Develop.Domain.Repository.Warehouse
{
    /// <summary>
    /// Count result
    /// </summary>
    public class CountResult
    {
        /// <summary>
        /// Gets or sets the new data count
        /// </summary>
        public long NewDataCount
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the persistent data remove count
        /// </summary>
        public long PersistentDataRemoveCount
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the persistent data count
        /// </summary>
        public long PersistentDataCount
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the total data count
        /// </summary>
        public long TotalDataCount
        {
            get; set;
        }
    }
}
