using EZNEW.Development.Query;

namespace EZNEW.Development.Domain.Repository.Warehouse
{
    /// <summary>
    /// Warehouse exists result
    /// </summary>
    public class WarehouseExistsResult
    {
        /// <summary>
        /// Indicates whether has data
        /// </summary>
        public bool IsExists { get; set; }

        /// <summary>
        /// Gets or sets the check query
        /// </summary>
        public IQuery CheckQuery { get; set; }
    }
}
