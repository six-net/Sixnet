using EZNEW.Development.Query;

namespace EZNEW.Development.Domain.Repository.Warehouse
{
    /// <summary>
    /// Defines warehouse aggregate result
    /// </summary>
    public class WarehouseAggregateResult<TValue>
    {
        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public TValue Value { get; set; }

        /// <summary>
        /// Gets or sets the aggregate query
        /// </summary>
        public IQuery AggregateQuery { get; set; }

        /// <summary>
        /// Gets or sets whether valid compute value
        /// </summary>
        public bool ValidValue { get; set; }
    }
}
