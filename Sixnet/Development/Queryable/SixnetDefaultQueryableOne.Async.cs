// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sixnet.Development.Data;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Default queryable two
    /// </summary>
    internal partial class SixnetDefaultQueryableOne<TFirst>
    {
        #region Select

        /// <summary>
        /// Select fields as a temp table
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public Task<ISixnetQueryable<TResult>> SelectAsTempTableAsync<TResult>(Expression<Func<TFirst, TResult>> fields)
        {
            return IncludeExpressionFieldsAsTempTableCoreAsync<TResult>(fields);
        }

        protected async Task<ISixnetQueryable<TResult>> IncludeExpressionFieldsAsTempTableCoreAsync<TResult>(Expression fields)
        {
            IncludeExpressionFieldsCore(fields);
            return await AsTempTableAsync<TResult>().ConfigureAwait(false);
        }

        #endregion
    }
}
