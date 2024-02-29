using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Field;
using Sixnet.Expressions.Linq;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Default queryable five
    /// </summary>
    internal partial class DefaultQueryableFive<TFirst, TSecond, TThird, TFourth, TFifth> : DefaultQueryableFour<TFirst, TSecond, TThird, TFourth>, ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth>
    {
        #region Constructor

        public DefaultQueryableFive(ISixnetQueryable sourceQueryable) : base(sourceQueryable) { }

        public DefaultQueryableFive(QueryableContext sourceQueryableContext) : base(sourceQueryableContext) { }

        #endregion

        #region Condition

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="condition">Condition</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Where(ISixnetCondition condition)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> WhereIf(bool predicate, ISixnetCondition condition)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Where(Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> WhereIf(bool predicate, Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Where(Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Where(Expression<Func<TFirst, TSecond, TThird, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Where(Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Where(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        /// Order by fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderBy(IEnumerable<string> fieldNames, bool desc = false, Type targetType = null)
        {
            base.OrderBy(fieldNames, desc, targetType);
            return this;
        }

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderBy(string fieldName, bool desc = false, Type targetType = null)
        {
            base.OrderBy(fieldName, desc, targetType);
            return this;
        }

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderBy(ISixnetDataField field, bool desc = false)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderBy(IEnumerable<ISixnetDataField> fields, bool desc = false)
        {
            base.OrderBy(fields, desc);
            return this;
        }

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fieldNames">Field names</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderByIf(bool predicate, IEnumerable<string> fieldNames, bool desc = false, Type targetType = null)
        {
            base.OrderByIf(predicate, fieldNames, desc, targetType);
            return this;
        }

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderByIf(bool predicate, string fieldName, bool desc = false, Type targetType = null)
        {
            base.OrderByIf(predicate, fieldName, desc, targetType);
            return this;
        }

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderByIf(bool predicate, ISixnetDataField field, bool desc = false)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderByIf(bool predicate, IEnumerable<ISixnetDataField> fields, bool desc = false)
        {
            base.OrderByIf(predicate, fields, desc);
            return this;
        }

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderBy(Expression<Func<TFirst, object>> field, bool desc = false)
        {
            OrderByExpressionField<TFirst>(field, desc);
            return this;
        }

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderByIf(bool predicate, Expression<Func<TFirst, object>> field, bool desc = false)
        {
            return predicate
                ? OrderBy(field, desc)
                : this;
        }

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderBy(Expression<Func<TSecond, object>> field, bool desc = false)
        {
            OrderByExpressionField(field, desc);
            return this;
        }

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderByIf(bool predicate, Expression<Func<TSecond, object>> field, bool desc = false)
        {
            return predicate
                    ? OrderBy(field, desc)
                    : this;
        }

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderBy(Expression<Func<TThird, object>> field, bool desc = false)
        {
            OrderByExpressionField(field, desc);
            return this;
        }

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderByIf(bool predicate, Expression<Func<TThird, object>> field, bool desc = false)
        {
            return predicate
                    ? OrderBy(field, desc)
                    : this;
        }

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderBy(Expression<Func<TFourth, object>> field, bool desc = false)
        {
            OrderByExpressionField(field, desc);
            return this;
        }

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderByIf(bool predicate, Expression<Func<TFourth, object>> field, bool desc = false)
        {
            return predicate
                    ? OrderBy(field, desc)
                    : this;
        }

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderBy(Expression<Func<TFifth, object>> field, bool desc = false)
        {
            OrderByExpressionField(field, desc);
            return this;
        }

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderByIf(bool predicate, Expression<Func<TFifth, object>> field, bool desc = false)
        {
            return predicate
                    ? OrderBy(field, desc)
                    : this;
        }

        #endregion

        #region Join

        #region Inner join

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> InnerJoin<TSixth>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null)
        {
            return InnerJoin(null, joinExpression);
        }

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="sixthQueryable">Fifth queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> InnerJoin<TSixth>(ISixnetQueryable<TSixth> sixthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null)
        {
            return Join(true, JoinType.InnerJoin, sixthQueryable, joinExpression);
        }

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> InnerJoinIf<TSixth>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null)
        {
            return InnerJoinIf(predicate, null, joinExpression);
        }

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="sixthQueryable">Fifth queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> InnerJoinIf<TSixth>(bool predicate, ISixnetQueryable<TSixth> sixthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null)
        {
            return Join(predicate, JoinType.InnerJoin, sixthQueryable, joinExpression);
        }

        #endregion

        #region Left join

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> LeftJoin<TSixth>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null)
        {
            return LeftJoin(null, joinExpression);
        }

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="sixthQueryable">Fifth queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> LeftJoin<TSixth>(ISixnetQueryable<TSixth> sixthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null)
        {
            return Join(true, JoinType.LeftJoin, sixthQueryable, joinExpression);
        }

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> LeftJoinIf<TSixth>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null)
        {
            return LeftJoinIf(predicate, null, joinExpression);
        }

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="sixthQueryable">Fifth queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> LeftJoinIf<TSixth>(bool predicate, ISixnetQueryable<TSixth> sixthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null)
        {
            return Join(predicate, JoinType.LeftJoin, sixthQueryable, joinExpression);
        }

        #endregion

        #region Right join

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> RightJoin<TSixth>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null)
        {
            return RightJoin(null, joinExpression);
        }

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="sixthQueryable">Fifth queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> RightJoin<TSixth>(ISixnetQueryable<TSixth> sixthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null)
        {
            return Join(true, JoinType.RightJoin, sixthQueryable, joinExpression);
        }

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> RightJoinIf<TSixth>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null)
        {
            return RightJoinIf(predicate, null, joinExpression);
        }

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="sixthQueryable">Fifth queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> RightJoinIf<TSixth>(bool predicate, ISixnetQueryable<TSixth> sixthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null)
        {
            return Join(predicate, JoinType.RightJoin, sixthQueryable, joinExpression);
        }

        #endregion

        #region Full join

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> FullJoin<TSixth>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null)
        {
            return FullJoin(null, joinExpression);
        }

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="sixthQueryable">Fifth queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> FullJoin<TSixth>(ISixnetQueryable<TSixth> sixthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null)
        {
            return Join(true, JoinType.FullJoin, sixthQueryable, joinExpression);
        }

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> FullJoinIf<TSixth>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null)
        {
            return FullJoinIf(predicate, null, joinExpression);
        }

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="sixthQueryable">Fifth queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> FullJoinIf<TSixth>(bool predicate, ISixnetQueryable<TSixth> sixthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null)
        {
            return Join(predicate, JoinType.FullJoin, sixthQueryable, joinExpression);
        }

        #endregion

        #region Cross join

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> CrossJoin<TSixth>()
        {
            return CrossJoin<TSixth>(null);
        }

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="sixthQueryable">Sixth queryable</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> CrossJoin<TSixth>(ISixnetQueryable<TSixth> sixthQueryable)
        {
            return Join(true, JoinType.CrossJoin, sixthQueryable);
        }

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> CrossJoinIf<TSixth>(bool predicate)
        {
            return CrossJoinIf<TSixth>(predicate);
        }

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="sixthQueryable">Sixth queryable</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> CrossJoinIf<TSixth>(bool predicate, ISixnetQueryable<TSixth> sixthQueryable)
        {
            return Join(predicate, JoinType.CrossJoin, sixthQueryable);
        }

        #endregion 

        #region Add join

        /// <summary>
        /// Add join
        /// </summary>
        /// <param name="joinEntries">Join entries</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Join(params JoinEntry[] joinEntries)
        {
            base.Join(joinEntries);
            return this;
        }

        #endregion

        #region Join core

        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Join<TSixth>(bool predicate, JoinType joinType, ISixnetQueryable<TSixth> sixthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null)
        {
            if (predicate)
            {
                var joinQueryable = SixnetExpressionHelper.GetQueryable(joinExpression, CriterionConnector.And);
                var targetQueryable = SixnetQueryable.Create<TSixth>();
                if (sixthQueryable != null)
                {
                    targetQueryable.From(sixthQueryable);
                }
                queryableContext.Join(new JoinEntry()
                {
                    TargetQueryable = targetQueryable,
                    Type = joinType,
                    Connection = joinQueryable
                });
            }
            return SixnetQueryable.Create<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(this);
        }

        #endregion

        #endregion

        #region Select

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Select(params ISixnetDataField[] fields)
        {
            base.Select(fields);
            return this;
        }

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Select(params string[] fieldNames)
        {
            base.Select(fieldNames);
            return this;
        }

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Unselect(params ISixnetDataField[] fields)
        {
            base.Unselect(fields);
            return this;
        }

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Unselect(params string[] fieldNames)
        {
            base.Unselect(fieldNames);
            return this;
        }

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Select(params Expression<Func<TFirst, object>>[] fields)
        {
            IncludeExpressionFieldsCore(fields);
            return this;
        }

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Unselect(params Expression<Func<TFirst, object>>[] fields)
        {
            ExcludeExpressionFieldsCore(fields);
            return this;
        }

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Select(params Expression<Func<TSecond, object>>[] fields)
        {
            IncludeExpressionFieldsCore(fields);
            return this;
        }

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Unselect(params Expression<Func<TSecond, object>>[] fields)
        {
            ExcludeExpressionFieldsCore(fields);
            return this;
        }

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Select(params Expression<Func<TThird, object>>[] fields)
        {
            IncludeExpressionFieldsCore(fields);
            return this;
        }

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Unselect(params Expression<Func<TThird, object>>[] fields)
        {
            ExcludeExpressionFieldsCore(fields);
            return this;
        }

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Select(params Expression<Func<TFourth, object>>[] fields)
        {
            IncludeExpressionFieldsCore(fields);
            return this;
        }

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Unselect(params Expression<Func<TFourth, object>>[] fields)
        {
            ExcludeExpressionFieldsCore(fields);
            return this;
        }

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Select(params Expression<Func<TFifth, object>>[] fields)
        {
            IncludeExpressionFieldsCore(fields);
            return this;
        }

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Unselect(params Expression<Func<TFifth, object>>[] fields)
        {
            ExcludeExpressionFieldsCore(fields);
            return this;
        }

        #endregion

        #region Tree

        /// <summary>
        /// Tree matching
        /// </summary>
        /// <param name="dataFieldName">Data field name</param>
        /// <param name="parentFieldName">Parent field name</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> TreeMatching(string dataFieldName, string parentFieldName, TreeMatchingDirection direction = TreeMatchingDirection.Down)
        {
            base.TreeMatching(dataFieldName, parentFieldName, direction);
            return this;
        }

        /// <summary>
        /// Tree matching
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> TreeMatching(ISixnetDataField dataField, ISixnetDataField parentField, TreeMatchingDirection direction = TreeMatchingDirection.Down)
        {
            base.TreeMatching(dataField, parentField, direction);
            return this;
        }

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> TreeMatching(Expression<Func<TFirst, object>> dataField, Expression<Func<TFirst, object>> parentField, TreeMatchingDirection direction = TreeMatchingDirection.Down)
        {
            TreeMatching(SixnetExpressionHelper.GetDataField(dataField), SixnetExpressionHelper.GetDataField(parentField), direction);
            return this;
        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns>Return the replicated Queryable</returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> LightClone()
        {
            return LightCloneCore() as ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth>;
        }

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns>Return the replicated Queryable</returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Clone()
        {
            return CloneCore() as ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth>;
        }

        protected override ISixnetQueryable LightCloneCore()
        {
            return new DefaultQueryableFive<TFirst, TSecond, TThird, TFourth, TFifth>(queryableContext?.LightClone());
        }

        protected override ISixnetQueryable CloneCore()
        {
            return new DefaultQueryableFive<TFirst, TSecond, TThird, TFourth, TFifth>(queryableContext?.Clone());
        }

        #endregion

        #region Combine

        #region UnionAll

        /// <summary>
        /// Union all
        /// </summary>
        /// <param name="unionQueryable">Union queryable</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> UnionAll(ISixnetQueryable unionQueryable)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> UnionAll<TTarget>(Expression<Func<TTarget, bool>> unionExpression = null)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Union(ISixnetQueryable unionQueryable)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Union<TTarget>(Expression<Func<TTarget, bool>> unionExpression = null)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Except(ISixnetQueryable exceptQueryable)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Except<TTarget>(Expression<Func<TTarget, bool>> exceptExpression = null)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Intersect(ISixnetQueryable intersectQueryable)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Intersect<TTarget>(Expression<Func<TTarget, bool>> intersectExpression = null)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> SetModelType(Type modelType)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Take(int count, int skip = 0)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Distinct()
        {
            queryableContext.Distinct();
            return this;
        }

        #endregion

        #region Group

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> GroupBy(params string[] fieldNames)
        {
            base.GroupBy(fieldNames);
            return this;
        }

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> GroupBy(params ISixnetDataField[] fields)
        {
            base.GroupBy(fields);
            return this;
        }

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> GroupBy(params Expression<Func<TFirst, object>>[] fields)
        {
            GroupByExpression(fields);
            return this;
        }

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> GroupBy(params Expression<Func<TSecond, object>>[] fields)
        {
            GroupByExpression(fields);
            return this;
        }

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> GroupBy(params Expression<Func<TThird, object>>[] fields)
        {
            GroupByExpression(fields);
            return this;
        }

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> GroupBy(params Expression<Func<TFourth, object>>[] fields)
        {
            GroupByExpression(fields);
            return this;
        }

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> GroupBy(params Expression<Func<TFifth, object>>[] fields)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Having(Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> HavingIf(bool predicate, Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Having(Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Having(Expression<Func<TFirst, TSecond, TThird, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Having(Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Having(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> From(ISixnetQueryable targetQueryable)
        {
            base.From(targetQueryable);
            return this;
        }

        #endregion

        #region Split table

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitValue">Split value</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> SplitTable(dynamic splitValue)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> SplitTable(dynamic startSplitValue, dynamic endSplitValue)
        {
            SplitTableCore(startSplitValue, endSplitValue);
            return this;
        }

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitValues">Split values</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> SplitTable(IEnumerable<dynamic> splitValues, SplitTableNameSelectionPattern selectionPattern = SplitTableNameSelectionPattern.Precision)
        {
            base.SplitTable(splitValues, selectionPattern);
            return this;
        }

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitTableNameFilter">Split table name filter</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> SplitTable(Func<IEnumerable<string>, IEnumerable<string>> splitTableNameFilter)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Output(QueryableOutputType outputType)
        {
            base.Output(outputType);
            return this;
        }

        #endregion

        #region Archived

        /// <summary>
        /// Include archived
        /// </summary>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> IncludeArchived()
        {
            queryableContext.IncludeArchived();
            return this;
        }

        #endregion

        #region Isolation

        /// <summary>
        /// Unisolated
        /// </summary>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Unisolated()
        {
            queryableContext.Unisolated();
            return this;
        }

        #endregion

        #region Negate

        /// <summary>
        /// Negate
        /// </summary>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Negate()
        {
            base.Negate();
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
        public List<TReturn> ToList<TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return ToList<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(dataMappingFunc, configure);
        }

        #endregion

        #endregion
    }
}
