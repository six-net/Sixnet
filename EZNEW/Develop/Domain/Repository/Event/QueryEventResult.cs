using System.Collections.Generic;

namespace EZNEW.Develop.Domain.Repository.Event
{
    /// <summary>
    /// Query event result
    /// </summary>
    /// <typeparam name="T">Data type</typeparam>
    public class QueryEventResult<T> : IRepositoryEventExecuteResult
    {
        /// <summary>
        /// Gets or sets the datas
        /// </summary>
        public List<T> Datas
        {
            get; set;
        }
    }
}
