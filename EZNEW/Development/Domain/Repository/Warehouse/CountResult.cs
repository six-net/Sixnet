using EZNEW.Development.Query;

namespace EZNEW.Development.Domain.Repository.Warehouse
{
    /// <summary>
    /// Count result
    /// </summary>
    public class CountResult
    {
        public long Count { get; set; }


        /// <summary>
        /// Gets or sets the compute query
        /// </summary>
        public IQuery ComputeQuery { get; set; }
    }
}
