using EZNEW.Development.Query;

namespace EZNEW.Development.Domain.Repository.Warehouse
{
    /// <summary>
    /// Defines warehouse count result
    /// </summary>
    public class WarehouseCountResult
    {
        public long Count { get; set; }

        /// <summary>
        /// Gets or sets the count query
        /// </summary>
        public IQuery CountQuery { get; set; }
    }
}
