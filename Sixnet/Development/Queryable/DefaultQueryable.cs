using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Database;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Repository;
using Sixnet.Expressions.Linq;
using Sixnet.Model.Paging;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Defines default implement for ISixnetQueryable
    /// </summary>
    [Serializable]
    internal partial class DefaultQueryable : ISixnetQueryable
    {
        #region Fields

        protected QueryableContext queryableContext = null;

        #endregion

        #region Properties

        /// <summary>
        /// Get the queryable id
        /// </summary>
        public Guid Id => queryableContext.Id;

        /// <summary>
        /// Gets the connector
        /// </summary>
        public CriterionConnector Connector
        {
            get
            {
                return queryableContext.Connector;
            }
            set
            {
                queryableContext.Connector = value;
            }
        }

        /// <summary>
        /// Gets all conditions
        /// </summary>
        public IEnumerable<ICondition> Conditions => queryableContext.Conditions;

        /// <summary>
        /// Gets all criterion
        /// </summary>
        public IEnumerable<Criterion> Criteria => queryableContext.Criteria;

        /// <summary>
        /// Gets all sorts
        /// </summary>
        public IEnumerable<SortEntry> Sorts => queryableContext.Sorts;

        /// <summary>
        /// Get the selected fields
        /// </summary>
        public IEnumerable<IDataField> SelectedFields => queryableContext.SelectedFields;

        /// <summary>
        /// Get the unselected fields
        /// </summary>
        public IEnumerable<IDataField> UnselectedFields => queryableContext.UnselectedFields;

        /// <summary>
        /// Gets the group fields
        /// </summary>
        public IEnumerable<IDataField> GroupFields => queryableContext.GroupFields;

        /// <summary>
        /// Gets the query text
        /// </summary>
        public string Script => queryableContext.Script;

        /// <summary>
        /// Gets the query text parameter
        /// </summary>
        public IEnumerable<KeyValuePair<string, object>> ScriptParameters => queryableContext.ScriptParameters;

        /// <summary>
        /// Gets the script type
        /// </summary>
        public DataScriptType ScriptType => queryableContext.ScriptType;

        /// <summary>
        /// Gets the query command type
        /// </summary>
        public QueryableExecutionMode ExecutionMode => queryableContext.ExecutionMode;

        /// <summary>
        /// Gets or sets the skip data count
        /// </summary>
        public int SkipCount => queryableContext.SkipCount;

        /// <summary>
        /// Gets or sets the take data count
        /// </summary>
        public int TakeCount => queryableContext.TakeCount;

        /// <summary>
        /// Indicates whether has subquery
        /// </summary>
        public bool HasSubquery => queryableContext.HasSubQueryable;

        /// <summary>
        /// Indicates whether has recurve
        /// </summary>
        public bool HasRecurve => queryableContext.HasTreeMatching;

        /// <summary>
        /// Indicates whether has join
        /// </summary>
        public bool HasJoin => queryableContext.HasJoin;

        /// <summary>
        /// Indicates whether has combine
        /// </summary>
        public bool HasCombine => queryableContext.HasCombine;

        /// <summary>
        /// Indicates whether has field formatter
        /// </summary>
        public bool HasFieldFormatter => queryableContext.HasFieldFormatter;

        /// <summary>
        /// Indicates whether is a complex query
        /// Has subquery,recurve,join,field converter
        /// </summary>
        public bool IsComplex => queryableContext.IsComplex;

        /// <summary>
        /// Gets the recurve condition
        /// </summary>
        public TreeInfo TreeInfo => queryableContext.TreeInfo;

        /// <summary>
        /// Gets the join entries
        /// </summary>
        public IEnumerable<JoinEntry> Joins => queryableContext.Joins;

        /// <summary>
        /// Gets the combine entries
        /// </summary>
        public IEnumerable<CombineEntry> Combines => queryableContext.Combines;

        /// <summary>
        /// Indicates there is no conditions
        /// </summary>
        public bool None => queryableContext.NoneCondition;

        /// <summary>
        /// Gets or sets the data isolation level
        /// </summary>
        public DataIsolationLevel? IsolationLevel => queryableContext.IsolationLevel;

        /// <summary>
        /// Whether include archived data
        /// </summary>
        public bool IncludeArchivedData => queryableContext.IncludeArchivedData;

        /// <summary>
        /// Whether unisolated data
        /// </summary>
        public bool UnisolatedData => queryableContext.UnisolatedData;

        /// <summary>
        /// Gets the from type
        /// </summary>
        public QueryableFromType FromType => queryableContext.FromType;

        /// <summary>
        /// Gets the target queryable
        /// </summary>
        public ISixnetQueryable TargetQueryable => queryableContext.TargetQueryable;

        /// <summary>
        /// Gets the output type
        /// </summary>
        public QueryableOutputType OutputType => queryableContext.OutputType;

        /// <summary>
        /// Whether is a group queryable
        /// </summary>
        public bool IsGroupQueryable => queryableContext.CheckUseForGroup();

        /// <summary>
        /// Gets or sets the queryable
        /// </summary>
        public ISixnetQueryable HavingQueryable => queryableContext.HavingQueryable;

        /// <summary>
        /// Gets the split table behavior
        /// </summary>
        public SplitTableBehavior SplitTableBehavior => queryableContext.SplitTableBehavior;

        /// <summary>
        /// Whether is distinct
        /// </summary>
        public bool IsDistincted => queryableContext.IsDistincted;

        /// <summary>
        /// Whether negation
        /// </summary>
        public bool Negation => queryableContext.Negation;

        #endregion

        #region Constructor

        public DefaultQueryable(QueryableContext sourceQueryableContext)
        {
            queryableContext = sourceQueryableContext ?? new QueryableContext();
            InitQueryable(queryableContext);
        }

        public DefaultQueryable(ISixnetQueryable sixnetQueryable) : this((sixnetQueryable as DefaultQueryable)?.queryableContext)
        {
        }

        void InitQueryable(QueryableContext queryableContext)
        {
            // Init model type
            var currentModelType = queryableContext.GetModelType();
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
        public ISixnetQueryable Where(ICondition condition)
        {
            queryableContext.AddCondition(condition);
            return this;
        }

        /// <summary>
        /// Append group condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="condition">Group condition</param>
        /// <returns></returns>
        public ISixnetQueryable WhereIf(bool predicate, ICondition condition)
        {
            if (predicate)
            {
                queryableContext.AddCondition(condition);
            }
            return this;
        }

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        protected void WhereExpressionCore(Expression expression, CriterionConnector connector = CriterionConnector.And)
        {
            if (expression != null)
            {
                var expressionCondition = ExpressionHelper.GetQueryable(expression, connector);
                queryableContext.AddCondition(expressionCondition);
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
                    queryableContext.AddSort(SortEntry.Create(field, desc, targetType));
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
                queryableContext.AddSort(SortEntry.Create(fieldName, desc, targetType));
            }
            return this;
        }

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        public ISixnetQueryable OrderBy(IDataField field, bool desc = false)
        {
            queryableContext.AddSort(new SortEntry()
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
        public ISixnetQueryable OrderBy(IEnumerable<IDataField> fields, bool desc = false)
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
                    queryableContext.AddSort(SortEntry.Create(field, desc, targetType));
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
                queryableContext.AddSort(SortEntry.Create(fieldName, desc, targetType));
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
        public ISixnetQueryable OrderByIf(bool predicate, IDataField field, bool desc = false)
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
        public ISixnetQueryable OrderByIf(bool predicate, IEnumerable<IDataField> fields, bool desc = false)
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
            queryableContext.ClearSort();
            return this;
        }

        #endregion

        #region Select

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public ISixnetQueryable Select(params IDataField[] fields)
        {
            if (!fields.IsNullOrEmpty())
            {
                queryableContext.SelectFields(fields);
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
            return Select(fieldNames?.Select(f => PropertyField.Create(f)).ToArray());
        }

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public ISixnetQueryable Unselect(params IDataField[] fields)
        {
            if (!fields.IsNullOrEmpty())
            {
                queryableContext.UnselectFields(fields);
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
            return Unselect(fieldNames?.Select(f => PropertyField.Create(f)).ToArray());
        }

        /// <summary>
        /// Get the selected fields
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <param name="includeNecessaryFields">Whether include the necessary fields</param>
        /// <returns></returns>
        public IEnumerable<IDataField> GetSelectedFields(Type modelType, bool includeNecessaryFields)
        {
            return queryableContext.GetFinallyFields(modelType, includeNecessaryFields);
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
        public ISixnetQueryable TreeMatching(string dataFieldName, string parentFieldName, TreeMatchingDirection direction = TreeMatchingDirection.Down)
        {
            queryableContext.TreeMatching(dataFieldName, parentFieldName, direction);
            return this;
        }

        /// <summary>
        /// Tree matching
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        public ISixnetQueryable TreeMatching(IDataField dataField, IDataField parentField, TreeMatchingDirection direction = TreeMatchingDirection.Down)
        {
            queryableContext.TreeMatching(dataField, parentField, direction);
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
            return new DefaultQueryable(queryableContext?.LightClone());
        }

        protected virtual ISixnetQueryable CloneCore()
        {
            return new DefaultQueryable(queryableContext?.Clone());
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
        public ISixnetQueryable SetScript(string script, DataScriptType scriptType = DataScriptType.Text, object parameters = null)
        {
            queryableContext.SetScript(script, scriptType, parameters);
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
            return queryableContext.GetValidationFunction<T>();
        }

        #endregion

        #region Join

        #region Inner join

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> InnerJoin<TFirst>(Expression<Func<TFirst, bool>> joinExpression = null)
        {
            return InnerJoin(null, joinExpression);
        }

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="firstQueryable">First queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> InnerJoin<TFirst>(ISixnetQueryable<TFirst> firstQueryable, Expression<Func<TFirst, bool>> joinExpression = null)
        {
            return Join(true, JoinType.InnerJoin, firstQueryable, joinExpression);
        }

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> InnerJoinIf<TFirst>(bool predicate, Expression<Func<TFirst, bool>> joinExpression = null)
        {
            return InnerJoinIf(predicate, null, joinExpression);
        }

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="firstQueryable">First queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> InnerJoinIf<TFirst>(bool predicate, ISixnetQueryable<TFirst> firstQueryable, Expression<Func<TFirst, bool>> joinExpression = null)
        {
            return Join(predicate, JoinType.InnerJoin, firstQueryable, joinExpression);
        }

        #endregion

        #region Left join

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> LeftJoin<TFirst>(Expression<Func<TFirst, bool>> joinExpression = null)
        {
            return LeftJoin(null, joinExpression);
        }

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="secondQueryable">First queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> LeftJoin<TFirst>(ISixnetQueryable<TFirst> firstQueryable, Expression<Func<TFirst, bool>> joinExpression = null)
        {
            return Join(true, JoinType.LeftJoin, firstQueryable, joinExpression);
        }

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> LeftJoinIf<TFirst>(bool predicate, Expression<Func<TFirst, bool>> joinExpression = null)
        {
            return LeftJoinIf(predicate, null, joinExpression);
        }

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="firstQueryable">First queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> LeftJoinIf<TFirst>(bool predicate, ISixnetQueryable<TFirst> firstQueryable, Expression<Func<TFirst, bool>> joinExpression = null)
        {
            return Join(predicate, JoinType.LeftJoin, firstQueryable, joinExpression);
        }

        #endregion

        #region Right join

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> RightJoin<TFirst>(Expression<Func<TFirst, bool>> joinExpression = null)
        {
            return RightJoin(null, joinExpression);
        }

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="firstQueryable">First queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> RightJoin<TFirst>(ISixnetQueryable<TFirst> firstQueryable, Expression<Func<TFirst, bool>> joinExpression = null)
        {
            return Join(true, JoinType.RightJoin, firstQueryable, joinExpression);
        }

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> RightJoinIf<TFirst>(bool predicate, Expression<Func<TFirst, bool>> joinExpression = null)
        {
            return RightJoinIf(predicate, null, joinExpression);
        }

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="firstQueryable">First queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> RightJoinIf<TFirst>(bool predicate, ISixnetQueryable<TFirst> firstQueryable, Expression<Func<TFirst, bool>> joinExpression = null)
        {
            return Join(predicate, JoinType.RightJoin, firstQueryable, joinExpression);
        }

        #endregion

        #region Full join

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> FullJoin<TFirst>(Expression<Func<TFirst, bool>> joinExpression = null)
        {
            return FullJoin(null, joinExpression);
        }

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="firstQueryable">First queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> FullJoin<TFirst>(ISixnetQueryable<TFirst> firstQueryable, Expression<Func<TFirst, bool>> joinExpression = null)
        {
            return Join(true, JoinType.FullJoin, firstQueryable, joinExpression);
        }

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> FullJoinIf<TFirst>(bool predicate, Expression<Func<TFirst, bool>> joinExpression = null)
        {
            return FullJoinIf(predicate, null, joinExpression);
        }

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="firstQueryable">First queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> FullJoinIf<TFirst>(bool predicate, ISixnetQueryable<TFirst> firstQueryable, Expression<Func<TFirst, bool>> joinExpression = null)
        {
            return Join(predicate, JoinType.FullJoin, firstQueryable, joinExpression);
        }

        #endregion

        #region Cross join

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> CrossJoin<TFirst>()
        {
            return CrossJoin<TFirst>(null);
        }

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="firstQueryable">First queryable</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> CrossJoin<TFirst>(ISixnetQueryable<TFirst> firstQueryable)
        {
            return Join(true, JoinType.CrossJoin, firstQueryable);
        }

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> CrossJoinIf<TFirst>(bool predicate)
        {
            return CrossJoinIf<TFirst>(predicate, null);
        }

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="firstQueryable">First queryable</param>
        /// <returns></returns>
        public ISixnetQueryable<TFirst> CrossJoinIf<TFirst>(bool predicate, ISixnetQueryable<TFirst> firstQueryable)
        {
            return Join(predicate, JoinType.CrossJoin, firstQueryable);
        }

        #endregion

        #region Add join

        /// <summary>
        /// Add join
        /// </summary>
        /// <param name="joinEntries">Join entries</param>
        /// <returns></returns>
        public ISixnetQueryable Join(params JoinEntry[] joinEntries)
        {
            if (!joinEntries.IsNullOrEmpty())
            {
                foreach (var joinEntry in joinEntries)
                {
                    queryableContext.Join(joinEntry);
                }
            }
            return this;
        }

        #endregion

        #region Join core

        ISixnetQueryable<TFirst> Join<TFirst>(bool predicate, JoinType joinType, ISixnetQueryable<TFirst> firstQueryable, Expression<Func<TFirst, bool>> joinExpression = null)
        {
            if (predicate)
            {
                var joinQueryable = ExpressionHelper.GetQueryable(joinExpression, CriterionConnector.And);
                var targetQueryable = SixnetQueryable.Create<TFirst>();
                if (firstQueryable != null)
                {
                    targetQueryable.From(firstQueryable);
                }
                queryableContext.Join(new JoinEntry()
                {
                    TargetQueryable = targetQueryable,
                    Type = joinType,
                    Connection = joinQueryable
                });
            }
            return SixnetQueryable.Create<TFirst>(this);
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
            return CombineCore(CombineType.UnionAll, unionQueryable);
        }

        /// <summary>
        /// Union all
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="unionExpression">Union expression</param>
        /// <returns></returns>
        public ISixnetQueryable UnionAll<TTarget>(Expression<Func<TTarget, bool>> unionExpression = null)
        {
            return CombineCore(CombineType.UnionAll, SixnetQueryable.Create(unionExpression));
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
            return CombineCore(CombineType.Union, unionQueryable);
        }

        /// <summary>
        /// Union
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="unionExpression">Union expression</param>
        /// <returns></returns>
        public ISixnetQueryable Union<TTarget>(Expression<Func<TTarget, bool>> unionExpression = null)
        {
            return CombineCore(CombineType.Union, SixnetQueryable.Create(unionExpression));
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
            return CombineCore(CombineType.Except, exceptQueryable);
        }

        /// <summary>
        /// Except
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="exceptExpression">Except expression</param>
        /// <returns></returns>
        public ISixnetQueryable Except<TTarget>(Expression<Func<TTarget, bool>> exceptExpression = null)
        {
            return CombineCore(CombineType.Except, SixnetQueryable.Create(exceptExpression));
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
            return CombineCore(CombineType.Intersect, intersectQueryable);
        }

        /// <summary>
        /// Intersect
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="intersectExpression">Intersect expression</param>
        /// <returns></returns>
        public ISixnetQueryable Intersect<TTarget>(Expression<Func<TTarget, bool>> intersectExpression = null)
        {
            return CombineCore(CombineType.Intersect, SixnetQueryable.Create(intersectExpression));
        }

        #endregion

        #region Combine core

        /// <summary>
        /// Combine
        /// </summary>
        /// <param name="combineType">Combine type</param>
        /// <param name="combineQueryable">Combine queryable</param>
        /// <returns></returns>
        ISixnetQueryable CombineCore(CombineType combineType, ISixnetQueryable combineQueryable)
        {
            queryableContext.Combine(new CombineEntry()
            {
                Type = combineType,
                TargetQueryable = combineQueryable
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
            return queryableContext.GetModelType();
        }

        /// <summary>
        /// Set the primary model type associated with the IQueryable
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <returns></returns>
        public ISixnetQueryable SetModelType(Type modelType)
        {
            queryableContext.SetModelType(modelType);
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
            queryableContext.Take(count, skip);
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
        public ISixnetQueryable GroupBy(params string[] fieldNames)
        {
            queryableContext.GroupBy(fieldNames);
            return this;
        }

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        public ISixnetQueryable GroupBy(params IDataField[] fields)
        {
            queryableContext.GroupBy(fields);
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
                queryableContext.Having(queryable);
            }
            return this;
        }

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        protected void HavingExpressionCore(bool predicate, Expression expression, CriterionConnector connector = CriterionConnector.And)
        {
            if (predicate && expression != null)
            {
                var expressionCondition = ExpressionHelper.GetQueryable(expression, connector);
                queryableContext.Having(expressionCondition);
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
            queryableContext.FromQueryable(targetQueryable);
            return this;
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
            queryableContext.SplitTable(new List<dynamic>(1) { splitValue }, SplitTableNameSelectionPattern.Precision);
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
            queryableContext.SplitTable(new List<dynamic>(2) { startSplitValue, endSplitValue }, SplitTableNameSelectionPattern.Range);
        }

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitValues">Split values</param>
        /// <returns></returns>
        public ISixnetQueryable SplitTable(IEnumerable<dynamic> splitValues, SplitTableNameSelectionPattern selectionPattern = SplitTableNameSelectionPattern.Precision)
        {
            queryableContext.SplitTable(splitValues, selectionPattern);
            return this;
        }

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitTableNameFilter">Split table name filter</param>
        /// <returns></returns>
        public ISixnetQueryable SplitTable(Func<IEnumerable<string>, IEnumerable<string>> splitTableNameFilter)
        {
            queryableContext.SplitTable(splitTableNameFilter);
            return this;
        }

        #endregion

        #region Output

        /// <summary>
        /// Set output
        /// </summary>
        /// <param name="outputType">Output type</param>
        /// <returns></returns>
        public ISixnetQueryable Output(QueryableOutputType outputType)
        {
            queryableContext.Output(outputType);
            return this;
        }

        #endregion

        #region Archived

        /// <summary>
        /// Include archived
        /// </summary>
        /// <returns></returns>
        public ISixnetQueryable IncludeArchived()
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
        public ISixnetQueryable Unisolated()
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
        public ISixnetQueryable Negate()
        {
            queryableContext.Negate();
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
        public int Update(FieldsAssignment fieldsAssignment, Action<DataOperationOptions> configure = null)
        {
            var repository = queryableContext.Repository;
            if (repository != null)
            {
                return repository.Update(fieldsAssignment, this, configure);
            }
            return DataClientContext.Update(fieldsAssignment, this, configure);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Delete by condition
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public int Delete(Action<DataOperationOptions> configure = null)
        {
            var repository = queryableContext.Repository;
            if (repository != null)
            {
                return repository.Delete(this, configure);
            }
            return DataClientContext.Delete(this, configure);
        }

        #endregion

        #region First

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data</returns>
        public T First<T>(Action<DataOperationOptions> configure = null)
        {
            if (queryableContext.Repository is IRepository<T> repository)
            {
                return repository.Get(this, configure);
            }
            return DataClientContext.QueryFirst<T>(this, configure);
        }

        #endregion

        #region List

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data list</returns>
        public List<T> ToList<T>(Action<DataOperationOptions> configure = null)
        {
            if (queryableContext.Repository is IRepository<T> repository)
            {
                return repository.GetList(this, configure);
            }
            return DataClientContext.Query<T>(this, configure);
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
        public List<TReturn> ToList<TFirst, TSecond, TReturn>(Func<TFirst, TSecond, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return DataClientContext.QueryMapping(this, dataMappingFunc, configure);
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
        public List<TReturn> ToList<TFirst, TSecond, TThird, TReturn>(Func<TFirst, TSecond, TThird, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return DataClientContext.QueryMapping(this, dataMappingFunc, configure);
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
        public List<TReturn> ToList<TFirst, TSecond, TThird, TFourth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return DataClientContext.QueryMapping(this, dataMappingFunc, configure);
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
        public List<TReturn> ToList<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return DataClientContext.QueryMapping(this, dataMappingFunc, configure);
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
        public List<TReturn> ToList<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return DataClientContext.QueryMapping(this, dataMappingFunc, configure);
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
        public List<TReturn> ToList<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return DataClientContext.QueryMapping(this, dataMappingFunc, configure);
        }

        #endregion

        #region Paging

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public PagingInfo<T> ToPaging<T>(PagingFilter pagingFilter, Action<DataOperationOptions> configure = null)
        {
            if (queryableContext.Repository is IRepository<T> repository)
            {
                return repository.GetPaging(this, pagingFilter, configure);
            }
            return DataClientContext.QueryPaging<T>(this, pagingFilter, configure);
        }

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public PagingInfo<T> ToPaging<T>(int page, int pageSize, Action<DataOperationOptions> configure = null)
        {
            return ToPaging<T>(PagingFilter.Create(page, pageSize), configure);
        }

        #endregion

        #region Any

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Whether has data</returns>
        public bool Any(Action<DataOperationOptions> configure = null)
        {
            var repository = queryableContext.Repository;
            if (repository != null)
            {
                return repository.Exists(this, configure);
            }
            return DataClientContext.Exists(this, configure);
        }

        #endregion

        #region Count

        /// <summary>
        /// Count data num
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data num</returns>
        public int Count(Action<DataOperationOptions> configure = null)
        {
            var repository = queryableContext.Repository;
            if (repository != null)
            {
                return repository.Count(this, configure);
            }
            return DataClientContext.Count(this, configure);
        }

        #endregion

        #region Max

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="configure">Confirure options </param>
        /// <returns>Max value</returns>
        public TValue Max<TValue>(Action<DataOperationOptions> configure = null)
        {
            var repository = queryableContext.Repository;
            if (repository != null)
            {
                return repository.Max<TValue>(this, configure);
            }
            return DataClientContext.Max<TValue>(this, configure);
        }

        #endregion

        #region Min

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="configure">Confirure options </param>
        /// <returns>Min value</returns>
        public TValue Min<TValue>(Action<DataOperationOptions> configure = null)
        {
            var repository = queryableContext.Repository;
            if (repository != null)
            {
                return repository.Min<TValue>(this, configure);
            }
            return DataClientContext.Min<TValue>(this, configure);
        }

        #endregion

        #region Sum

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="configure">Confirure options </param>
        /// <returns>Sum value</returns>
        public TValue Sum<TValue>(Action<DataOperationOptions> configure = null)
        {
            var repository = queryableContext.Repository;
            if (repository != null)
            {
                return repository.Sum<TValue>(this, configure);
            }
            return DataClientContext.Sum<TValue>(this, configure);
        }

        #endregion

        #region Avg

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="configure">Confirure options </param>
        /// <returns>Average value</returns>
        public TValue Avg<TValue>(Action<DataOperationOptions> configure = null)
        {
            var repository = queryableContext.Repository;
            if (repository != null)
            {
                return repository.Avg<TValue>(this, configure);
            }
            return DataClientContext.Avg<TValue>(this, configure);
        }

        #endregion

        #region Scalar

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        public TValue Scalar<TValue>(Action<DataOperationOptions> configure = null)
        {
            var repository = queryableContext.Repository;
            if (repository != null)
            {
                return repository.Scalar<TValue>(this, configure);
            }
            return DataClientContext.Scalar<TValue>(this, configure);
        }

        #endregion

        #endregion

        #endregion
    }
}
