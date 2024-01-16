using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Development.Data;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Default queryable five
    /// </summary>
    internal partial class DefaultQueryableFive<TFirst, TSecond, TThird, TFourth, TFifth>
    {
        #region Data access

        #region List

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        public async Task<List<TReturn>> ToListAsync<TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return await ToListAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(dataMappingFunc, configure).ConfigureAwait(false);
        }

        #endregion

        #endregion
    }
}
