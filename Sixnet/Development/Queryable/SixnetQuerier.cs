using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Entity;
using Sixnet.Exceptions;
using Sixnet.Expressions.Linq;
using Sixnet.Session;

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
        /// <param name="sourceQueryable">Source queryable</param>
        /// <returns></returns>
        public static ISixnetQueryable Create(ISixnetQueryable sourceQueryable = null)
        {
            return new DefaultQueryable(sourceQueryable).SetModelType(typeof(ExpandoObject));
        }

        /// <summary>
        /// Create a new queryable instance
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <param name="sourceQueryable">Source queryable</param>
        /// <returns></returns>
        public static ISixnetQueryable<TFirst> Create<TFirst>(ISixnetQueryable sourceQueryable = null)
        {
            return new DefaultQueryableOne<TFirst>(sourceQueryable);
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
            return new DefaultQueryableTwo<TFirst, TSecond>(sourceQueryable);
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
            return new DefaultQueryableThree<TFirst, TSecond, TThird>(sourceQueryable);
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
            return new DefaultQueryableFour<TFirst, TSecond, TThird, TFourth>(sourceQueryable);
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
            return new DefaultQueryableFive<TFirst, TSecond, TThird, TFourth, TFifth>(sourceQueryable);
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
            return new DefaultQueryableSix<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(sourceQueryable);
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
            return new DefaultQueryableSeven<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(sourceQueryable);
        }

        /// <summary>
        /// Create a new query instance
        /// </summary>
        /// <typeparam name="T">Query model</typeparam>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return query object</returns>
        public static ISixnetQueryable Create<T>(Expression<Func<T, bool>> conditionExpression)
        {
            var query = Create<T>();
            if (conditionExpression != null)
            {
                query.Where(conditionExpression);
            }
            return query;
        }

        #endregion

        #region Func

        /// <summary>
        /// Max func
        /// </summary>
        /// <param name="value">Original value</param>
        /// <returns></returns>
        public static T Max<T>(T value)
        {
            return value;
        }

        /// <summary>
        /// Min func
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Min<T>(T value)
        {
            return value;
        }

        /// <summary>
        /// Avg func
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Avg<T>(T value)
        {
            return value;
        }

        /// <summary>
        /// Count func
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Count<T>(T value)
        {
            return 0;
        }

        /// <summary>
        /// Sum func
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Sum<T>(T value)
        {
            return value;
        }

        /// <summary>
        /// Json value func
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T JsonValue<T>(object value, string path)
        {
            return default;
        }

        /// <summary>
        /// Json object func
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T JsonObject<T>(object value, string path)
        {
            return default;
        }

        /// <summary>
        /// Is null
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns></returns>
        public static bool IsNull(object value)
        {
            return true;
        }

        /// <summary>
        /// Not null
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool NotNull(object value)
        {
            return true;
        }

        #endregion
    }
}
