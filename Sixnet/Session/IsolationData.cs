namespace Sixnet.Session
{
    /// <summary>
    /// Defines tenant info
    /// </summary>
    public class IsolationData
    {
        /// <summary>
        /// Gets or sets tenant id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the tenant code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the tenant name
        /// </summary>
        public string Name { get; set; }
    }
}
