using EZNEW.Develop.CQuery;

namespace EZNEW.Develop.Domain.Repository.Warehouse
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
