// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Development.Data;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Default queryable six
    /// </summary>
    internal partial class SixnetDefaultQueryableSix<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>
    {
        #region Select

        /// <summary>
        /// Select fields as a temp table
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public Task<ISixnetQueryable<TResult>> SelectAsTempTableAsync<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TResult>> fields)
        {
            return IncludeExpressionFieldsAsTempTableCoreAsync<TResult>(fields);
        }

        #endregion

        #region Data access

        #region List

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        public async Task<List<TReturn>> ToListAsync<TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null)
        {
            return await ToListAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(dataMappingFunc, configure).ConfigureAwait(false);
        }

        #endregion

        #endregion
    }
}
