// "Company © 2025. All rights reserved."

using System.Dynamic;
using System.Threading.Tasks;

using Sixnet.Model;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Sixnet querier
    /// </summary>
    public static class SixnetQuerier
    {
        #region Create 

        /// <summary>
        /// Create a new queryable instance
        /// </summary>
        /// <returns></returns>
        public static ISixnetQueryable Create()
        {
            return Create(null);
        }

        /// <summary>
        /// Create a new queryable instance
        /// </summary>
        /// <param name="sourceQueryable">Source queryable</param>
        /// <returns></returns>
        public static ISixnetQueryable Create(ISixnetQueryable sourceQueryable = null)
        {
            return new SixnetDefaultQueryable(sourceQueryable).SetModelType(typeof(ExpandoObject));
        }

        /// <summary>
        /// Create a new queryable instance
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <returns></returns>
        public static ISixnetQueryable<TFirst> Create<TFirst>()
        {
            ISixnetQueryable sourceQueryable = null;
            return Create<TFirst>(sourceQueryable);
        }

        /// <summary>
        /// Create a new queryable instance
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <param name="sourceQueryable">Source queryable</param>
        /// <returns></returns>
        public static ISixnetQueryable<TFirst> Create<TFirst>(ISixnetQueryable sourceQueryable = null)
        {
            return new SixnetDefaultQueryableOne<TFirst>(sourceQueryable);
        }

        /// <summary>
        /// Create a new queryable instance
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <param name="sourceQueryable">Source queryable</param>
        /// <returns></returns>
        public static ISixnetQueryable<TFirst, TSecond> Create<TFirst, TSecond>(ISixnetQueryable sourceQueryable = null)
        {
            return new SixnetDefaultQueryableTwo<TFirst, TSecond>(sourceQueryable);
        }

        /// <summary>
        /// Create a new queryable instance
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <typeparam name="TThird"></typeparam>
        /// <param name="sourceQueryable">Source queryable</param>
        /// <returns></returns>
        public static ISixnetQueryable<TFirst, TSecond, TThird> Create<TFirst, TSecond, TThird>(ISixnetQueryable sourceQueryable = null)
        {
            return new SixnetDefaultQueryableThree<TFirst, TSecond, TThird>(sourceQueryable);
        }

        /// <summary>
        /// Create a new queryable instance
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <typeparam name="TThird"></typeparam>
        /// <typeparam name="TFourth"></typeparam>
        /// <param name="sourceQueryable">Source queryable</param>
        /// <returns></returns>
        public static ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Create<TFirst, TSecond, TThird, TFourth>(ISixnetQueryable sourceQueryable = null)
        {
            return new SixnetDefaultQueryableFour<TFirst, TSecond, TThird, TFourth>(sourceQueryable);
        }

        /// <summary>
        /// Create a new queryable instance
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <typeparam name="TThird"></typeparam>
        /// <typeparam name="TFourth"></typeparam>
        /// <typeparam name="TFifth"></typeparam>
        /// <param name="sourceQueryable">Source queryable</param>
        /// <returns></returns>
        public static ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Create<TFirst, TSecond, TThird, TFourth, TFifth>(ISixnetQueryable sourceQueryable = null)
        {
            return new SixnetDefaultQueryableFive<TFirst, TSecond, TThird, TFourth, TFifth>(sourceQueryable);
        }

        /// <summary>
        /// Create a new queryable instance
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <typeparam name="TThird"></typeparam>
        /// <typeparam name="TFourth"></typeparam>
        /// <typeparam name="TFifth"></typeparam>
        /// <typeparam name="TSixth"></typeparam>
        /// <param name="sourceQueryable">Source queryable</param>
        /// <returns></returns>
        public static ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Create<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(ISixnetQueryable sourceQueryable = null)
        {
            return new SixnetDefaultQueryableSix<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(sourceQueryable);
        }

        /// <summary>
        /// Create a new queryable instance
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <typeparam name="TThird"></typeparam>
        /// <typeparam name="TFourth"></typeparam>
        /// <typeparam name="TFifth"></typeparam>
        /// <typeparam name="TSixth"></typeparam>
        /// <typeparam name="TSeventh"></typeparam>
        /// <param name="sourceQueryable">Source queryable</param>
        /// <returns></returns>
        public static ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Create<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(ISixnetQueryable sourceQueryable = null)
        {
            return new SixnetDefaultQueryableSeven<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(sourceQueryable);
        }

        /// <summary>
        /// Create a new queryable instance
        /// </summary>
        /// <typeparam name="T">Query model</typeparam>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return query object</returns>
        public static ISixnetQueryable<T> Create<T>(Expression<Func<T, bool>> conditionExpression)
        {
            var query = Create<T>();
            if (conditionExpression != null)
            {
                query.Where(conditionExpression);
            }
            return query;
        }

        /// <summary>
        /// Create a new queryable instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static ISixnetQueryable<SixnetValue<T>> FromDatas<T>(IEnumerable<T> datas)
        {
            var queryable = Create<SixnetValue<T>>();
            queryable.Info.From(datas);
            return queryable.AsTempTable();
        }

        /// <summary>
        /// Create a new queryable instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static Task<ISixnetQueryable<SixnetValue<T>>> FromDatasAsync<T>(IEnumerable<T> datas)
        {
            var queryable = Create<SixnetValue<T>>();
            queryable.Info.From(datas);
            return queryable.AsTempTableAsync();
        }

        #endregion
    }
}
