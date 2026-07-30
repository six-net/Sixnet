// "Company © 2025. All rights reserved."

using Sixnet.Development.Data;
using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Database;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Entity;
using Sixnet.Development.Repository;
using Sixnet.Expressions.Linq;
using Sixnet.Model.Paging;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Default queryable
    /// </summary>
    [Serializable]
    internal partial class SixnetDefaultQueryable : ISixnetQueryable
    {
        #region Fields

        protected SixnetQueryableInfo queryableInfo = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the queryable info
        /// </summary>
        public SixnetQueryableInfo Info => queryableInfo;

        /// <summary>
        /// Gets the connector
        /// </summary>
        public SixnetCriterionConnector Connector
        {
            get
            {
                return queryableInfo.Connector;
            }
            set
            {
                queryableInfo.Connector = value;
            }
        }

        /// <summary>
        /// Indicates there is no conditions
        /// </summary>
        public bool None => queryableInfo.NoneCondition;

        /// <summary>
        /// Whether negation
        /// </summary>
        public bool Negation => queryableInfo.Negation;

        #endregion

        #region Constructor

        public SixnetDefaultQueryable(SixnetQueryableInfo sourceQueryableContext)
        {
            queryableInfo = sourceQueryableContext ?? new SixnetQueryableInfo();
            InitQueryable(queryableInfo);
        }

        public SixnetDefaultQueryable(ISixnetQueryable sixnetQueryable) : this((sixnetQueryable as SixnetDefaultQueryable)?.queryableInfo)
        {
        }

        void InitQueryable(SixnetQueryableInfo queryableContext)
        {
            // Init model type
            var currentModelType = queryableContext.ModelType;
            var thisType = GetType();
            if (currentModelType == null && thisType.IsGenericType)
            {
                queryableContext.SetModelType(thisType.GenericTypeArguments[0]);
            }
        }

        #endregion

        #region Methods

        #region Condition

        /// <summary>
        /// Append group condition
        /// </summary>
        /// <param name="condition">Group condition</param>
        /// <returns></returns>
        public ISixnetQueryable Where(ISixnetCondition condition)
        {
            queryableInfo.AddCondition(condition);
            return this;
        }

        /// <summary>
        /// Append group condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="condition">Group condition</param>
        /// <returns></returns>
        public ISixnetQueryable WhereIf(bool predicate, ISixnetCondition condition)
        {
            if (predicate)
            {
                queryableInfo.AddCondition(condition);
            }
            return this;
        }

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        protected void WhereExpressionCore(Expression expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And)
        {
            if (expression != null)
            {
                var expressionCondition = SixnetExpressionHelper.GetQueryable(expression, connector);
                queryableInfo.AddCondition(expressionCondition);
            }
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
        public ISixnetQueryable OrderBy(IEnumerable<string> fieldNames, bool desc = false, Type targetType = null)
        {
            if (!fieldNames.IsNullOrEmpty())
            {
                foreach (var field in fieldNames)
                {
                    queryableInfo.AddSort(SixnetSortEntry.Create(field, desc, targetType));
                }
            }
            return this;
        }

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        public ISixnetQueryable OrderBy(string fieldName, bool desc = false, Type targetType = null)
        {
            if (!string.IsNullOrWhiteSpace(fieldName))
            {
                queryableInfo.AddSort(SixnetSortEntry.Create(fieldName, desc, targetType));
            }
            return this;
        }

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        public ISixnetQueryable OrderBy(ISixnetField field, bool desc = false)
        {
            queryableInfo.AddSort(new SixnetSortEntry()
            {
                Desc = desc,
                Field = field
            });
            return this;
        }

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        public ISixnetQueryable OrderBy(IEnumerable<ISixnetField> fields, bool desc = false)
        {
            if (!fields.IsNullOrEmpty())
            {
                foreach (var field in fields)
                {
                    OrderBy(field, desc);
                }
            }
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
        public ISixnetQueryable OrderByIf(bool predicate, IEnumerable<string> fieldNames, bool desc = false, Type targetType = null)
        {
            if (predicate && !fieldNames.IsNullOrEmpty())
            {
                foreach (var field in fieldNames)
                {
                    queryableInfo.AddSort(SixnetSortEntry.Create(field, desc, targetType));
                }
            }
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
        public ISixnetQueryable OrderByIf(bool predicate, string fieldName, bool desc = false, Type targetType = null)
        {
            if (predicate && !string.IsNullOrWhiteSpace(fieldName))
            {
                queryableInfo.AddSort(SixnetSortEntry.Create(fieldName, desc, targetType));
            }
            return this;
        }

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        public ISixnetQueryable OrderByIf(bool predicate, ISixnetField field, bool desc = false)
        {
            if (predicate)
            {
                OrderBy(field, desc);
            }
            return this;
        }

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        public ISixnetQueryable OrderByIf(bool predicate, IEnumerable<ISixnetField> fields, bool desc = false)
        {
            if (predicate)
            {
                OrderBy(fields, desc);
            }
            return this;
        }

        /// <summary>
        /// Clear sort condition
        /// </summary>
        /// <returns></returns>
        public ISixnetQueryable ClearSort()
        {
            queryableInfo.ClearSort();
            return this;
        }

        #endregion

        #region Select

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public ISixnetQueryable Select(params ISixnetField[] fields)
        {
            if (!fields.IsNullOrEmpty())
            {
                queryableInfo.Select(fields);
            }
            return this;
        }

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        public ISixnetQueryable Select(params string[] fieldNames)
        {
            return Select(fieldNames?.Select(f => SixnetDataField.Create(f)).ToArray());
        }

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public ISixnetQueryable Unselect(params ISixnetField[] fields)
        {
            if (!fields.IsNullOrEmpty())
            {
                queryableInfo.UnselectFields(fields);
            }
            return this;
        }

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        public ISixnetQueryable Unselect(params string[] fieldNames)
        {
            return Unselect(fieldNames?.Select(f => SixnetDataField.Create(f)).ToArray());
        }

        /// <summary>
        /// Get the selected fields
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <param name="includeNecessary">Whether include the necessary fields</param>
        /// <returns></returns>
        public IEnumerable<ISixnetField> GetSelectedFields(Type modelType, bool includeNecessary)
        {
            return queryableInfo.GetFinallyFields(modelType, includeNecessary);
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
        public ISixnetQueryable Tree(string dataFieldName, string parentFieldName, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down)
        {
            queryableInfo.TreeMatching(dataFieldName, parentFieldName, direction);
            return this;
        }

        /// <summary>
        /// Tree matching
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        public ISixnetQueryable Tree(ISixnetField dataField, ISixnetField parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down)
        {
            queryableInfo.TreeMatching(dataField, parentField, direction);
            return this;
        }

        #endregion

        #region Clone

        /// <summary>
        /// Light clone an IQuery object
        /// </summary>
        /// <returns>Return the replicated Queryable</returns>
        public ISixnetQueryable LightClone()
        {
            return LightCloneCore();
        }

        /// <summary>
        /// Clone a IQuery object
        /// </summary>
        /// <returns>Return the replicated Queryable</returns>
        public ISixnetQueryable Clone()
        {
            return CloneCore();
        }

        protected virtual ISixnetQueryable LightCloneCore()
        {
            return new SixnetDefaultQueryable(queryableInfo?.LightClone());
        }

        protected virtual ISixnetQueryable CloneCore()
        {
            return new SixnetDefaultQueryable(queryableInfo?.Clone());
        }

        #endregion

        #region Script

        /// <summary>
        /// Set script
        /// </summary>
        /// <param name="script">Script</param>
        /// <param name="scriptType">Script type</param>
        /// <param name="parameters">Parameters</param>
        /// <returns></returns>
        public ISixnetQueryable SetScript(string script, SixnetDataScriptType scriptType = SixnetDataScriptType.Text, object parameters = null)
        {
            queryableInfo.SetScript(script, scriptType, parameters);
            return this;
        }

        #endregion

        #region Validation

        /// <summary>
        /// Get validation function
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <returns>Return a validation function</returns>
        public Func<T, bool> GetValidationFunction<T>()
        {
            return queryableInfo.GetValidationFunction<T>();
        }

        #endregion

        #region Join

        #region Inner join

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> InnerJoin<TFirst>(Expression<Func<TFirst, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            return InnerJoin(null, connection, configure);
        }

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="firstQueryable">First queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> InnerJoin<TFirst>(ISixnetQueryable firstQueryable, Expression<Func<TFirst, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            return Join(true, SixnetJoinType.InnerJoin, firstQueryable, connection, configure);
        }

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> InnerJoinIf<TFirst>(bool predicate, Expression<Func<TFirst, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            return InnerJoinIf(predicate, null, connection, configure);
        }

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="firstQueryable">First queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> InnerJoinIf<TFirst>(bool predicate, ISixnetQueryable firstQueryable, Expression<Func<TFirst, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            return Join(predicate, SixnetJoinType.InnerJoin, firstQueryable, connection, configure);
        }

        #endregion

        #region Left join

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> LeftJoin<TFirst>(Expression<Func<TFirst, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            return LeftJoin(null, connection, configure);
        }

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="firstQueryable">First queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> LeftJoin<TFirst>(ISixnetQueryable firstQueryable, Expression<Func<TFirst, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            return Join(true, SixnetJoinType.LeftJoin, firstQueryable, connection, configure);
        }

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> LeftJoinIf<TFirst>(bool predicate, Expression<Func<TFirst, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            return LeftJoinIf(predicate, null, connection, configure);
        }

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="firstQueryable">First queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> LeftJoinIf<TFirst>(bool predicate, ISixnetQueryable firstQueryable, Expression<Func<TFirst, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            return Join(predicate, SixnetJoinType.LeftJoin, firstQueryable, connection, configure);
        }

        #endregion

        #region Right join

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> RightJoin<TFirst>(Expression<Func<TFirst, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            return RightJoin(null, connection, configure);
        }

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="firstQueryable">First queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> RightJoin<TFirst>(ISixnetQueryable firstQueryable, Expression<Func<TFirst, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            return Join(true, SixnetJoinType.RightJoin, firstQueryable, connection, configure);
        }

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> RightJoinIf<TFirst>(bool predicate, Expression<Func<TFirst, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            return RightJoinIf(predicate, null, connection, configure);
        }

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="firstQueryable">First queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> RightJoinIf<TFirst>(bool predicate, ISixnetQueryable firstQueryable, Expression<Func<TFirst, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            return Join(predicate, SixnetJoinType.RightJoin, firstQueryable, connection, configure);
        }

        #endregion

        #region Full join

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> FullJoin<TFirst>(Expression<Func<TFirst, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            return FullJoin(null, connection, configure);
        }

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="firstQueryable">First queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> FullJoin<TFirst>(ISixnetQueryable firstQueryable, Expression<Func<TFirst, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            return Join(true, SixnetJoinType.FullJoin, firstQueryable, connection, configure);
        }

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> FullJoinIf<TFirst>(bool predicate, Expression<Func<TFirst, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            return FullJoinIf(predicate, null, connection, configure);
        }

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="firstQueryable">First queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> FullJoinIf<TFirst>(bool predicate, ISixnetQueryable firstQueryable, Expression<Func<TFirst, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            return Join(predicate, SixnetJoinType.FullJoin, firstQueryable, connection, configure);
        }

        #endregion

        #region Cross join

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> CrossJoin<TFirst>(Action<SixnetJoinEntry> configure = null)
        {
            return CrossJoin<TFirst>(null, configure);
        }

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="firstQueryable">First queryable</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> CrossJoin<TFirst>(ISixnetQueryable firstQueryable, Action<SixnetJoinEntry> configure = null)
        {
            return Join<TFirst>(true, SixnetJoinType.CrossJoin, firstQueryable, null, configure);
        }

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> CrossJoinIf<TFirst>(bool predicate, Action<SixnetJoinEntry> configure = null)
        {
            return CrossJoinIf<TFirst>(predicate, null, configure);
        }

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="firstQueryable">First queryable</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> CrossJoinIf<TFirst>(bool predicate, ISixnetQueryable firstQueryable, Action<SixnetJoinEntry> configure = null)
        {
            return Join<TFirst>(predicate, SixnetJoinType.CrossJoin, firstQueryable, null, configure);
        }

        #endregion

        #region Add join

        /// <summary>
        /// Add join
        /// </summary>
        /// <param name="joinEntry">Join entry</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        public ISixnetQueryable Join(SixnetJoinEntry joinEntry, Action<SixnetJoinEntry> configure = null)
        {
            queryableInfo.Join(joinEntry, configure);
            return this;
        }

        #endregion

        #region Join core

        ISixnetQueryable<TFirst> Join<TFirst>(bool predicate, SixnetJoinType joinType, ISixnetQueryable firstQueryable
            , Expression<Func<TFirst, bool>> connection = null, Action<SixnetJoinEntry> configure = null)
        {
            if (predicate)
            {
                var joinQueryable = SixnetExpressionHelper.GetQueryable(connection, SixnetCriterionConnector.And);
                var targetQueryable = SixnetQuerier.Create<TFirst>();
                if (firstQueryable != null)
                {
                    targetQueryable.From(firstQueryable);
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
            return SixnetQuerier.Create<TFirst>(this);
        }

        #endregion

        #endregion

        #region Combine

        #region UnionAll

        /// <summary>
        /// Union all
        /// </summary>
        /// <param name="unionQueryable">Union queryable</param>
        /// <returns></returns>
        public ISixnetQueryable UnionAll(ISixnetQueryable unionQueryable)
        {
            return CombineCore(SixnetCombineType.UnionAll, unionQueryable);
        }

        /// <summary>
        /// Union all
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="unionExpression">Union expression</param>
        /// <returns></returns>
        public ISixnetQueryable UnionAll<TTarget>(Expression<Func<TTarget, bool>> unionExpression = null)
        {
            return CombineCore(SixnetCombineType.UnionAll, SixnetQuerier.Create(unionExpression));
        }

        #endregion

        #region Union

        /// <summary>
        /// Union
        /// </summary>
        /// <param name="unionQueryable">Union queryable</param>
        /// <returns></returns>
        public ISixnetQueryable Union(ISixnetQueryable unionQueryable)
        {
            return CombineCore(SixnetCombineType.Union, unionQueryable);
        }

        /// <summary>
        /// Union
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="unionExpression">Union expression</param>
        /// <returns></returns>
        public ISixnetQueryable Union<TTarget>(Expression<Func<TTarget, bool>> unionExpression = null)
        {
            return CombineCore(SixnetCombineType.Union, SixnetQuerier.Create(unionExpression));
        }

        #endregion

        #region Except

        /// <summary>
        /// Except
        /// </summary>
        /// <param name="exceptQueryable">Except queryable</param>
        /// <returns></returns>
        public ISixnetQueryable Except(ISixnetQueryable exceptQueryable)
        {
            return CombineCore(SixnetCombineType.Except, exceptQueryable);
        }

        /// <summary>
        /// Except
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="exceptExpression">Except expression</param>
        /// <returns></returns>
        public ISixnetQueryable Except<TTarget>(Expression<Func<TTarget, bool>> exceptExpression = null)
        {
            return CombineCore(SixnetCombineType.Except, SixnetQuerier.Create(exceptExpression));
        }

        #endregion

        #region Intersect

        /// <summary>
        /// Intersect
        /// </summary>
        /// <param name="intersectQueryable">Intersect queryable</param>
        /// <returns></returns>
        public ISixnetQueryable Intersect(ISixnetQueryable intersectQueryable)
        {
            return CombineCore(SixnetCombineType.Intersect, intersectQueryable);
        }

        /// <summary>
        /// Intersect
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="intersectExpression">Intersect expression</param>
        /// <returns></returns>
        public ISixnetQueryable Intersect<TTarget>(Expression<Func<TTarget, bool>> intersectExpression = null)
        {
            return CombineCore(SixnetCombineType.Intersect, SixnetQuerier.Create(intersectExpression));
        }

        #endregion

        #region Combine core

        /// <summary>
        /// Combine
        /// </summary>
        /// <param name="combineType">Combine type</param>
        /// <param name="combineQueryable">Combine queryable</param>
        /// <returns></returns>
        ISixnetQueryable CombineCore(SixnetCombineType combineType, ISixnetQueryable combineQueryable)
        {
            queryableInfo.Combine(new SixnetCombineEntry()
            {
                Type = combineType,
                Target = combineQueryable
            });
            return this;
        }

        #endregion

        #endregion

        #region Model Type

        /// <summary>
        /// Get the primary model type associated with the IQueryable
        /// </summary>
        /// <returns></returns>
        public Type GetModelType()
        {
            return queryableInfo.ModelType;
        }

        /// <summary>
        /// Set the primary model type associated with the IQueryable
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <returns></returns>
        public ISixnetQueryable SetModelType(Type modelType)
        {
            queryableInfo.SetModelType(modelType);
            return this;
        }

        #endregion

        #region Take

        /// <summary>
        /// Set take data count
        /// </summary>
        /// <param name="count">Take data count</param>
        /// <param name="skip">Skip data count</param>
        /// <returns></returns>
        public ISixnetQueryable Take(int count, int skip = 0)
        {
            queryableInfo.Take(count, skip);
            return this;
        }

        #endregion

        #region Distinct

        /// <summary>
        /// Distinct
        /// </summary>
        /// <returns></returns>
        public ISixnetQueryable Distinct()
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
        public ISixnetQueryable GroupBy(params string[] fieldNames)
        {
            queryableInfo.GroupBy(fieldNames);
            return this;
        }

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public ISixnetQueryable GroupBy(params ISixnetField[] fields)
        {
            queryableInfo.GroupBy(fields);
            return this;
        }

        #endregion

        #region Having

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <returns></returns>
        public ISixnetQueryable Having(ISixnetQueryable queryable)
        {
            return HavingIf(true, queryable);
        }

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="queryable">Queryable</param>
        /// <returns></returns>
        public ISixnetQueryable HavingIf(bool predicate, ISixnetQueryable queryable)
        {
            if (predicate && queryable != null)
            {
                queryableInfo.Having(queryable);
            }
            return this;
        }

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        protected void HavingExpressionCore(bool predicate, Expression expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And)
        {
            if (predicate && expression != null)
            {
                var expressionCondition = SixnetExpressionHelper.GetQueryable(expression, connector);
                queryableInfo.Having(expressionCondition);
            }
        }

        #endregion

        #region From

        /// <summary>
        /// From other queryable
        /// </summary>
        /// <param name="targetQueryable">Target queryable</param>
        /// <returns></returns>
        public ISixnetQueryable From(ISixnetQueryable targetQueryable)
        {
            queryableInfo.FromQueryable(targetQueryable);
            return this;
        }

        /// <summary>
        /// From specify table
        /// </summary>
        /// <param name="tableNames">Table names</param>
        /// <returns></returns>
        public ISixnetQueryable From(params string[] tableNames)
        {
            FromSpecifyTableCore(tableNames);
            return this;
        }

        protected void FromSpecifyTableCore(params string[] tableNames)
        {
            queryableInfo.From(tableNames?.Select(t => SixnetDatabaseObjectName.Create(t, SixnetDatabaseObjectType.Table)));
        }

        /// <summary>
        /// As a data source
        /// </summary>
        /// <returns>A new Queryable</returns>
        public ISixnetQueryable AsSource()
        {
            return SixnetQuerier.Create().From(this);
        }

        /// <summary>
        /// As a data source
        /// </summary>
        /// <returns>A new Queryable</returns>
        public ISixnetQueryable<TSource> AsSource<TSource>()
        {
            return SixnetQuerier.Create<TSource>().From(this);
        }

        /// <summary>
        /// As a temp table
        /// </summary>
        /// <returns></returns>
        public ISixnetQueryable AsTempTable()
        {
            var tempTableName = CreateTempTableCore();
            var newQueryable = SixnetQuerier.Create();
            newQueryable.From(tempTableName);
            return newQueryable;

        }

        /// <summary>
        /// As a temp table
        /// </summary>
        /// <returns></returns>
        public ISixnetQueryable<TTable> AsTempTable<TTable>()
        {
            var tempTableName = CreateTempTableCore();
            var newQueryable = SixnetQuerier.Create<TTable>();
            newQueryable.From(tempTableName);
            return newQueryable;
        }

        string CreateTempTableCore()
        {
            return CreateTempTable()?.Name;
        }

        #endregion

        #region Split table

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitValue">Split value</param>
        /// <returns></returns>
        public ISixnetQueryable SplitTable(dynamic splitValue)
        {
            SplitTableCore(splitValue);
            return this;
        }

        protected void SplitTableCore(dynamic splitValue)
        {
            queryableInfo.SplitTable(new List<dynamic>(1) { splitValue }, SixnetSplitTableNameSelectionPattern.Precision);
        }

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="startSplitValue">Start split value</param>
        /// <param name="endSplitValue">End split value</param>
        /// <returns></returns>
        public ISixnetQueryable SplitTable(dynamic startSplitValue, dynamic endSplitValue)
        {
            SplitTableCore(startSplitValue, endSplitValue);
            return this;
        }

        protected void SplitTableCore(dynamic startSplitValue, dynamic endSplitValue)
        {
            queryableInfo.SplitTable(new List<dynamic>(2) { startSplitValue, endSplitValue }, SixnetSplitTableNameSelectionPattern.Range);
        }

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitValues">Split values</param>
        /// <returns></returns>
        public ISixnetQueryable SplitTable(IEnumerable<dynamic> splitValues, SixnetSplitTableNameSelectionPattern selectionPattern = SixnetSplitTableNameSelectionPattern.Precision)
        {
            queryableInfo.SplitTable(splitValues, selectionPattern);
            return this;
        }

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitTableNameFilter">Split table name filter</param>
        /// <returns></returns>
        public ISixnetQueryable SplitTable(Func<IEnumerable<SixnetDatabaseObjectName>, IEnumerable<SixnetDatabaseObjectName>, IEnumerable<SixnetDatabaseObjectName>> splitTableNameFilter)
        {
            queryableInfo.SplitTable(splitTableNameFilter);
            return this;
        }

        #endregion

        #region Output

        /// <summary>
        /// Set output
        /// </summary>
        /// <param name="outputType">Output type</param>
        /// <returns></returns>
        public ISixnetQueryable Output(SixnetQueryableOutputType outputType)
        {
            queryableInfo.Output(outputType);
            return this;
        }

        #endregion

        #region Filter

        /// <summary>
        /// Include archived
        /// </summary>
        /// <returns></returns>
        public ISixnetQueryable IncludeArchived()
        {
            queryableInfo.IgnoreFilter(SixnetFieldRole.Archive);
            return this;
        }

        /// <summary>
        /// Ignore data isolation
        /// </summary>
        /// <returns></returns>
        public ISixnetQueryable IgnoreIsolation()
        {
            queryableInfo.IgnoreFilter(SixnetFieldRole.Isolation);
            return this;
        }

        /// <summary>
        /// Ignore filter
        /// </summary>
        /// <typeparam name="TFilter">Filter type</typeparam>
        /// <returns></returns>
        public ISixnetQueryable IgnoreFilter<TFilter>()
        {
            queryableInfo.IgnoreFilter(typeof(TFilter));
            return this;
        }

        /// <summary>
        /// Ignore filter
        /// </summary>
        /// <param name="filterType">Filter type</param>
        /// <returns></returns>
        public ISixnetQueryable IgnoreFilter(Type filterType)
        {
            queryableInfo.IgnoreFilter(filterType);
            return this;
        }

        /// <summary>
        /// Has ignored filter
        /// </summary>
        /// <param name="fieldRole">Field role</param>
        /// <returns></returns>
        public bool HasIgnoredFilter(SixnetFieldRole fieldRole)
        {
            return queryableInfo.HasIgnoredFilter(fieldRole);
        }

        /// <summary>
        /// Has ignored type filter
        /// </summary>
        /// <typeparam name="TFilter">Filter type</typeparam>
        /// <returns></returns>
        public bool HasIgnoredFilter<TFilter>()
        {
            return queryableInfo.HasIgnoredFilter(typeof(TFilter));
        }

        /// <summary>
        /// Has ignored type filter
        /// </summary>
        /// <param name="filterType">Filter type</param>
        /// <returns></returns>
        public bool HasIgnoredFilter(Type filterType)
        {
            return queryableInfo.HasIgnoredFilter(filterType);
        }

        #endregion

        #region Negate

        /// <summary>
        /// Negate
        /// </summary>
        /// <returns></returns>
        public ISixnetQueryable Negate()
        {
            queryableInfo.Negate();
            return this;
        }

        #endregion

        #region Subquery

        /// <summary>
        /// Contains
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns></returns>
        public bool Contains(object value)
        {
            return true;
        }

        /// <summary>
        /// Not contains
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns></returns>
        public bool NotContains(object value)
        {
            return true;
        }

        /// <summary>
        /// Equal
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Equal(object value)
        {
            return true;
        }

        /// <summary>
        /// Not equal
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool NotEqual(object value)
        {
            return true;
        }

        /// <summary>
        /// Less than or equal
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool LessThanOrEqual(object value)
        {
            return true;
        }

        /// <summary>
        /// Less than
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool LessThan(object value)
        {
            return true;
        }

        /// <summary>
        /// Greater than
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool GreaterThan(object value)
        {
            return true;
        }

        /// <summary>
        /// Greater than or equal
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool GreaterThanOrEqual(object value)
        {
            return true;
        }

        #endregion

        #region Data access

        #region Update

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public int Update(SixnetFieldsAssignment fieldsAssignment, Action<SixnetDataOperationOptions> configure = null)
        {
            var repository = queryableInfo.Repository;
            if (repository != null)
            {
                return repository.Update(fieldsAssignment, this, configure);
            }
            return SixnetDataClientContext.Update(fieldsAssignment, this, configure);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Delete by condition
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public int Delete(Action<SixnetDataOperationOptions> configure = null)
        {
            var repository = queryableInfo.Repository;
            if (repository != null)
            {
                return repository.Delete(this, configure);
            }
            return SixnetDataClientContext.Delete(this, configure);
        }

        #endregion

        #region First

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data</returns>
        public T First<T>(Action<SixnetDataOperationOptions> configure = null)
        {
            if (queryableInfo.Repository is ISixnetRepository<T> repository)
            {
                return repository.Get(this, configure);
            }
            return SixnetDataClientContext.QueryFirst<T>(this, configure);
        }

        #endregion

        #region List

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data list</returns>
        public List<T> ToList<T>(Action<SixnetDataOperationOptions> configure = null)
        {
            if (queryableInfo.Repository is ISixnetRepository<T> repository)
            {
                return repository.GetList(this, configure);
            }
            return SixnetDataClientContext.Query<T>(this, configure);
        }

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        public List<TReturn> ToList<TFirst, TSecond, TReturn>(Func<TFirst, TSecond, TReturn> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null)
        {
            return SixnetDataClientContext.QueryMapping(this, dataMappingFunc, configure);
        }

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        public List<TReturn> ToList<TFirst, TSecond, TThird, TReturn>(Func<TFirst, TSecond, TThird, TReturn> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null)
        {
            return SixnetDataClientContext.QueryMapping(this, dataMappingFunc, configure);
        }

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        public List<TReturn> ToList<TFirst, TSecond, TThird, TFourth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TReturn> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null)
        {
            return SixnetDataClientContext.QueryMapping(this, dataMappingFunc, configure);
        }

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <typeparam name="TFifth">Fifth data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        public List<TReturn> ToList<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null)
        {
            return SixnetDataClientContext.QueryMapping(this, dataMappingFunc, configure);
        }

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <typeparam name="TFifth">Fifth data type</typeparam>
        /// <typeparam name="TSixth">Sixth data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        public List<TReturn> ToList<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null)
        {
            return SixnetDataClientContext.QueryMapping(this, dataMappingFunc, configure);
        }

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <typeparam name="TFifth">Fifth data type</typeparam>
        /// <typeparam name="TSixth">Sixth data type</typeparam>
        /// <typeparam name="TSeventh">Seventh data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        public List<TReturn> ToList<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null)
        {
            return SixnetDataClientContext.QueryMapping(this, dataMappingFunc, configure);
        }

        #endregion

        #region Paging

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public SixnetPagingInfo<T> ToPaging<T>(SixnetPagingFilter pagingFilter, Action<SixnetDataOperationOptions> configure = null)
        {
            if (queryableInfo.Repository is ISixnetRepository<T> repository)
            {
                return repository.GetPaging(this, pagingFilter, configure);
            }
            return SixnetDataClientContext.QueryPaging<T>(this, pagingFilter, configure);
        }

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public SixnetPagingInfo<T> ToPaging<T>(int page, int pageSize, Action<SixnetDataOperationOptions> configure = null)
        {
            return ToPaging<T>(SixnetPagingFilter.Create(page, pageSize), configure);
        }

        #endregion

        #region Any

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Whether has data</returns>
        public bool Any(Action<SixnetDataOperationOptions> configure = null)
        {
            var repository = queryableInfo.Repository;
            if (repository != null)
            {
                return repository.Exists(this, configure);
            }
            return SixnetDataClientContext.Exists(this, configure);
        }

        #endregion

        #region Count

        /// <summary>
        /// Count data num
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data num</returns>
        public int Count(Action<SixnetDataOperationOptions> configure = null)
        {
            var repository = queryableInfo.Repository;
            if (repository != null)
            {
                return repository.Count(this, configure);
            }
            return SixnetDataClientContext.Count(this, configure);
        }

        #endregion

        #region Max

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="configure">Confirure options </param>
        /// <returns>Max value</returns>
        public TValue Max<TValue>(Action<SixnetDataOperationOptions> configure = null)
        {
            var repository = queryableInfo.Repository;
            if (repository != null)
            {
                return repository.Max<TValue>(this, configure);
            }
            return SixnetDataClientContext.Max<TValue>(this, configure);
        }

        #endregion

        #region Min

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="configure">Confirure options </param>
        /// <returns>Min value</returns>
        public TValue Min<TValue>(Action<SixnetDataOperationOptions> configure = null)
        {
            var repository = queryableInfo.Repository;
            if (repository != null)
            {
                return repository.Min<TValue>(this, configure);
            }
            return SixnetDataClientContext.Min<TValue>(this, configure);
        }

        #endregion

        #region Sum

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="configure">Confirure options </param>
        /// <returns>Sum value</returns>
        public TValue Sum<TValue>(Action<SixnetDataOperationOptions> configure = null)
        {
            var repository = queryableInfo.Repository;
            if (repository != null)
            {
                return repository.Sum<TValue>(this, configure);
            }
            return SixnetDataClientContext.Sum<TValue>(this, configure);
        }

        #endregion

        #region Avg

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="configure">Confirure options </param>
        /// <returns>Average value</returns>
        public TValue Avg<TValue>(Action<SixnetDataOperationOptions> configure = null)
        {
            var repository = queryableInfo.Repository;
            if (repository != null)
            {
                return repository.Avg<TValue>(this, configure);
            }
            return SixnetDataClientContext.Avg<TValue>(this, configure);
        }

        #endregion

        #region Scalar

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        public TValue Scalar<TValue>(Action<SixnetDataOperationOptions> configure = null)
        {
            var repository = queryableInfo.Repository;
            if (repository != null)
            {
                return repository.Scalar<TValue>(this, configure);
            }
            return SixnetDataClientContext.Scalar<TValue>(this, configure);
        }

        #endregion

        #region Temp table

        protected SixnetTempTable CreateTempTable(Action<SixnetDataOperationOptions> configure = null)
        {
            return SixnetDataClientContext.CreateTempTable(this, configure);
        }

        #endregion

        #endregion

        #region Read only

        /// <summary>
        /// Mark as read only
        /// </summary>
        /// <returns></returns>
        public ISixnetQueryable ReadOnly()
        {
            queryableInfo.ReadOnly();
            return this;
        }

        #endregion

        #endregion
    }
}
