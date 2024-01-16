using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Field;
using Sixnet.Expressions.Linq;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Default queryable one
    /// </summary>
    internal class DefaultQueryableOne<TFirst> : DefaultModelQueryable<TFirst>, ISixnetQueryable<TFirst>
    {
        #region Constructor

        public DefaultQueryableOne(ISixnetQueryable sourceQueryable = null) : base(sourceQueryable) { }

        public DefaultQueryableOne(QueryableContext sourceQueryableContext = null) : base(sourceQueryableContext) { }

        #endregion

        #region Condition

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="condition">Condition</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst> Where(ICondition condition)
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
        public new ISixnetQueryable<TFirst> WhereIf(bool predicate, ICondition condition)
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
        public ISixnetQueryable<TFirst> Where(Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public ISixnetQueryable<TFirst> WhereIf(bool predicate, Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst> OrderBy(IEnumerable<string> fieldNames, bool desc = false, Type targetType = null)
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
        public new ISixnetQueryable<TFirst> OrderBy(string fieldName, bool desc = false, Type targetType = null)
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
        public new ISixnetQueryable<TFirst> OrderBy(IDataField field, bool desc = false)
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
        public new ISixnetQueryable<TFirst> OrderBy(IEnumerable<IDataField> fields, bool desc = false)
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
        public new ISixnetQueryable<TFirst> OrderByIf(bool predicate, IEnumerable<string> fieldNames, bool desc = false, Type targetType = null)
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
        public new ISixnetQueryable<TFirst> OrderByIf(bool predicate, string fieldName, bool desc = false, Type targetType = null)
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
        public new ISixnetQueryable<TFirst> OrderByIf(bool predicate, IDataField field, bool desc = false)
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
        public new ISixnetQueryable<TFirst> OrderByIf(bool predicate, IEnumerable<IDataField> fields, bool desc = false)
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
        public ISixnetQueryable<TFirst> OrderBy(Expression<Func<TFirst, object>> field, bool desc = false)
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
        public ISixnetQueryable<TFirst> OrderByIf(bool predicate, Expression<Func<TFirst, object>> field, bool desc = false)
        {
            return predicate
                ? OrderBy(field, desc)
                : this;
        }

        protected void OrderByExpressionField<T>(Expression<Func<T, object>> field, bool desc = false)
        {
            var expressionField = ExpressionHelper.GetDataField(field);
            OrderBy(expressionField, desc);
        }

        #endregion

        #region Join

        #region Inner join

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond> InnerJoin<TSecond>(Expression<Func<TFirst, TSecond, bool>> joinExpression = null)
        {
            return InnerJoin(null, joinExpression);
        }

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="secondQueryable">Second queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond> InnerJoin<TSecond>(ISixnetQueryable<TSecond> secondQueryable, Expression<Func<TFirst, TSecond, bool>> joinExpression = null)
        {
            return Join(true, JoinType.InnerJoin, secondQueryable, joinExpression);
        }

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond> InnerJoinIf<TSecond>(bool predicate, Expression<Func<TFirst, TSecond, bool>> joinExpression = null)
        {
            return InnerJoinIf(predicate, null, joinExpression);
        }

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="secondQueryable">Second queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond> InnerJoinIf<TSecond>(bool predicate, ISixnetQueryable<TSecond> secondQueryable, Expression<Func<TFirst, TSecond, bool>> joinExpression = null)
        {
            return Join(predicate, JoinType.InnerJoin, secondQueryable, joinExpression);
        }

        #endregion

        #region Left join

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond> LeftJoin<TSecond>(Expression<Func<TFirst, TSecond, bool>> joinExpression = null)
        {
            return LeftJoin(null, joinExpression);
        }

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="secondQueryable">Second queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond> LeftJoin<TSecond>(ISixnetQueryable<TSecond> secondQueryable, Expression<Func<TFirst, TSecond, bool>> joinExpression = null)
        {
            return Join(true, JoinType.LeftJoin, secondQueryable, joinExpression);
        }

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond> LeftJoinIf<TSecond>(bool predicate, Expression<Func<TFirst, TSecond, bool>> joinExpression = null)
        {
            return LeftJoinIf(predicate, null, joinExpression);
        }

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="secondQueryable">Second queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond> LeftJoinIf<TSecond>(bool predicate, ISixnetQueryable<TSecond> secondQueryable, Expression<Func<TFirst, TSecond, bool>> joinExpression = null)
        {
            return Join(predicate, JoinType.LeftJoin, secondQueryable, joinExpression);
        }

        #endregion

        #region Right join

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond> RightJoin<TSecond>(Expression<Func<TFirst, TSecond, bool>> joinExpression = null)
        {
            return RightJoin(null, joinExpression);
        }

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="secondQueryable">Second queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond> RightJoin<TSecond>(ISixnetQueryable<TSecond> secondQueryable, Expression<Func<TFirst, TSecond, bool>> joinExpression = null)
        {
            return Join(true, JoinType.RightJoin, secondQueryable, joinExpression);
        }

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond> RightJoinIf<TSecond>(bool predicate, Expression<Func<TFirst, TSecond, bool>> joinExpression = null)
        {
            return RightJoinIf(predicate, null, joinExpression);
        }

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="secondQueryable">Second queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond> RightJoinIf<TSecond>(bool predicate, ISixnetQueryable<TSecond> secondQueryable, Expression<Func<TFirst, TSecond, bool>> joinExpression = null)
        {
            return Join(predicate, JoinType.RightJoin, secondQueryable, joinExpression);
        }

        #endregion

        #region Full join

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond> FullJoin<TSecond>(Expression<Func<TFirst, TSecond, bool>> joinExpression = null)
        {
            return FullJoin(null, joinExpression);
        }

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="secondQueryable">Second queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond> FullJoin<TSecond>(ISixnetQueryable<TSecond> secondQueryable, Expression<Func<TFirst, TSecond, bool>> joinExpression = null)
        {
            return Join(true, JoinType.FullJoin, secondQueryable, joinExpression);
        }

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond> FullJoinIf<TSecond>(bool predicate, Expression<Func<TFirst, TSecond, bool>> joinExpression = null)
        {
            return FullJoinIf(predicate, null, joinExpression);
        }

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="secondQueryable">Second queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst, TSecond> FullJoinIf<TSecond>(bool predicate, ISixnetQueryable<TSecond> secondQueryable, Expression<Func<TFirst, TSecond, bool>> joinExpression = null)
        {
            return Join(predicate, JoinType.FullJoin, secondQueryable, joinExpression);
        }

        #endregion

        #region Cross join

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond> CrossJoin<TSecond>()
        {
            return CrossJoin<TSecond>(null);
        }

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="secondQueryable">Second queryable</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond> CrossJoin<TSecond>(ISixnetQueryable<TSecond> secondQueryable)
        {
            return Join(true, JoinType.CrossJoin, secondQueryable);
        }

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond> CrossJoinIf<TSecond>(bool predicate)
        {
            return CrossJoinIf<TSecond>(true, null);
        }

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="secondQueryable">Second queryable</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst, TSecond> CrossJoinIf<TSecond>(bool predicate, ISixnetQueryable<TSecond> secondQueryable)
        {
            return Join(predicate, JoinType.CrossJoin, secondQueryable);
        }

        #endregion

        #region Add join

        /// <summary>
        /// Add join
        /// </summary>
        /// <param name="joinEntries">Join entries</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst> Join(params JoinEntry[] joinEntries)
        {
            base.Join(joinEntries);
            return this;
        }

        #endregion

        #region Join core

        ISixnetQueryable<TFirst, TSecond> Join<TSecond>(bool predicate, JoinType joinType, ISixnetQueryable<TSecond> secondQueryable, Expression<Func<TFirst, TSecond, bool>> joinExpression = null)
        {
            if (predicate)
            {
                var joinQueryable = ExpressionHelper.GetQueryable(joinExpression, CriterionConnector.And);
                var targetQueryable = SixnetQueryable.Create<TSecond>();
                if (secondQueryable != null)
                {
                    targetQueryable.From(secondQueryable);
                }
                queryableContext.Join(new JoinEntry()
                {
                    TargetQueryable = targetQueryable,
                    Type = joinType,
                    Connection = joinQueryable
                });
            }
            return SixnetQueryable.Create<TFirst, TSecond>(this);
        }

        #endregion

        #endregion

        #region Select

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst> Select(params IDataField[] fields)
        {
            base.Select(fields);
            return this;
        }

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst> Select(params string[] fieldNames)
        {
            base.Select(fieldNames);
            return this;
        }

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst> Unselect(params IDataField[] fields)
        {
            base.Unselect(fields);
            return this;
        }

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst> Unselect(params string[] fieldNames)
        {
            base.Unselect(fieldNames);
            return this;
        }

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> Select(params Expression<Func<TFirst, object>>[] fields)
        {
            IncludeExpressionFieldsCore(fields);
            return this;
        }

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> Unselect(params Expression<Func<TFirst, object>>[] fields)
        {
            ExcludeExpressionFieldsCore(fields);
            return this;
        }

        protected void IncludeExpressionFieldsCore<T>(params Expression<Func<T, object>>[] fields)
        {
            Select(fields?.Select(f => ExpressionHelper.GetOutputDataField(this, f)).ToArray());
        }

        protected void ExcludeExpressionFieldsCore<T>(params Expression<Func<T, object>>[] fields)
        {
            Unselect(fields?.Select(f => ExpressionHelper.GetOutputDataField(this, f)).ToArray());
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
        public new ISixnetQueryable<TFirst> TreeMatching(string dataFieldName, string parentFieldName, TreeMatchingDirection direction = TreeMatchingDirection.Down)
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
        public new ISixnetQueryable<TFirst> TreeMatching(IDataField dataField, IDataField parentField, TreeMatchingDirection direction = TreeMatchingDirection.Down)
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
        public ISixnetQueryable<TFirst> TreeMatching(Expression<Func<TFirst, object>> dataField, Expression<Func<TFirst, object>> parentField, TreeMatchingDirection direction = TreeMatchingDirection.Down)
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
        public new ISixnetQueryable<TFirst> LightClone()
        {
            return LightCloneCore() as ISixnetQueryable<TFirst>;
        }

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns>Return the replicated Queryable</returns>
        public new ISixnetQueryable<TFirst> Clone()
        {
            return CloneCore() as ISixnetQueryable<TFirst>;
        }

        protected override ISixnetQueryable LightCloneCore()
        {
            return new DefaultQueryableOne<TFirst>(queryableContext?.LightClone());
        }

        protected override ISixnetQueryable CloneCore()
        {
            return new DefaultQueryableOne<TFirst>(queryableContext?.Clone());
        }

        #endregion

        #region Combine

        #region UnionAll

        /// <summary>
        /// Union all
        /// </summary>
        /// <param name="unionQueryable">Union queryable</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst> UnionAll(ISixnetQueryable unionQueryable)
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
        public new ISixnetQueryable<TFirst> UnionAll<TTarget>(Expression<Func<TTarget, bool>> unionExpression = null)
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
        public new ISixnetQueryable<TFirst> Union(ISixnetQueryable unionQueryable)
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
        public new ISixnetQueryable<TFirst> Union<TTarget>(Expression<Func<TTarget, bool>> unionExpression = null)
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
        public new ISixnetQueryable<TFirst> Except(ISixnetQueryable exceptQueryable)
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
        public new ISixnetQueryable<TFirst> Except<TTarget>(Expression<Func<TTarget, bool>> exceptExpression = null)
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
        public new ISixnetQueryable<TFirst> Intersect(ISixnetQueryable intersectQueryable)
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
        public new ISixnetQueryable<TFirst> Intersect<TTarget>(Expression<Func<TTarget, bool>> intersectExpression = null)
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
        public new ISixnetQueryable<TFirst> SetModelType(Type modelType)
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
        public new ISixnetQueryable<TFirst> Take(int count, int skip = 0)
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
        public new ISixnetQueryable<TFirst> Distinct()
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
        public new ISixnetQueryable<TFirst> GroupBy(params string[] fieldNames)
        {
            base.GroupBy(fieldNames);
            return this;
        }

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst> GroupBy(params IDataField[] fields)
        {
            base.GroupBy(fields);
            return this;
        }

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> GroupBy(params Expression<Func<TFirst, object>>[] fields)
        {
            GroupByExpression(fields);
            return this;
        }

        protected void GroupByExpression<T>(params Expression<Func<T, object>>[] fields)
        {
            GroupBy(fields?.Select(f => ExpressionHelper.GetDataField(f.Body)).ToArray());
        }

        #endregion

        #region Having

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> Having(Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public ISixnetQueryable<TFirst> HavingIf(bool predicate, Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And)
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
        public new ISixnetQueryable<TFirst> From(ISixnetQueryable targetQueryable)
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
        public new ISixnetQueryable<TFirst> SplitTable(dynamic splitValue)
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
        public new ISixnetQueryable<TFirst> SplitTable(dynamic startSplitValue, dynamic endSplitValue)
        {
            SplitTableCore(startSplitValue, endSplitValue);
            return this;
        }

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitValues">Split values</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst> SplitTable(IEnumerable<dynamic> splitValues, SplitTableNameSelectionPattern selectionPattern = SplitTableNameSelectionPattern.Precision)
        {
            base.SplitTable(splitValues, selectionPattern);
            return this;
        }

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitTableNameFilter">Split table name filter</param>
        /// <returns></returns>
        public new ISixnetQueryable<TFirst> SplitTable(Func<IEnumerable<string>, IEnumerable<string>> splitTableNameFilter)
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
        public new ISixnetQueryable<TFirst> Output(QueryableOutputType outputType)
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
        public new ISixnetQueryable<TFirst> IncludeArchived()
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
        public new ISixnetQueryable<TFirst> Unisolated()
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
        public new ISixnetQueryable<TFirst> Negate()
        {
            base.Negate();
            return this;
        }

        #endregion
    }
}
