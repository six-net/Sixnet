// "Company © 2025. All rights reserved."

using Sixnet.Development.Data;
using Sixnet.Development.Data.Database;
using Sixnet.Development.Data.Field;
using Sixnet.Expressions.Linq;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Default queryable four
    /// </summary>
    internal partial class SixnetDefaultQueryableFour<TFirst, TSecond, TThird, TFourth> : SixnetDefaultQueryableThree<TFirst, TSecond, TThird>, ISixnetQueryable<TFirst, TSecond, TThird, TFourth>
    {
        #region Constructor

        public SixnetDefaultQueryableFour(ISixnetQueryable sourceQueryable) : base(sourceQueryable) { }

        public SixnetDefaultQueryableFour(SixnetQueryableInfo sourceQueryableContext) : base(sourceQueryableContext) { }

        #endregion

        #region Condition

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="condition">Condition</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Where(ISixnetCondition condition)
        {
            base.Where(condition);
            return this;
        }

        /// <summary>
        /// Append group condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="condition">Group condition</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> WhereIf(bool predicate, ISixnetCondition condition)
        {
            base.WhereIf(predicate, condition);
            return this;
        }

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Where(Expression<Func<TFirst, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And)
        {
            WhereExpressionCore(expression, connector);
            return this;
        }

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> WhereIf(bool predicate, Expression<Func<TFirst, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And)
        {
            if (predicate)
            {
                WhereExpressionCore(expression, connector);
            }
            return this;
        }

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Where(Expression<Func<TFirst, TSecond, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And)
        {
            WhereExpressionCore(expression, connector);
            return this;
        }

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And)
        {
            if (predicate)
            {
                WhereExpressionCore(expression, connector);
            }
            return this;
        }

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Where(Expression<Func<TFirst, TSecond, TThird, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And)
        {
            WhereExpressionCore(expression, connector);
            return this;
        }

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And)
        {
            if (predicate)
            {
                WhereExpressionCore(expression, connector);
            }
            return this;
        }

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Where(Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And)
        {
            WhereExpressionCore(expression, connector);
            return this;
        }

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And)
        {
            if (predicate)
            {
                WhereExpressionCore(expression, connector);
            }
            return this;
        }

        #endregion

        #region Sort

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderBy(ISixnetField field, bool desc = false)
        {
            base.OrderBy(field, desc);
            return this;
        }

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderBy(IEnumerable<ISixnetField> fields, bool desc = false)
        {
            base.OrderBy(fields, desc);
            return this;
        }

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderByIf(bool predicate, ISixnetField field, bool desc = false)
        {
            base.OrderByIf(predicate, field, desc);
            return this;
        }

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderByIf(bool predicate, IEnumerable<ISixnetField> fields, bool desc = false)
        {
            base.OrderByIf(predicate, fields, desc);
            return this;
        }

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderBy(Expression<Func<TFirst, object>> fields, bool desc = false)
        {
            OrderByExpressionField(fields, desc);
            return this;
        }

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderByIf(bool predicate, Expression<Func<TFirst, object>> fields, bool desc = false)
        {
            return predicate
                ? OrderBy(fields, desc)
                : this;
        }

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderBy(Expression<Func<TFirst, TSecond, object>> fields, bool desc = false)
        {
            OrderByExpressionField(fields, desc);
            return this;
        }

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderByIf(bool predicate, Expression<Func<TFirst, TSecond, object>> fields, bool desc = false)
        {
            return predicate
                    ? OrderBy(fields, desc)
                    : this;
        }

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderBy(Expression<Func<TFirst, TSecond, TThird, object>> fields, bool desc = false)
        {
            OrderByExpressionField(fields, desc);
            return this;
        }

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderByIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, object>> fields, bool desc = false)
        {
            return predicate
                    ? OrderBy(fields, desc)
                    : this;
        }

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderBy(Expression<Func<TFirst, TSecond, TThird, TFourth, object>> fields, bool desc = false)
        {
            OrderByExpressionField(fields, desc);
            return this;
        }

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderByIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, object>> fields, bool desc = false)
        {
            return predicate
                    ? OrderBy(fields, desc)
                    : this;
        }

        #endregion

        #region Join

        #region Inner join

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> InnerJoin<TFifth>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            return InnerJoin(null, connection, configure);
        }

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="fifthQueryable">Fifth queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> InnerJoin<TFifth>(ISixnetQueryable fifthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            return Join(true, SixnetJoinType.InnerJoin, fifthQueryable, connection, configure);
        }

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> InnerJoinIf<TFifth>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            return InnerJoinIf(predicate, null, connection, configure);
        }

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="fifthQueryable">Fifth queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> InnerJoinIf<TFifth>(bool predicate, ISixnetQueryable fifthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            return Join(predicate, SixnetJoinType.InnerJoin, fifthQueryable, connection, configure);
        }

        #endregion

        #region Left join

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> LeftJoin<TFifth>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            return LeftJoin(null, connection, configure);
        }

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="fifthQueryable">Fifth queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> LeftJoin<TFifth>(ISixnetQueryable fifthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            return Join(true, SixnetJoinType.LeftJoin, fifthQueryable, connection, configure);
        }

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> LeftJoinIf<TFifth>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            return LeftJoinIf(predicate, null, connection, configure);
        }

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="fifthQueryable">Fifth queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> LeftJoinIf<TFifth>(bool predicate, ISixnetQueryable fifthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            return Join(predicate, SixnetJoinType.LeftJoin, fifthQueryable, connection, configure);
        }

        #endregion

        #region Right join

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> RightJoin<TFifth>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            return RightJoin(null, connection, configure);
        }

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="fifthQueryable">Fifth queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> RightJoin<TFifth>(ISixnetQueryable fifthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            return Join(true, SixnetJoinType.RightJoin, fifthQueryable, connection, configure);
        }

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> RightJoinIf<TFifth>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            return RightJoinIf(predicate, null, connection, configure);
        }

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="fifthQueryable">Fifth queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> RightJoinIf<TFifth>(bool predicate, ISixnetQueryable fifthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            return Join(predicate, SixnetJoinType.RightJoin, fifthQueryable, connection, configure);
        }

        #endregion

        #region Full join

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> FullJoin<TFifth>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            return FullJoin(null, connection, configure);
        }

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="fifthQueryable">Fifth queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> FullJoin<TFifth>(ISixnetQueryable fifthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            return Join(true, SixnetJoinType.FullJoin, fifthQueryable, connection, configure);
        }

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> FullJoinIf<TFifth>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            return FullJoinIf(predicate, null, connection, configure);
        }

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="fifthQueryable">Fifth queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> FullJoinIf<TFifth>(bool predicate, ISixnetQueryable fifthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            return Join(predicate, SixnetJoinType.FullJoin, fifthQueryable, connection, configure);
        }

        #endregion

        #region Cross join

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> CrossJoin<TFifth>(Action<SixnetJoinEntry> configure = null)
        {
            return CrossJoin<TFifth>(null);
        }

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="fifthQueryable">Third queryable</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> CrossJoin<TFifth>(ISixnetQueryable fifthQueryable, Action<SixnetJoinEntry> configure = null)
        {
            return Join<TFifth>(true, SixnetJoinType.CrossJoin, fifthQueryable);
        }

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> CrossJoinIf<TFifth>(bool predicate, Action<SixnetJoinEntry> configure = null)
        {
            return CrossJoinIf<TFifth>(predicate, null);
        }

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="fifthQueryable">Third queryable</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> CrossJoinIf<TFifth>(bool predicate, ISixnetQueryable fifthQueryable, Action<SixnetJoinEntry> configure = null)
        {
            return Join<TFifth>(predicate, SixnetJoinType.CrossJoin, fifthQueryable);
        }

        #endregion 

        #region Add join

        /// <summary>
        /// Add join
        /// </summary>
        /// <param name="joinEntry">Join entry</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Join(SixnetJoinEntry joinEntry, Action<SixnetJoinEntry> configure = null)
        {
            base.Join(joinEntry, configure);
            return this;
        }

        #endregion

        #region Join core

        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Join<TFifth>(bool predicate, SixnetJoinType joinType, ISixnetQueryable fifthQueryable
            , Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            if (predicate)
            {
                var joinQueryable = SixnetExpressionHelper.GetQueryable(connection, SixnetCriterionConnector.And);
                var targetQueryable = SixnetQuerier.Create<TFifth>();
                if (fifthQueryable != null)
                {
                    targetQueryable.From(fifthQueryable);
                }
                Join(new SixnetJoinEntry()
                {
                    Target = targetQueryable,
                    Type = joinType,
                    Connection = joinQueryable
                }, configure);
            }
            else
            {
                queryableInfo.IncrementJoinIndex();
            }
            return SixnetQuerier.Create<TFirst, TSecond, TThird, TFourth, TFifth>(this);
        }

        #endregion

        #endregion

        #region Select

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> SelectFields(params ISixnetField[] fields)
        {
            base.SelectFields(fields);
            return this;
        }

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> SelectFields<TResult>(Expression<Func<TFirst, TResult>> fields)
        {
            IncludeExpressionFieldsCore(fields);
            return this;
        }



        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> SelectFields<TResult>(Expression<Func<TFirst, TSecond, TResult>> fields)
        {
            IncludeExpressionFieldsCore(fields);
            return this;
        }

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> SelectFields<TResult>(Expression<Func<TFirst, TSecond, TThird, TResult>> fields)
        {
            IncludeExpressionFieldsCore(fields);
            return this;
        }

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth> SelectFields<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TResult>> fields)
        {
            IncludeExpressionFieldsCore(fields);
            return this;
        }

        /// <summary>
        /// Select fields as a data source
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TResult> SelectAsSource<TResult>(Expression<Func<TFirst, TResult>> fields)
        {
            return IncludeExpressionFieldsAsSourceCore<TResult>(fields);
        }

        /// <summary>
        /// Select fields as a data source
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TResult> SelectAsSource<TResult>(Expression<Func<TFirst, TSecond, TResult>> fields)
        {
            return IncludeExpressionFieldsAsSourceCore<TResult>(fields);
        }

        /// <summary>
        /// Select fields as a data source
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TResult> SelectAsSource<TResult>(Expression<Func<TFirst, TSecond, TThird, TResult>> fields)
        {
            return IncludeExpressionFieldsAsSourceCore<TResult>(fields);
        }

        /// <summary>
        /// Select fields as a data source
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public ISixnetQueryable<TResult> SelectAsSource<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TResult>> fields)
        {
            return IncludeExpressionFieldsAsSourceCore<TResult>(fields);
        }

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Unselect(Expression<Func<TFirst, TSecond, TThird, TFourth, object>> fields)
        {
            ExcludeExpressionFieldsCore(fields);
            return this;
        }

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Unselect(params ISixnetField[] fields)
        {
            base.Unselect(fields);
            return this;
        }

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Unselect(Expression<Func<TFirst, TSecond, TThird, object>> fields)
        {
            ExcludeExpressionFieldsCore(fields);
            return this;
        }

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Unselect(Expression<Func<TFirst, TSecond, object>> fields)
        {
            ExcludeExpressionFieldsCore(fields);
            return this;
        }

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Unselect(Expression<Func<TFirst, object>> fields)
        {
            ExcludeExpressionFieldsCore(fields);
            return this;
        }

        /// <summary>
        /// Select fields as a temp table
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public ISixnetQueryable<TResult> SelectAsTempTable<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TResult>> fields)
        {
            return IncludeExpressionFieldsAsTempTableCore<TResult>(fields);
        }

        #endregion

        #region Tree

        /// <summary>
        /// Tree matching
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Tree(ISixnetField dataField, ISixnetField parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down)
        {
            base.Tree(dataField, parentField, direction);
            return this;
        }

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Tree(Expression<Func<TFirst, object>> dataField, Expression<Func<TFirst, object>> parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down)
        {
            Tree(SixnetExpressionHelper.GetDataField(dataField), SixnetExpressionHelper.GetDataField(parentField), direction);
            return this;
        }

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Tree(Expression<Func<TFirst, TSecond, object>> dataField, Expression<Func<TFirst, TSecond, object>> parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down)
        {
            Tree(SixnetExpressionHelper.GetDataField(dataField), SixnetExpressionHelper.GetDataField(parentField), direction);
            return this;
        }

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Tree(Expression<Func<TFirst, TSecond, TThird, object>> dataField, Expression<Func<TFirst, TSecond, TThird, object>> parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down)
        {
            Tree(SixnetExpressionHelper.GetDataField(dataField), SixnetExpressionHelper.GetDataField(parentField), direction);
            return this;
        }

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Tree(Expression<Func<TFirst, TSecond, TThird, TFourth, object>> dataField, Expression<Func<TFirst, TSecond, TThird, TFourth, object>> parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down)
        {
            Tree(SixnetExpressionHelper.GetDataField(dataField), SixnetExpressionHelper.GetDataField(parentField), direction);
            return this;
        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns>Return the replicated Queryable</returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> LightClone()
        {
            return LightCloneCore() as ISixnetQueryable<TFirst, TSecond, TThird, TFourth>;
        }

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns>Return the replicated Queryable</returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Clone()
        {
            return CloneCore() as ISixnetQueryable<TFirst, TSecond, TThird, TFourth>;
        }

        protected override ISixnetQueryable LightCloneCore()
        {
            return new SixnetDefaultQueryableFour<TFirst, TSecond, TThird, TFourth>(queryableInfo?.LightClone());
        }

        protected override ISixnetQueryable CloneCore()
        {
            return new SixnetDefaultQueryableFour<TFirst, TSecond, TThird, TFourth>(queryableInfo?.Clone());
        }

        #endregion

        #region Combine

        #region UnionAll

        /// <summary>
        /// Union all
        /// </summary>
        /// <param name="unionQueryable">Union queryable</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> UnionAll(ISixnetQueryable unionQueryable)
        {
            base.UnionAll(unionQueryable);
            return this;
        }

        /// <summary>
        /// Union all
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="unionExpression">Union expression</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> UnionAll<TTarget>(Expression<Func<TTarget, bool>> unionExpression = null)
        {
            base.UnionAll(unionExpression);
            return this;
        }

        #endregion

        #region Union

        /// <summary>
        /// Union
        /// </summary>
        /// <param name="unionQueryable">Union queryable</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Union(ISixnetQueryable unionQueryable)
        {
            base.Union(unionQueryable);
            return this;
        }

        /// <summary>
        /// Union
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="unionExpression">Union expression</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Union<TTarget>(Expression<Func<TTarget, bool>> unionExpression = null)
        {
            base.Union(unionExpression);
            return this;
        }

        #endregion

        #region Except

        /// <summary>
        /// Except
        /// </summary>
        /// <param name="exceptQueryable">Except queryable</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Except(ISixnetQueryable exceptQueryable)
        {
            base.Except(exceptQueryable);
            return this;
        }

        /// <summary>
        /// Except
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="exceptQueryable">Except expression</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Except<TTarget>(Expression<Func<TTarget, bool>> exceptExpression = null)
        {
            base.Except(exceptExpression);
            return this;
        }

        #endregion

        #region Intersect

        /// <summary>
        /// Intersect
        /// </summary>
        /// <param name="intersectQueryable">Intersect queryable</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Intersect(ISixnetQueryable intersectQueryable)
        {
            base.Intersect(intersectQueryable);
            return this;
        }

        /// <summary>
        /// Intersect
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="intersectQueryable">Intersect expression</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Intersect<TTarget>(Expression<Func<TTarget, bool>> intersectExpression = null)
        {
            base.Intersect(intersectExpression);
            return this;
        }

        #endregion

        #endregion

        #region Model type

        /// <summary>
        /// Set model type
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> SetModelType(Type modelType)
        {
            base.SetModelType(modelType);
            return this;
        }

        #endregion

        #region Take

        /// <summary>
        /// Take
        /// </summary>
        /// <param name="count">Take data count</param>
        /// <param name="skip">Skip data count</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Take(int count, int skip = 0)
        {
            base.Take(count, skip);
            return this;
        }

        #endregion

        #region Distinct

        /// <summary>
        /// Distinct
        /// </summary>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Distinct()
        {
            queryableInfo.Distinct();
            return this;
        }

        #endregion

        #region Group

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> GroupBy(params string[] fieldNames)
        {
            base.GroupBy(fieldNames);
            return this;
        }

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> GroupBy(params ISixnetField[] fields)
        {
            base.GroupBy(fields);
            return this;
        }

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> GroupBy(Expression<Func<TFirst, object>> fields)
        {
            GroupByExpression(fields);
            return this;
        }

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> GroupBy(Expression<Func<TFirst, TSecond, object>> fields)
        {
            GroupByExpression(fields);
            return this;
        }

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> GroupBy(Expression<Func<TFirst, TSecond, TThird, object>> fields)
        {
            GroupByExpression(fields);
            return this;
        }

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth> GroupBy(Expression<Func<TFirst, TSecond, TThird, TFourth, object>> fields)
        {
            GroupByExpression(fields);
            return this;
        }

        #endregion

        #region Having

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Having(Expression<Func<TFirst, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And)
        {
            return HavingIf(true, expression, connector);
        }

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> HavingIf(bool predicate, Expression<Func<TFirst, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And)
        {
            HavingExpressionCore(predicate, expression, connector);
            return this;
        }

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Having(Expression<Func<TFirst, TSecond, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And)
        {
            return HavingIf(true, expression, connector);
        }

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And)
        {
            HavingExpressionCore(predicate, expression, connector);
            return this;
        }

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Having(Expression<Func<TFirst, TSecond, TThird, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And)
        {
            return HavingIf(true, expression, connector);
        }

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And)
        {
            HavingExpressionCore(predicate, expression, connector);
            return this;
        }

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Having(Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And)
        {
            return HavingIf(true, expression, connector);
        }

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And)
        {
            HavingExpressionCore(predicate, expression, connector);
            return this;
        }

        #endregion

        #region From

        /// <summary>
        /// Query from other queryable
        /// </summary>
        /// <param name="targetQueryable">Target queryable</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> From(ISixnetQueryable targetQueryable)
        {
            base.From(targetQueryable);
            return this;
        }

        /// <summary>
        /// From specify table
        /// </summary>
        /// <param name="tableNames">Table names</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> From(params string[] tableNames)
        {
            FromSpecifyTableCore(tableNames);
            return this;
        }

        #endregion

        #region Split table

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitValue">Split value</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> SplitTable(dynamic splitValue)
        {
            SplitTableCore(splitValue);
            return this;
        }

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="startSplitValue">Start split value</param>
        /// <param name="endSplitValue">End split value</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> SplitTable(dynamic startSplitValue, dynamic endSplitValue)
        {
            SplitTableCore(startSplitValue, endSplitValue);
            return this;
        }

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitValues">Split values</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> SplitTable(IEnumerable<dynamic> splitValues, SixnetSplitTableNameSelectionPattern selectionPattern = SixnetSplitTableNameSelectionPattern.Precision)
        {
            base.SplitTable(splitValues, selectionPattern);
            return this;
        }

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitTableNameFilter">Split table name filter</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> SplitTable(Func<IEnumerable<SixnetDatabaseObjectName>, IEnumerable<SixnetDatabaseObjectName>, IEnumerable<SixnetDatabaseObjectName>> splitTableNameFilter)
        {
            base.SplitTable(splitTableNameFilter);
            return this;
        }

        #endregion

        #region Output

        /// <summary>
        /// Set output
        /// </summary>
        /// <param name="outputType">Output type</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Output(SixnetQueryableOutputType outputType)
        {
            base.Output(outputType);
            return this;
        }

        #endregion

        #region Filter

        /// <summary>
        /// Include archived
        /// </summary>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> IncludeArchived()
        {
            base.IncludeArchived();
            return this;
        }

        /// <summary>
        /// Ignore data isolation
        /// </summary>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> IgnoreIsolation()
        {
            base.IgnoreIsolation();
            return this;
        }

        /// <summary>
        /// Ignore filter
        /// </summary>
        /// <typeparam name="TFilter">Filter type</typeparam>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> IgnoreFilter<TFilter>()
        {
            base.IgnoreFilter<TFilter>();
            return this;
        }

        /// <summary>
        /// Ignore filter
        /// </summary>
        /// <param name="filterType">Filter type</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> IgnoreFilter(Type filterType)
        {
            base.IgnoreFilter(filterType);
            return this;
        }

        #endregion

        #region Negate

        /// <summary>
        /// Negate
        /// </summary>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Negate()
        {
            base.Negate();
            return this;
        }

        #endregion

        #region Read only

        /// <summary>
        /// Mark as read only
        /// </summary>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> ReadOnly()
        {
            base.ReadOnly();
            return this;
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
        public List<TReturn> ToList<TReturn>(Func<TFirst, TSecond, TThird, TFourth, TReturn> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null)
        {
            return ToList<TFirst, TSecond, TThird, TFourth, TReturn>(dataMappingFunc, configure);
        }

        #endregion

        #endregion
    }
}
