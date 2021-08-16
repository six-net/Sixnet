using EZNEW.Development.Query;

namespace EZNEW.Development.Domain.Repository.Warehouse
{
    /// <summary>
    /// Compute result
    /// </summary>
    public class ComputeResult<TValue>
    {
        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public TValue Value { get; set; }

        /// <summary>
        /// Gets or sets the compute query
        /// </summary>
        public IQuery ComputeQuery { get; set; }

        /// <summary>
        /// Gets or sets whether valid compute value
        /// </summary>
        public bool ValidValue { get; set; }
    }
}
