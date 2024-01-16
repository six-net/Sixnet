using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Field;
using Sixnet.Expressions.Linq;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Default queryable two
    /// </summary>
    internal partial class DefaultQueryableTwo<TFirst, TSecond> : DefaultQueryableOne<TFirst>, ISixnetQueryable<TFirst, TSecond>
    {
        #region Constructor

        public DefaultQueryableTwo(ISixnetQueryable sourceQueryable) : base(sourceQueryable) { }

        public DefaultQueryableTwo(QueryableContext sourceQueryableContext) : base(sourceQueryableContext) { }

        #endregion

        #region Condition

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="condition">Condition</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond> Where(ICondition condition)
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
        public new ISixnetQueryable<TFirst, TSecond> WhereIf(bool predicate, ICondition condition)
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
        public new ISixnetQueryable<TFirst, TSecond> Where(Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond> WhereIf(bool predicate, Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public ISixnetQueryable<TFirst, TSecond> Where(Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public ISixnetQueryable<TFirst, TSecond> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond> OrderBy(IEnumerable<string> fieldNames, bool desc = false, Type targetType = null)
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
        public new ISixnetQueryable<TFirst, TSecond> OrderBy(string fieldName, bool desc = false, Type targetType = null)
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
        public new ISixnetQueryable<TFirst, TSecond> OrderBy(IDataField field, bool desc = false)
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
        public new ISixnetQueryable<TFirst, TSecond> OrderBy(IEnumerable<IDataField> fields, bool desc = false)
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
        public new ISixnetQueryable<TFirst, TSecond> OrderByIf(bool predicate, IEnumerable<string> fieldNames, bool desc = false, Type targetType = null)
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
        public new ISixnetQueryable<TFirst, TSecond> OrderByIf(bool predicate, string fieldName, bool desc = false, Type targetType = null)
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
        public new ISixnetQueryable<TFirst, TSecond> OrderByIf(bool predicate, IDataField field, bool desc = false)
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
        public new ISixnetQueryable<TFirst, TSecond> OrderByIf(bool predicate, IEnumerable<IDataField> fields, bool desc = false)
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
        public new ISixnetQueryable<TFirst, TSecond> OrderBy(Expression<Func<TFirst, object>> field, bool desc = false)
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
        public new ISixnetQueryable<TFirst, TSecond> OrderByIf(bool predicate, Expression<Func<TFirst, object>> field, bool desc = false)
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
        public ISixnetQueryable<TFirst, TSecond> OrderBy(Expression<Func<TSecond, object>> field, bool desc = false)
        {
            OrderByExpressionField<TSecond>(field, desc);
            return this;
        }

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond> OrderByIf(bool predicate, Expression<Func<TSecond, object>> field, bool desc = false)
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
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird> InnerJoin<TThird>(Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null)
        {
            return InnerJoin(null, joinExpression);
        }

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="thirdQueryable">Third queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird> InnerJoin<TThird>(ISixnetQueryable<TThird> thirdQueryable, Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null)
        {
            return Join(true, JoinType.InnerJoin, thirdQueryable, joinExpression);
        }

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird> InnerJoinIf<TThird>(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null)
        {
            return InnerJoinIf(predicate, null, joinExpression);
        }

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="thirdQueryable">Third queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird> InnerJoinIf<TThird>(bool predicate, ISixnetQueryable<TThird> thirdQueryable, Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null)
        {
            return Join(predicate, JoinType.InnerJoin, thirdQueryable, joinExpression);
        }

        #endregion

        #region Left join

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird> LeftJoin<TThird>(Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null)
        {
            return LeftJoin(null, joinExpression);
        }

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="thirdQueryable">Third queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird> LeftJoin<TThird>(ISixnetQueryable<TThird> thirdQueryable, Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null)
        {
            return Join(true, JoinType.LeftJoin, thirdQueryable, joinExpression);
        }

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird> LeftJoinIf<TThird>(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null)
        {
            return LeftJoinIf(predicate, null, joinExpression);
        }

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="thirdQueryable">Third queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird> LeftJoinIf<TThird>(bool predicate, ISixnetQueryable<TThird> thirdQueryable, Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null)
        {
            return Join(predicate, JoinType.LeftJoin, thirdQueryable, joinExpression);
        }

        #endregion

        #region Right join

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird> RightJoin<TThird>(Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null)
        {
            return RightJoin(null, joinExpression);
        }

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="thirdQueryable">Third queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird> RightJoin<TThird>(ISixnetQueryable<TThird> thirdQueryable, Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null)
        {
            return Join(true, JoinType.RightJoin, thirdQueryable, joinExpression);
        }

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird> RightJoinIf<TThird>(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null)
        {
            return RightJoinIf(predicate, null, joinExpression);
        }

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="thirdQueryable">Third queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird> RightJoinIf<TThird>(bool predicate, ISixnetQueryable<TThird> thirdQueryable, Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null)
        {
            return Join(predicate, JoinType.RightJoin, thirdQueryable, joinExpression);
        }

        #endregion

        #region Full join

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird> FullJoin<TThird>(Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null)
        {
            return FullJoin(null, joinExpression);
        }

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="thirdQueryable">Third queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird> FullJoin<TThird>(ISixnetQueryable<TThird> thirdQueryable, Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null)
        {
            return Join(true, JoinType.FullJoin, thirdQueryable, joinExpression);
        }

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird> FullJoinIf<TThird>(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null)
        {
            return FullJoinIf(predicate, null, joinExpression);
        }

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="thirdQueryable">Third queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond, TThird> FullJoinIf<TThird>(bool predicate, ISixnetQueryable<TThird> thirdQueryable, Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null)
        {
            return Join(predicate, JoinType.FullJoin, thirdQueryable, joinExpression);
        }

        #endregion

        #region Cross join

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird> CrossJoin<TThird>()
        {
            return CrossJoin<TThird>(null);
        }

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="thirdQueryable">Third queryable</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird> CrossJoin<TThird>(ISixnetQueryable<TThird> thirdQueryable)
        {
            return Join(true, JoinType.CrossJoin, thirdQueryable);
        }

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird> CrossJoinIf<TThird>(bool predicate)
        {
            return CrossJoinIf<TThird>(predicate);
        }

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="thirdQueryable">Third queryable</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond, TThird> CrossJoinIf<TThird>(bool predicate, ISixnetQueryable<TThird> thirdQueryable)
        {
            return Join(predicate, JoinType.CrossJoin, thirdQueryable);
        }

        #endregion

        #region Add join

        /// <summary>
        /// Add join
        /// </summary>
        /// <param name="joinEntries">Join entries</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond> Join(params JoinEntry[] joinEntries)
        {
            base.Join(joinEntries);
            return this;
        }

        #endregion

        #region Join core

        ISixnetQueryable<TFirst, TSecond, TThird> Join<TThird>(bool predicate, JoinType joinType, ISixnetQueryable<TThird> thirdQueryable, Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null)
        {
            if (predicate)
            {
                var joinQueryable = ExpressionHelper.GetQueryable(joinExpression, CriterionConnector.And);
                var targetQueryable = SixnetQueryable.Create<TThird>();
                if (thirdQueryable != null)
                {
                    targetQueryable.From(thirdQueryable);
                }
                queryableContext.Join(new JoinEntry()
                {
                    TargetQueryable = targetQueryable,
                    Type = joinType,
                    Connection = joinQueryable
                });
            }
            return SixnetQueryable.Create<TFirst, TSecond, TThird>(this);
        }

        #endregion

        #endregion

        #region Select

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond> Select(params IDataField[] fields)
        {
            base.Select(fields);
            return this;
        }

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond> Select(params string[] fieldNames)
        {
            base.Select(fieldNames);
            return this;
        }

        /// <summary>
        /// Exclude fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond> Unselect(params IDataField[] fields)
        {
            base.Unselect(fields);
            return this;
        }

        /// <summary>
        /// Exclude fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond> Unselect(params string[] fieldNames)
        {
            base.Unselect(fieldNames);
            return this;
        }

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond> Select(params Expression<Func<TFirst, object>>[] fields)
        {
            IncludeExpressionFieldsCore(fields);
            return this;
        }

        /// <summary>
        /// Exclude fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond> Unselect(params Expression<Func<TFirst, object>>[] fields)
        {
            ExcludeExpressionFieldsCore(fields);
            return this;
        }

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond> Select(params Expression<Func<TSecond, object>>[] fields)
        {
            IncludeExpressionFieldsCore(fields);
            return this;
        }

        /// <summary>
        /// Exclude fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond> Unselect(params Expression<Func<TSecond, object>>[] fields)
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
        public new ISixnetQueryable<TFirst, TSecond> TreeMatching(string dataFieldName, string parentFieldName, TreeMatchingDirection direction = TreeMatchingDirection.Down)
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
        public new ISixnetQueryable<TFirst, TSecond> TreeMatching(IDataField dataField, IDataField parentField, TreeMatchingDirection direction = TreeMatchingDirection.Down)
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
        public new ISixnetQueryable<TFirst, TSecond> TreeMatching(Expression<Func<TFirst, object>> dataField, Expression<Func<TFirst, object>> parentField, TreeMatchingDirection direction = TreeMatchingDirection.Down)
        {
            TreeMatching(ExpressionHelper.GetDataField(dataField), ExpressionHelper.GetDataField(parentField), direction);
            return this;
        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns>Return the replicated Queryable</returns>
        public new ISixnetQueryable<TFirst, TSecond> LightClone()
        {
            return LightCloneCore() as ISixnetQueryable<TFirst, TSecond>;
        }

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns>Return the replicated Queryable</returns>
        public new ISixnetQueryable<TFirst, TSecond> Clone()
        {
            return CloneCore() as ISixnetQueryable<TFirst, TSecond>;
        }

        protected override ISixnetQueryable LightCloneCore()
        {
            return new DefaultQueryableTwo<TFirst, TSecond>(queryableContext?.LightClone());
        }

        protected override ISixnetQueryable CloneCore()
        {
            return new DefaultQueryableTwo<TFirst, TSecond>(queryableContext?.Clone());
        }

        #endregion

        #region Combine

        #region UnionAll

        /// <summary>
        /// Union all
        /// </summary>
        /// <param name="unionQueryable">Union queryable</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond> UnionAll(ISixnetQueryable unionQueryable)
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
        public new ISixnetQueryable<TFirst, TSecond> UnionAll<TTarget>(Expression<Func<TTarget, bool>> unionExpression = null)
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
        public new ISixnetQueryable<TFirst, TSecond> Union(ISixnetQueryable unionQueryable)
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
        public new ISixnetQueryable<TFirst, TSecond> Union<TTarget>(Expression<Func<TTarget, bool>> unionExpression = null)
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
        public new ISixnetQueryable<TFirst, TSecond> Except(ISixnetQueryable exceptQueryable)
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
        public new ISixnetQueryable<TFirst, TSecond> Except<TTarget>(Expression<Func<TTarget, bool>> exceptExpression = null)
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
        public new ISixnetQueryable<TFirst, TSecond> Intersect(ISixnetQueryable intersectQueryable)
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
        public new ISixnetQueryable<TFirst, TSecond> Intersect<TTarget>(Expression<Func<TTarget, bool>> intersectExpression = null)
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
        public new ISixnetQueryable<TFirst, TSecond> SetModelType(Type modelType)
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
        public new ISixnetQueryable<TFirst, TSecond> Take(int count, int skip = 0)
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
        public new ISixnetQueryable<TFirst, TSecond> Distinct()
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
        public new ISixnetQueryable<TFirst, TSecond> GroupBy(params string[] fieldNames)
        {
            base.GroupBy(fieldNames);
            return this;
        }

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond> GroupBy(params IDataField[] fields)
        {
            base.GroupBy(fields);
            return this;
        }

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond> GroupBy(params Expression<Func<TFirst, object>>[] fields)
        {
            GroupByExpression(fields);
            return this;
        }

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond> GroupBy(params Expression<Func<TSecond, object>>[] fields)
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
        public new ISixnetQueryable<TFirst, TSecond> Having(Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond> HavingIf(bool predicate, Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public ISixnetQueryable<TFirst, TSecond> Having(Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public ISixnetQueryable<TFirst, TSecond> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst, TSecond> From(ISixnetQueryable targetQueryable)
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
        public new ISixnetQueryable<TFirst, TSecond> SplitTable(dynamic splitValue)
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
        public new ISixnetQueryable<TFirst, TSecond> SplitTable(dynamic startSplitValue, dynamic endSplitValue)
        {
            SplitTableCore(startSplitValue, endSplitValue);
            return this;
        }

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitValues">Split values</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond> SplitTable(IEnumerable<dynamic> splitValues, SplitTableNameSelectionPattern selectionPattern = SplitTableNameSelectionPattern.Precision)
        {
            base.SplitTable(splitValues, selectionPattern);
            return this;
        }

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitTableNameFilter">Split table name filter</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond> SplitTable(Func<IEnumerable<string>, IEnumerable<string>> splitTableNameFilter)
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
        public new ISixnetQueryable<TFirst, TSecond> Output(QueryableOutputType outputType)
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
        public new ISixnetQueryable<TFirst, TSecond> IncludeArchived()
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
        public new ISixnetQueryable<TFirst, TSecond> Unisolated()
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
        public new ISixnetQueryable<TFirst, TSecond> Negate()
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
        public List<TReturn> ToList<TReturn>(Func<TFirst, TSecond, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return ToList<TFirst, TSecond, TReturn>(dataMappingFunc, configure);
        }

        #endregion

        #endregion
    }
}
