﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Field;
using Sixnet.Expressions.Linq;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Default queryable six
    /// </summary>
    internal partial class DefaultQueryableSix<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> : DefaultQueryableFive<TFirst, TSecond, TThird, TFourth, TFifth>, ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>
    {
        #region Constructor

        public DefaultQueryableSix(ISixnetQueryable sourceQueryable) : base(sourceQueryable) { }

        public DefaultQueryableSix(QueryableContext sourceQueryableContext) : base(sourceQueryableContext) { }

        #endregion

        #region Condition

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="condition">Condition</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Where(ISixnetCondition condition)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> WhereIf(bool predicate, ISixnetCondition condition)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Where(Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> WhereIf(bool predicate, Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Where(Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Where(Expression<Func<TFirst, TSecond, TThird, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Where(Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Where(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Where(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderBy(IEnumerable<string> fieldNames, bool desc = false, Type targetType = null)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderBy(string fieldName, bool desc = false, Type targetType = null)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderBy(ISixnetDataField field, bool desc = false)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderBy(IEnumerable<ISixnetDataField> fields, bool desc = false)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderByIf(bool predicate, IEnumerable<string> fieldNames, bool desc = false, Type targetType = null)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderByIf(bool predicate, string fieldName, bool desc = false, Type targetType = null)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderByIf(bool predicate, ISixnetDataField field, bool desc = false)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderByIf(bool predicate, IEnumerable<ISixnetDataField> fields, bool desc = false)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderBy(Expression<Func<TFirst, object>> field, bool desc = false)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderByIf(bool predicate, Expression<Func<TFirst, object>> field, bool desc = false)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderBy(Expression<Func<TSecond, object>> field, bool desc = false)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderByIf(bool predicate, Expression<Func<TSecond, object>> field, bool desc = false)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderBy(Expression<Func<TThird, object>> field, bool desc = false)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderByIf(bool predicate, Expression<Func<TThird, object>> field, bool desc = false)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderBy(Expression<Func<TFourth, object>> field, bool desc = false)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderByIf(bool predicate, Expression<Func<TFourth, object>> field, bool desc = false)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderBy(Expression<Func<TFifth, object>> field, bool desc = false)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderByIf(bool predicate, Expression<Func<TFifth, object>> field, bool desc = false)
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
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderBy(Expression<Func<TSixth, object>> field, bool desc = false)
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
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderByIf(bool predicate, Expression<Func<TSixth, object>> field, bool desc = false)
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
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> InnerJoin<TSeventh>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null)
        {
            return InnerJoin(null, joinExpression);
        }

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="seventhQueryable">Seventh queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> InnerJoin<TSeventh>(ISixnetQueryable<TSeventh> seventhQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null)
        {
            return Join(true, JoinType.InnerJoin, seventhQueryable, joinExpression);
        }

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> InnerJoinIf<TSeventh>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null)
        {
            return InnerJoinIf(predicate, null, joinExpression);
        }

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="seventhQueryable">Seventh queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> InnerJoinIf<TSeventh>(bool predicate, ISixnetQueryable<TSeventh> seventhQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null)
        {
            return Join(predicate, JoinType.InnerJoin, seventhQueryable, joinExpression);
        }

        #endregion

        #region Left join

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> LeftJoin<TSeventh>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null)
        {
            return LeftJoin(null, joinExpression);
        }

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="seventhQueryable">Seventh queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> LeftJoin<TSeventh>(ISixnetQueryable<TSeventh> seventhQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null)
        {
            return Join(true, JoinType.LeftJoin, seventhQueryable, joinExpression);
        }

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> LeftJoinIf<TSeventh>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null)
        {
            return LeftJoinIf(predicate, null, joinExpression);
        }

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="seventhQueryable">Seventh queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> LeftJoinIf<TSeventh>(bool predicate, ISixnetQueryable<TSeventh> seventhQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null)
        {
            return Join(predicate, JoinType.LeftJoin, seventhQueryable, joinExpression);
        }

        #endregion

        #region Right join

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> RightJoin<TSeventh>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null)
        {
            return RightJoin(null, joinExpression);
        }

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="seventhQueryable">Seventh queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> RightJoin<TSeventh>(ISixnetQueryable<TSeventh> seventhQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null)
        {
            return Join(true, JoinType.RightJoin, seventhQueryable, joinExpression);
        }

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> RightJoinIf<TSeventh>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null)
        {
            return RightJoinIf(predicate, null, joinExpression);
        }

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="seventhQueryable">Seventh queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> RightJoinIf<TSeventh>(bool predicate, ISixnetQueryable<TSeventh> seventhQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null)
        {
            return Join(predicate, JoinType.RightJoin, seventhQueryable, joinExpression);
        }

        #endregion

        #region Full join

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> FullJoin<TSeventh>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null)
        {
            return FullJoin(null, joinExpression);
        }

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="seventhQueryable">Seventh queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> FullJoin<TSeventh>(ISixnetQueryable<TSeventh> seventhQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null)
        {
            return Join(true, JoinType.FullJoin, seventhQueryable, joinExpression);
        }

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> FullJoinIf<TSeventh>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null)
        {
            return FullJoinIf(predicate, null, joinExpression);
        }

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="seventhQueryable">Seventh queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> FullJoinIf<TSeventh>(bool predicate, ISixnetQueryable<TSeventh> seventhQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null)
        {
            return Join(predicate, JoinType.FullJoin, seventhQueryable, joinExpression);
        }

        #endregion

        #region Cross join

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> CrossJoin<TSeventh>()
        {
            return CrossJoin<TSeventh>(null);
        }

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="seventhQueryable">Seventh queryable</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> CrossJoin<TSeventh>(ISixnetQueryable<TSeventh> seventhQueryable)
        {
            return Join(true, JoinType.CrossJoin, seventhQueryable);
        }

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> CrossJoinIf<TSeventh>(bool predicate)
        {
            return CrossJoinIf<TSeventh>(predicate, null);
        }

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="seventhQueryable">Seventh queryable</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> CrossJoinIf<TSeventh>(bool predicate, ISixnetQueryable<TSeventh> seventhQueryable)
        {
            return Join(predicate, JoinType.CrossJoin, seventhQueryable);
        }

        #endregion 

        #region Add join

        /// <summary>
        /// Add join
        /// </summary>
        /// <param name="joinEntries">Join entries</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Join(params JoinEntry[] joinEntries)
        {
            base.Join(joinEntries);
            return this;
        }

        #endregion

        #region Join core

        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Join<TSeventh>(bool predicate, JoinType joinType, ISixnetQueryable<TSeventh> seventhQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null)
        {
            if (predicate)
            {
                var joinQueryable = SixnetExpressionHelper.GetQueryable(joinExpression, CriterionConnector.And);
                var targetQueryable = SixnetQueryable.Create<TSeventh>();
                if (seventhQueryable != null)
                {
                    targetQueryable.From(seventhQueryable);
                }
                queryableContext.Join(new JoinEntry()
                {
                    TargetQueryable = targetQueryable,
                    Type = joinType,
                    Connection = joinQueryable
                });
            }
            return SixnetQueryable.Create<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(this);
        }

        #endregion

        #endregion

        #region Select

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Select(params ISixnetDataField[] fields)
        {
            base.Select(fields);
            return this;
        }

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Select(params string[] fieldNames)
        {
            base.Select(fieldNames);
            return this;
        }

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Unselect(params ISixnetDataField[] fields)
        {
            base.Unselect(fields);
            return this;
        }

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Unselect(params string[] fieldNames)
        {
            base.Unselect(fieldNames);
            return this;
        }

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Select(params Expression<Func<TFirst, object>>[] fields)
        {
            IncludeExpressionFieldsCore(fields);
            return this;
        }

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Unselect(params Expression<Func<TFirst, object>>[] fields)
        {
            ExcludeExpressionFieldsCore(fields);
            return this;
        }

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Select(params Expression<Func<TSecond, object>>[] fields)
        {
            IncludeExpressionFieldsCore(fields);
            return this;
        }

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Unselect(params Expression<Func<TSecond, object>>[] fields)
        {
            ExcludeExpressionFieldsCore(fields);
            return this;
        }

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Select(params Expression<Func<TThird, object>>[] fields)
        {
            IncludeExpressionFieldsCore(fields);
            return this;
        }

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Unselect(params Expression<Func<TThird, object>>[] fields)
        {
            ExcludeExpressionFieldsCore(fields);
            return this;
        }

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Select(params Expression<Func<TFourth, object>>[] fields)
        {
            IncludeExpressionFieldsCore(fields);
            return this;
        }

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Unselect(params Expression<Func<TFourth, object>>[] fields)
        {
            ExcludeExpressionFieldsCore(fields);
            return this;
        }

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Select(params Expression<Func<TFifth, object>>[] fields)
        {
            IncludeExpressionFieldsCore(fields);
            return this;
        }

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Unselect(params Expression<Func<TFifth, object>>[] fields)
        {
            ExcludeExpressionFieldsCore(fields);
            return this;
        }

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Select(params Expression<Func<TSixth, object>>[] fields)
        {
            IncludeExpressionFieldsCore(fields);
            return this;
        }

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Unselect(params Expression<Func<TSixth, object>>[] fields)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> TreeMatching(string dataFieldName, string parentFieldName, TreeMatchingDirection direction = TreeMatchingDirection.Down)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> TreeMatching(ISixnetDataField dataField, ISixnetDataField parentField, TreeMatchingDirection direction = TreeMatchingDirection.Down)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> TreeMatching(Expression<Func<TFirst, object>> dataField, Expression<Func<TFirst, object>> parentField, TreeMatchingDirection direction = TreeMatchingDirection.Down)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> LightClone()
        {
            return LightCloneCore() as ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>;
        }

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns>Return the replicated Queryable</returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Clone()
        {
            return CloneCore() as ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>;
        }

        protected override ISixnetQueryable LightCloneCore()
        {
            return new DefaultQueryableSix<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(queryableContext?.LightClone());
        }

        protected override ISixnetQueryable CloneCore()
        {
            return new DefaultQueryableSix<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(queryableContext?.Clone());
        }

        #endregion

        #region Combine

        #region UnionAll

        /// <summary>
        /// Union all
        /// </summary>
        /// <param name="unionQueryable">Union queryable</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> UnionAll(ISixnetQueryable unionQueryable)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> UnionAll<TTarget>(Expression<Func<TTarget, bool>> unionExpression = null)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Union(ISixnetQueryable unionQueryable)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Union<TTarget>(Expression<Func<TTarget, bool>> unionExpression = null)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Except(ISixnetQueryable exceptQueryable)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Except<TTarget>(Expression<Func<TTarget, bool>> exceptExpression = null)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Intersect(ISixnetQueryable intersectQueryable)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Intersect<TTarget>(Expression<Func<TTarget, bool>> intersectExpression = null)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> SetModelType(Type modelType)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Take(int count, int skip = 0)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Distinct()
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> GroupBy(params string[] fieldNames)
        {
            base.GroupBy(fieldNames);
            return this;
        }

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> GroupBy(params ISixnetDataField[] fields)
        {
            base.GroupBy(fields);
            return this;
        }

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> GroupBy(params Expression<Func<TFirst, object>>[] fields)
        {
            GroupByExpression(fields);
            return this;
        }

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> GroupBy(params Expression<Func<TSecond, object>>[] fields)
        {
            GroupByExpression(fields);
            return this;
        }

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> GroupBy(params Expression<Func<TThird, object>>[] fields)
        {
            GroupByExpression(fields);
            return this;
        }

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> GroupBy(params Expression<Func<TFourth, object>>[] fields)
        {
            GroupByExpression(fields);
            return this;
        }

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> GroupBy(params Expression<Func<TFifth, object>>[] fields)
        {
            GroupByExpression(fields);
            return this;
        }

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> GroupBy(params Expression<Func<TSixth, object>>[] fields)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Having(Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> HavingIf(bool predicate, Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Having(Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Having(Expression<Func<TFirst, TSecond, TThird, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Having(Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Having(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Having(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> From(ISixnetQueryable targetQueryable)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> SplitTable(dynamic splitValue)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> SplitTable(dynamic startSplitValue, dynamic endSplitValue)
        {
            SplitTableCore(startSplitValue, endSplitValue);
            return this;
        }

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitValues">Split values</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> SplitTable(IEnumerable<dynamic> splitValues, SplitTableNameSelectionPattern selectionPattern = SplitTableNameSelectionPattern.Precision)
        {
            base.SplitTable(splitValues, selectionPattern);
            return this;
        }

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitTableNameFilter">Split table name filter</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> SplitTable(Func<IEnumerable<string>, IEnumerable<string>> splitTableNameFilter)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Output(QueryableOutputType outputType)
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> IncludeArchived()
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Unisolated()
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
        public new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Negate()
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
        public List<TReturn> ToList<TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return ToList<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(dataMappingFunc, configure);
        }

        #endregion

        #endregion
    }
}
