using System.Collections.Generic;
using Newtonsoft.Json;

namespace EZNEW.Paging
{
    /// <summary>
    /// Paging contract
    /// </summary>
    [JsonObject]
    public interface IPaging<T> : IEnumerable<T>
    {
        #region Properties

        /// <summary>
        /// Gets the current page
        /// </summary>
        long Page { get; }

        /// <summary>
        /// Gets the page size
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// Gets the page count
        /// </summary>
        long PageCount { get; }

        /// <summary>
        /// Gets the total count
        /// </summary>
        long TotalCount { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Converts paging data to a paging object of the specified data type
        /// </summary>
        /// <typeparam name="TTarget">Target object type</typeparam>
        /// <returns>Return the target paging object</returns>
        IPaging<TTarget> ConvertTo<TTarget>();

        #endregion
    }
}
