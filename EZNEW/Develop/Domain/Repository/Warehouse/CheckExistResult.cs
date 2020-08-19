using EZNEW.Develop.CQuery;

namespace EZNEW.Develop.Domain.Repository.Warehouse
{
    /// <summary>
    /// Check exist result
    /// </summary>
    public class CheckExistResult
    {
        /// <summary>
        /// Gets or sets whether has data
        /// </summary>
        public bool IsExist { get; set; }

        /// <summary>
        /// Gets or sets the check query
        /// </summary>
        public IQuery CheckQuery { get; set; }
    }
}
