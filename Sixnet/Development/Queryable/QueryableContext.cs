using Sixnet.Development.Data;
using Sixnet.Development.Data.Database;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Entity;
using Sixnet.Development.Repository;
using Sixnet.Exceptions;
using Sixnet.Expressions.Linq;
using Sixnet.Model;
using Sixnet.Model.Paging;
using Sixnet.Reflection;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Queryable context
    /// </summary>
    internal class QueryableContext : ISixnetCloneable<QueryableContext>
    {
        #region Fields

        /// <summary>
        /// Finally fields cache
        /// </summary>
        Dictionary<string, List<ISixnetField>> _finallyFieldsCache = new();

        /// <summary>
        /// Validation func
        /// </summary>
        [NonSerialized]
        Dictionary<Guid, dynamic> _validationFuncDict = new();

        /// <summary>
        /// Nullable entity id
        /// </summary>
        static readonly Guid _nullableEntityId = Guid.NewGuid();

        /// <summary>
        /// Join index
        /// </summary>
        int _joinIndex = 1;

        /// <summary>
        /// Default model type
        /// </summary>
        public static readonly Type DefaultModelType = typeof(ExpandoObject);

        #endregion

        #region Constructor

        internal QueryableContext()
        {
            Id = Guid.NewGuid();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the queryable context id
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Whether exclude necessary fields
        /// </summary>
        public bool IsExcludedNecessaryFields { get; private set; }

        /// <summary>
        /// Gets or sets the model type
        /// </summary>
        public Type ModelType { get; private set; }

        /// <summary>
        /// Ignored filter field role
        /// </summary>
        public FieldRole IgnoredFilterFieldRole { get; private set; }

        /// <summary>
        /// Ignored filter types
        /// </summary>
        public HashSet<Type> IgnoredFilterTypes { get; private set; }

        /// <summary>
        /// Gets all condition
        /// </summary>
        public List<ISixnetCondition> Conditions { get; private set; }

        /// <summary>
        /// Gets all criterion
        /// </summary>
        public List<Criterion> Criteria { get; private set; }

        /// <summary>
        /// Gets all sort
        /// </summary>
        public List<SortEntry> Sorts { get; private set; }

        /// <summary>
        /// Get the selected fields
        /// </summary>
        public List<ISixnetField> SelectedFields { get; private set; }

        /// <summary>
        /// Get the unselected fields
        /// </summary>
        public List<ISixnetField> UnselectedFields { get; private set; }

        /// <summary>
        /// Gets the group fields
        /// </summary>
        public List<ISixnetField> GroupFields { get; private set; }

        /// <summary>
        /// Gets the script
        /// </summary>
        public string Script { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the script parameters
        /// </summary>
        public Dictionary<string, object> ScriptParameters { get; private set; }

        /// <summary>
        /// Gets the script type
        /// </summary>
        public DataScriptType ScriptType { get; private set; } = DataScriptType.Text;

        /// <summary>
        /// Gets the query execution mode
        /// </summary>
        public QueryableExecutionMode ExecutionMode { get; private set; } = QueryableExecutionMode.Regular;

        /// <summary>
        /// Gets or sets the take data count
        /// </summary>
        public int TakeCount { get; private set; } = 0;

        /// <summary>
        /// Gets or sets the skip data count
        /// </summary>
        public int SkipCount { get; private set; } = 0;

        /// <summary>
        /// Indicates whether has subquery
        /// </summary>
        public bool HasSubQueryable { get; private set; }

        /// <summary>
        /// Whether has tree matching
        /// </summary>
        public bool HasTreeMatching { get; private set; }

        /// <summary>
        /// Indicates whether has join
        /// </summary>
        public bool HasJoin { get; private set; }

        /// <summary>
        /// Indicates whether has combine
        /// </summary>
        public bool HasCombine { get; private set; }

        /// <summary>
        /// Indicates whether has field converter
        /// </summary>
        public bool HasFieldFormatter { get; private set; }

        /// <summary>
        /// Indicates whether is a complex query
        /// Include subquery,tree,join
        /// </summary>
        public bool IsComplex => GetIsComplexQueryable();

        /// <summary>
        /// Gets the tree info
        /// </summary>
        public TreeMatchingInfo TreeInfo { get; private set; }

        /// <summary>
        /// Gets the joins
        /// </summary>
        public List<JoinEntry> Joins { get; private set; }

        /// <summary>
        /// Gets the combines
        /// </summary>
        public List<CombineEntry> Combines { get; private set; }

        /// <summary>
        /// Indecats whether are no conditions included
        /// </summary>
        public bool NoneCondition => Conditions.IsNullOrEmpty() && Joins.IsNullOrEmpty() && Combines.IsNullOrEmpty();

        /// <summary>
        /// Gets or sets the data isolation level
        /// </summary>
        public DataIsolationLevel? IsolationLevel { get; private set; }

        /// <summary>
        /// Gets or sets the connection operator
        /// </summary>
        public CriterionConnector Connector { get; internal set; } = CriterionConnector.And;

        /// <summary>
        /// Gets the from type
        /// </summary>
        public QueryableFromType FromType { get; private set; } = QueryableFromType.Table;

        /// <summary>
        /// Gets the target queryable
        /// </summary>
        public ISixnetQueryable Target { get; private set; }

        /// <summary>
        /// Gets the output type
        /// </summary>
        public QueryableOutputType OutputType { get; private set; } = QueryableOutputType.Data;

        /// <summary>
        /// Gets or sets the repository
        /// </summary>
        public ISixnetRepository Repository { get; private set; }

        /// <summary>
        /// Gets or sets the having queryable
        /// </summary>
        public ISixnetQueryable HavingQueryable { get; private set; }

        /// <summary>
        /// Gets or sets the split table behavior
        /// </summary>
        public SplitTableBehavior SplitTableBehavior { get; private set; }

        /// <summary>
        /// Whether is distinct
        /// </summary>
        public bool IsDistincted { get; private set; }

        /// <summary>
        /// Whether negation
        /// </summary>
        public bool Negation { get; private set; }

        /// <summary>
        /// Whether si read only
        /// </summary>
        public bool IsReadOnly { get; private set; }

        #endregion

        #region Methods

        #region Condition

        /// <summary>
        /// Add criterion
        /// </summary>
        /// <param name="connector">Connector</param>
        /// <param name="field">Field</param>
        /// <param name="criterionOperator">Criterion operator</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        internal QueryableContext AddCriterion(CriterionConnector connector, ISixnetField field, CriterionOperator criterionOperator, dynamic value, CriterionOptions criterionOptions = null)
        {
            SixnetDirectThrower.ThrowArgErrorIf(field == null, nameof(field));

            var newCriterion = Criterion.Create(criterionOperator, field, value, connector, criterionOptions);
            return AddCondition(newCriterion);
        }

        /// <summary>
        /// Add condition
        /// </summary>
        /// <param name="condition">Condition</param>
        internal QueryableContext AddCondition(ISixnetCondition condition)
        {
            #region Handle condition

            if (condition is Criterion criterion)
            {
                SetHasFieldFormatter(HasFieldFormatter || criterion.HasFieldFormatter());

                #region Left field

                if (criterion.Left is QueryableField leftQueryableField)
                {
                    leftQueryableField.Queryable = HandleParameterQueryable(leftQueryableField.Queryable);
                    AddSubQueryable(leftQueryableField.Queryable);
                }

                #endregion

                #region Right field

                if (criterion.Right is QueryableField rightQueryableField)
                {
                    rightQueryableField.Queryable = HandleParameterQueryable(rightQueryableField.Queryable);
                    AddSubQueryable(rightQueryableField.Queryable);
                }

                #endregion

                Criteria ??= new List<Criterion>();
                Criteria.Add(criterion);
            }
            else if (condition is ISixnetQueryable groupQueryable)
            {
                condition = HandleParameterQueryable(groupQueryable);
                AddGroupQueryable(groupQueryable);
            }

            #endregion

            if (condition != null)
            {
                // Clear validation function
                _validationFuncDict?.Clear();
                Conditions ??= new List<ISixnetCondition>();
                Conditions.Add(condition);
            }

            return this;
        }

        #endregion

        #region Sort

        /// <summary>
        /// Clear sort condition
        /// </summary>
        /// <returns></returns>
        internal QueryableContext ClearSort()
        {
            Sorts?.Clear();
            return this;
        }

        /// <summary>
        /// Add sort
        /// </summary>
        /// <param name="sortEntries">Sort entries</param>
        /// <returns></returns>
        internal QueryableContext AddSort(params SortEntry[] sortEntries)
        {
            if (!sortEntries.IsNullOrEmpty())
            {
                Sorts ??= new List<SortEntry>();
                Sorts.AddRange(sortEntries);
            }
            return this;
        }

        #endregion

        #region Fields

        /// <summary>
        /// Set select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        internal QueryableContext SelectFields(params ISixnetField[] fields)
        {
            if (!fields.IsNullOrEmpty())
            {
                SelectedFields ??= new List<ISixnetField>();
                SelectedFields.AddRange(fields);
                ClearCacheFields();
            }
            return this;
        }

        /// <summary>
        /// Set select fields
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        internal QueryableContext SelectFields<T>(params Expression<Func<T, dynamic>>[] fields)
        {
            return SelectFields(fields?.Select(f => SixnetExpressionHelper.GetDataField(f.Body)).ToArray());
        }

        /// <summary>
        /// Clear selected fields
        /// </summary>
        /// <returns></returns>
        internal QueryableContext ClearSelectedFields()
        {
            SelectedFields?.Clear();
            ClearCacheFields();
            return this;
        }

        /// <summary>
        /// Set unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        internal QueryableContext UnselectFields(params ISixnetField[] fields)
        {
            if (!fields.IsNullOrEmpty())
            {
                UnselectedFields ??= new List<ISixnetField>();
                UnselectedFields.AddRange(fields);
                ClearCacheFields();
            }
            return this;
        }

        /// <summary>
        /// Set unselect fields
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        internal QueryableContext UnselectFields<T>(params Expression<Func<T, object>>[] fields)
        {
            return UnselectFields(fields?.Select(f => SixnetExpressionHelper.GetDataField(f.Body)).ToArray());
        }

        /// <summary>
        /// Clear unselected fields
        /// </summary>
        /// <returns></returns>
        internal QueryableContext ClearUnselectedFields()
        {
            UnselectedFields?.Clear();
            ClearCacheFields();
            return this;
        }

        /// <summary>
        /// Get the finally fields
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <param name="includeNecessary">Whether include the necessary fields</param>
        /// <returns></returns>
        internal List<ISixnetField> GetFinallyFields(Type modelType, bool includeNecessary)
        {
            string fieldsCacheKey = $"{modelType?.GUID ?? _nullableEntityId}_{(includeNecessary ? 1 : 0)}";
            if (!_finallyFieldsCache.TryGetValue(fieldsCacheKey, out var finallyFields) || (finallyFields?.IsNullOrEmpty() ?? true))
            {
                finallyFields = SelectedFields ?? new List<ISixnetField>();
                var hasSelectFields = !(SelectedFields?.IsNullOrEmpty() ?? true);
                var hasUnselectFields = !(UnselectedFields?.IsNullOrEmpty() ?? true);
                includeNecessary &= (hasSelectFields || hasUnselectFields);
                if (!hasSelectFields)
                {
                    finallyFields.AddRange(SixnetEntityManager.GetQueryableFields(modelType));
                    if (hasUnselectFields)
                    {
                        finallyFields = finallyFields.Except(UnselectedFields, SixnetFieldComparer.DefaultComparer).ToList();
                    }
                }
                if (includeNecessary)
                {
                    var necessaryFields = SixnetEntityManager.GetNecessaryFields(modelType);
                    if (!necessaryFields.IsNullOrEmpty())
                    {
                        finallyFields = finallyFields.Union(necessaryFields, SixnetFieldComparer.DefaultComparer).ToList();
                    }
                }
                _finallyFieldsCache[fieldsCacheKey] = finallyFields;
            }
            return finallyFields;
        }

        /// <summary>
        /// Exclude necessary fields
        /// </summary>
        /// <returns></returns>
        internal QueryableContext ExcludeNecessaryFields()
        {
            IsExcludedNecessaryFields = true;
            ClearCacheFields();
            return this;
        }

        /// <summary>
        /// Include necessary fields
        /// </summary>
        /// <returns></returns>
        internal QueryableContext IncludeNecessaryFields()
        {
            IsExcludedNecessaryFields = false;
            ClearCacheFields();
            return this;
        }

        /// <summary>
        /// Clear cache fields
        /// </summary>
        void ClearCacheFields()
        {
            _finallyFieldsCache?.Clear();
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
        internal QueryableContext SetScript(string script, DataScriptType scriptType = DataScriptType.Text, object parameters = null)
        {
            Script = script;
            ScriptType = scriptType;
            ScriptParameters = parameters?.ToObjectDictionary();
            ExecutionMode = QueryableExecutionMode.Script;
            return this;
        }

        #endregion

        #region Validation function

        /// <summary>
        /// Get validation function
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <returns>Return a validation function</returns>
        internal Func<T, bool> GetValidationFunction<T>()
        {
            var modelType = typeof(T);
            _validationFuncDict ??= new Dictionary<Guid, dynamic>();
            if (_validationFuncDict.ContainsKey(modelType.GUID))
            {
                return _validationFuncDict[modelType.GUID];
            }
            if (IsComplex || Conditions.IsNullOrEmpty())
            {
                Func<T, bool> trueFunc = (data) => true;
                _validationFuncDict.Add(modelType.GUID, trueFunc);
                return trueFunc;
            }
            var funcType = SixnetEntityManager.GetEntityPredicateType(modelType);
            var parExp = Expression.Parameter(modelType);
            var parameterArray = Array.CreateInstance(typeof(ParameterExpression), 1);
            parameterArray.SetValue(parExp, 0);
            Expression conditionExpression = null;
            foreach (var conditionEntry in Conditions)
            {
                Expression childExpression = GenerateExpression(parExp, conditionEntry);
                if (childExpression == null)
                {
                    continue;
                }
                if (conditionExpression == null)
                {
                    conditionExpression = childExpression;
                    continue;
                }
                if (conditionEntry.Connector == CriterionConnector.And)
                {
                    conditionExpression = Expression.AndAlso(conditionExpression, childExpression);
                }
                else
                {
                    conditionExpression = Expression.OrElse(conditionExpression, childExpression);
                }
            }
            if (conditionExpression == null)
            {
                return null;
            }
            var genericLambdaMethod = SixnetReflecter.Expression.LambdaMethod.MakeGenericMethod(funcType);
            var lambdaExpression = genericLambdaMethod.Invoke(null, new object[]
            {
                conditionExpression,parameterArray
            });
            Func<T, bool> func = ((Expression<Func<T, bool>>)lambdaExpression).Compile();
            _validationFuncDict.Add(modelType.GUID, func);
            return func;
        }

        /// <summary>
        /// Generate expressionn
        /// </summary>
        /// <param name="parameter">Parameter expression</param>
        /// <param name="condition">Condition</param>
        /// <returns>Return query expression</returns>
        Expression GenerateExpression(Expression parameter, ISixnetCondition condition)
        {
            if (condition is Criterion)
            {
                return GenerateSingleExpression(parameter, condition as Criterion);
            }
            var query = condition as ISixnetQueryable;
            if (query?.Conditions.IsNullOrEmpty() ?? true)
            {
                return null;
            }
            var conditionCount = query.Conditions.Count();
            var firstCondition = query.Conditions.First();
            if (conditionCount == 1 && firstCondition is Criterion firstCriterion)
            {
                return GenerateSingleExpression(parameter, firstCriterion);
            }
            Expression conditionExpression = null;
            foreach (var conditionEntry in query.Conditions)
            {
                var childExpression = GenerateExpression(parameter, conditionEntry);
                if (conditionExpression == null)
                {
                    conditionExpression = childExpression;
                    continue;
                }
                if (conditionEntry.Connector == CriterionConnector.And)
                {
                    conditionExpression = Expression.AndAlso(conditionExpression, childExpression);
                }
                else
                {
                    conditionExpression = Expression.OrElse(conditionExpression, childExpression);
                }
            }
            return conditionExpression;
        }

        /// <summary>
        /// Generate a single expression
        /// </summary>
        /// <param name="parameter">Parameter expression</param>
        /// <param name="criterion">Criterion</param>
        /// <returns>Return query expression</returns>
        Expression GenerateSingleExpression(Expression parameter, Criterion criterion)
        {
            //object criteriaValue = criterion.Value;
            //Expression valueExpression = Expression.Constant(criteriaValue, criteriaValue?.GetType() ?? typeof(object));
            //Expression property = string.IsNullOrWhiteSpace(criterion.Left?.FieldName) ? null : Expression.PropertyOrField(parameter, criterion.Left.FieldName);
            //Expression valueExpression = GetValueExpression(property, criterion);
            var leftField = criterion.Left;
            var rightField = criterion.Right;
            var leftExp = GetFieldExpression(parameter, leftField);
            var rightExp = GetFieldExpression(parameter, rightField);
            var propertyExp = leftExp;
            var valueExp = rightExp;
            object criterionValue = null;
            if (leftField is ConstantField leftConstantField)
            {
                propertyExp = rightExp;
                valueExp = leftExp;
                criterionValue = leftConstantField.Value;
            }
            if (rightField is ConstantField rightConstantField)
            {
                criterionValue = rightConstantField.Value;
            }
            var valueType = criterionValue?.GetType();
            var valueIsNotCollection = valueType == null || !SixnetReflecter.Collections.IsCollectionType(valueType);
            if (leftExp != null && leftExp.NodeType == ExpressionType.MemberAccess && leftExp.Type.IsEnum && valueIsNotCollection)
            {
                leftExp = Expression.Convert(leftExp, criterionValue.GetType());
            }
            if (rightExp != null && rightExp.NodeType == ExpressionType.MemberAccess && rightExp.Type.IsEnum && valueIsNotCollection)
            {
                rightExp = Expression.Convert(rightExp, criterionValue.GetType());
            }

            Expression conditionExp = null;
            switch (criterion.Operator)
            {
                case CriterionOperator.False:
                    conditionExp = Expression.Constant(false, typeof(bool));
                    break;
                case CriterionOperator.True:
                    conditionExp = Expression.Constant(true, typeof(bool));
                    break;
                case CriterionOperator.Equal:
                    conditionExp = Expression.Equal(leftExp, rightExp);
                    break;
                case CriterionOperator.IsNull:
                    conditionExp = leftExp ?? rightExp;
                    if (conditionExp == null)
                    {
                        conditionExp = Expression.Constant(true, typeof(bool));
                    }
                    else if (conditionExp.Type.AllowNull())
                    {
                        conditionExp = Expression.Equal(conditionExp, Expression.Constant(null, typeof(object)));
                    }
                    else
                    {
                        conditionExp = Expression.Constant(false, typeof(bool));
                    }
                    break;
                case CriterionOperator.NotEqual:
                    conditionExp = Expression.NotEqual(leftExp, rightExp);
                    break;
                case CriterionOperator.NotNull:
                    conditionExp = leftExp ?? rightExp;
                    if (conditionExp == null)
                    {
                        conditionExp = Expression.Constant(false, typeof(bool));
                    }
                    else if (conditionExp.Type.AllowNull())
                    {
                        conditionExp = Expression.NotEqual(conditionExp, Expression.Constant(null, typeof(object)));
                    }
                    else
                    {
                        conditionExp = Expression.Constant(true, typeof(bool));
                    }
                    break;
                case CriterionOperator.GreaterThan:
                    conditionExp = Expression.GreaterThan(leftExp, rightExp);
                    break;
                case CriterionOperator.GreaterThanOrEqual:
                    conditionExp = Expression.GreaterThanOrEqual(leftExp, rightExp);
                    break;
                case CriterionOperator.LessThan:
                    conditionExp = Expression.LessThan(leftExp, rightExp);
                    break;
                case CriterionOperator.LessThanOrEqual:
                    conditionExp = Expression.LessThanOrEqual(leftExp, rightExp);
                    break;
                case CriterionOperator.BeginLike:
                    var beginLikeExpression = Expression.Call(propertyExp, SixnetReflecter.String.StringIndexOfMethod, valueExp);
                    conditionExp = Expression.Equal(beginLikeExpression, Expression.Constant(0));
                    break;
                case CriterionOperator.Like:
                    var likeExpression = Expression.Call(propertyExp, SixnetReflecter.String.StringIndexOfMethod, valueExp);
                    conditionExp = Expression.GreaterThanOrEqual(likeExpression, Expression.Constant(0));
                    break;
                case CriterionOperator.EndLike:
                    var endLikeExpression = Expression.Call(propertyExp, SixnetReflecter.String.EndWithMethod, valueExp);
                    conditionExp = Expression.Equal(endLikeExpression, Expression.Constant(true));
                    break;
                case CriterionOperator.In:
                    SixnetDirectThrower.ThrowArgNullIf(criterionValue == null, "Criterion value is null");
                    if (valueType != null && valueType.GenericTypeArguments != null && valueType.GenericTypeArguments.Length > 0)
                    {
                        valueType = valueType.GenericTypeArguments[valueType.GenericTypeArguments.Length - 1];
                    }
                    else if (valueType.IsArray)
                    {
                        var arrayValue = criterionValue as Array;
                        if (arrayValue != null && arrayValue.Length > 0)
                        {
                            valueType = arrayValue.GetValue(0).GetType();
                        }
                        else
                        {
                            valueType = typeof(object);
                        }
                    }
                    else
                    {
                        valueType = typeof(object);
                    }
                    var inMethod = SixnetReflecter.Collections.GetCollectionContainsMethod(valueType);
                    conditionExp = Expression.Call(inMethod, valueExp, propertyExp);
                    break;
                case CriterionOperator.NotIn:
                    SixnetDirectThrower.ThrowArgNullIf(criterionValue == null, "Criterion value is null");
                    if (valueType != null && valueType.GenericTypeArguments != null)
                    {
                        valueType = valueType.GenericTypeArguments[0];
                    }
                    else
                    {
                        valueType = typeof(object);
                    }
                    var notInMethod = SixnetReflecter.Collections.GetCollectionContainsMethod(valueType);
                    conditionExp = Expression.Not(Expression.Call(notInMethod, valueExp, propertyExp));
                    break;
                default:
                    conditionExp = null;
                    break;
            }
            return conditionExp;
        }

        /// <summary>
        /// Get field expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        Expression GetFieldExpression(Expression parameter, ISixnetField field)
        {
            if (field == null)
            {
                return null;
            }
            if (field is DataField regularField)
            {
                return string.IsNullOrWhiteSpace(regularField.PropertyName) ? null : Expression.PropertyOrField(parameter, regularField.PropertyName);
            }
            if (field is ConstantField constantField)
            {
                var value = constantField.Value;
                var valueType = value?.GetType() ?? typeof(object);
                return Expression.Constant(value, valueType);
            }
            return null;
        }

        #endregion

        #region Tree

        /// <summary>
        /// Tree matching
        /// </summary>
        /// <param name="dataFieldName">Data field name</param>
        /// <param name="parentFieldName">Parent field name</param>
        /// <param name="direction">Recurve direction</param>
        /// <returns></returns>
        internal QueryableContext TreeMatching(string dataFieldName, string parentFieldName, TreeMatchingDirection direction = TreeMatchingDirection.Down)
        {
            SixnetDirectThrower.ThrowArgErrorIf(string.IsNullOrWhiteSpace(dataFieldName) || string.IsNullOrWhiteSpace(parentFieldName), $"{nameof(dataFieldName)} or {nameof(parentFieldName)} is null or empty");
            return TreeMatching(DataField.Create(dataFieldName), DataField.Create(parentFieldName), direction);
        }

        /// <summary>
        /// Tree matching
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        internal QueryableContext TreeMatching(ISixnetField dataField, ISixnetField parentField, TreeMatchingDirection direction = TreeMatchingDirection.Down)
        {
            SixnetDirectThrower.ThrowArgErrorIf(dataField == null || parentField == null, $"{nameof(dataField)} or {nameof(parentField)} is null");
            SixnetDirectThrower.ThrowArgErrorIf(dataField == parentField, $"{nameof(dataField)} and {nameof(parentField)} can not be the same value");

            TreeInfo = new TreeMatchingInfo()
            {
                DataField = dataField,
                ParentField = parentField,
                Direction = direction
            };

            return this;
        }

        #endregion

        #region Clone

        /// <summary>
        /// Light clone an IQuery object
        /// </summary>
        /// <returns>Return the replicated Queryable</returns>
        public QueryableContext LightClone()
        {
            var newQueryableContext = CloneValueMember();
            newQueryableContext.IgnoredFilterTypes = IgnoredFilterTypes == null ? null : new HashSet<Type>(IgnoredFilterTypes);
            newQueryableContext.Conditions = Conditions == null ? null : new List<ISixnetCondition>(Conditions);
            newQueryableContext.Criteria = Criteria == null ? null : new List<Criterion>(Criteria);
            newQueryableContext.Sorts = Sorts == null ? null : new List<SortEntry>(Sorts);
            newQueryableContext.SelectedFields = SelectedFields == null ? null : new List<ISixnetField>(SelectedFields);
            newQueryableContext.UnselectedFields = UnselectedFields == null ? null : new List<ISixnetField>(UnselectedFields);
            newQueryableContext.GroupFields = GroupFields == null ? null : new List<ISixnetField>(GroupFields);
            newQueryableContext.ScriptParameters = ScriptParameters?.ToDictionary(c => c.Key, c => c.Value);
            newQueryableContext.TreeInfo = TreeInfo;
            newQueryableContext.Joins = Joins == null ? null : new List<JoinEntry>(Joins);
            newQueryableContext.Combines = Combines == null ? null : new List<CombineEntry>(Combines);
            newQueryableContext.Target = Target?.LightClone();
            newQueryableContext.Repository = Repository;
            newQueryableContext.HavingQueryable = HavingQueryable?.LightClone();
            newQueryableContext.SplitTableBehavior = SplitTableBehavior;
            newQueryableContext._finallyFieldsCache = new Dictionary<string, List<ISixnetField>>(_finallyFieldsCache);
            newQueryableContext._validationFuncDict = new Dictionary<Guid, dynamic>(_validationFuncDict);
            return newQueryableContext;
        }

        /// <summary>
        /// Clone a IQuery object
        /// </summary>
        /// <returns>Return the replicated Queryable</returns>
        public QueryableContext Clone()
        {
            var newQueryableContext = CloneValueMember();
            newQueryableContext.IgnoredFilterTypes = IgnoredFilterTypes == null ? null : new HashSet<Type>(IgnoredFilterTypes);
            newQueryableContext.Conditions = Conditions?.Select(c => c?.Clone()).ToList();
            newQueryableContext.Criteria = Criteria?.Select(c => c.Clone()).ToList();
            newQueryableContext.Sorts = Sorts?.Select(c => c?.Clone()).ToList();
            newQueryableContext.SelectedFields = SelectedFields?.Select(c => c.Clone()).ToList();
            newQueryableContext.UnselectedFields = UnselectedFields?.Select(c => c.Clone()).ToList();
            newQueryableContext.GroupFields = GroupFields?.Select(c => c.Clone()).ToList();
            newQueryableContext.ScriptParameters = ScriptParameters?.ToDictionary(c => c.Key, c => c.Value);
            newQueryableContext.TreeInfo = TreeInfo?.Clone();
            newQueryableContext.Joins = Joins?.Select(c => c.Clone()).ToList();
            newQueryableContext.Combines = Combines?.Select(c => c.Clone()).ToList();
            newQueryableContext.Target = Target?.Clone();
            newQueryableContext.Repository = Repository;
            newQueryableContext.HavingQueryable = HavingQueryable?.Clone();
            newQueryableContext.SplitTableBehavior = SplitTableBehavior?.Clone();
            newQueryableContext._finallyFieldsCache = _finallyFieldsCache?.ToDictionary(f => f.Key, f => f.Value?.Select(fv => fv.Clone()).ToList());
            newQueryableContext._validationFuncDict = new Dictionary<Guid, dynamic>(_validationFuncDict);
            return newQueryableContext;
        }

        /// <summary>
        /// Clone value member
        /// </summary>
        /// <returns></returns>
        QueryableContext CloneValueMember()
        {
            var newQueryableContext = new QueryableContext
            {
                Id = Guid.NewGuid(),
                IsExcludedNecessaryFields = IsExcludedNecessaryFields,
                ModelType = ModelType,
                IgnoredFilterFieldRole = IgnoredFilterFieldRole,
                _joinIndex = _joinIndex,
                Script = Script,
                ScriptType = ScriptType,
                ExecutionMode = ExecutionMode,
                TakeCount = TakeCount,
                SkipCount = SkipCount,
                HasSubQueryable = HasSubQueryable,
                HasTreeMatching = HasTreeMatching,
                HasJoin = HasJoin,
                HasCombine = HasCombine,
                HasFieldFormatter = HasFieldFormatter,
                IsolationLevel = IsolationLevel,
                Connector = Connector,
                FromType = FromType,
                OutputType = OutputType,
                IsDistincted = IsDistincted,
                Negation = Negation,
                IsReadOnly = IsReadOnly
            };
            return newQueryableContext;
        }

        #endregion

        #region Join

        /// <summary>
        /// Add join
        /// </summary>
        /// <param name="joinEntry">Join entry</param>
        /// <returns></returns>
        internal QueryableContext Join(JoinEntry joinEntry, Action<JoinEntry> configure = null)
        {
            if (joinEntry?.Target != null)
            {
                HasJoin = true;
                joinEntry.Target = HandleParameterQueryable(joinEntry.Target);
                joinEntry.Index = _joinIndex++;

                // default join connection
                Dictionary<string, string> joinFields = null;
                if (joinEntry.Connection == null)
                {
                    var sourceModelType = ModelType;
                    var targetModelType = joinEntry.Target.GetModelType();
                    if (sourceModelType == targetModelType)
                    {
                        var primaryKeys = SixnetEntityManager.GetPrimaryKeyNames(sourceModelType);
                        SixnetDirectThrower.ThrowSixnetExceptionIf(primaryKeys.IsNullOrEmpty(), $"{sourceModelType.FullName} not set primary key");
                        joinFields = primaryKeys.ToDictionary(c => c, c => c);
                    }
                    else
                    {
                        joinFields = SixnetEntityManager.GetRelationFieldNames(sourceModelType, targetModelType);
                    }
                    if (!joinFields.IsNullOrEmpty())
                    {
                        var connectionQueryable = SixnetQuerier.Create();
                        foreach (var joinFieldEntry in joinFields)
                        {
                            connectionQueryable = connectionQueryable.Where(Criterion.Create(CriterionOperator.Equal, DataField.Create(joinFieldEntry.Key, sourceModelType), DataField.Create(joinFieldEntry.Value, targetModelType, joinEntry.Index)));
                        }
                        joinEntry.Connection = connectionQueryable;
                    }
                }
                else
                {
                    joinEntry.Connection = HandleParameterQueryable(joinEntry.Connection);
                }
                configure?.Invoke(joinEntry);
                Joins ??= new List<JoinEntry>();
                Joins.Add(joinEntry);
            }
            return this;
        }

        #endregion

        #region Combine

        /// <summary>
        /// Add combine
        /// </summary>
        /// <param name="combineEntry">Combine entry</param>
        /// <returns></returns>
        internal QueryableContext Combine(CombineEntry combineEntry)
        {
            if (combineEntry == null)
            {
                return this;
            }
            combineEntry.Target = HandleParameterQueryable(combineEntry.Target);
            Combines ??= new List<CombineEntry>();
            Combines.Add(combineEntry);
            HasCombine = true;
            return this;
        }

        #endregion

        #region Distinct

        /// <summary>
        /// Distinct
        /// </summary>
        /// <returns></returns>
        internal QueryableContext Distinct()
        {
            IsDistincted = true;
            return this;
        }

        #endregion

        #region Model Type

        ///// <summary>
        ///// Get the model type
        ///// </summary>
        ///// <returns></returns>
        //internal Type GetModelType()
        //{
        //    return ModelType;
        //}

        /// <summary>
        /// Set model type
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <returns></returns>
        internal QueryableContext SetModelType(Type modelType)
        {
            ModelType = modelType ?? DefaultModelType;
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
        internal QueryableContext Take(int count, int skip = 0)
        {
            TakeCount = count;
            SkipCount = skip;
            return this;
        }

        /// <summary>
        /// Set take data by paging
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns></returns>
        internal QueryableContext TakeByPaging(int pageIndex, int pageSize)
        {
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            if (pageSize < 1)
            {
                pageSize = SixnetDataManager.GetDefaultPagingSize();
            }

            return Take(pageSize, (pageIndex - 1) * pageSize);
        }

        /// <summary>
        /// Set take data by paging
        /// </summary>
        /// <param name="pagingFilter">Paging filter</param>
        /// <returns></returns>
        internal QueryableContext TakeByPaging(PagingFilter pagingFilter)
        {
            if (pagingFilter == null)
            {
                return TakeByPaging(1, SixnetDataManager.GetDefaultPagingSize());
            }
            return TakeByPaging(pagingFilter.Page, pagingFilter.PageSize);
        }

        /// <summary>
        /// Get paging filter
        /// </summary>
        /// <returns></returns>
        internal PagingFilter GetPagingFilter()
        {
            if (SkipCount < 0)
            {
                SkipCount = 0;
            }
            if (TakeCount < 1)
            {
                TakeCount = SixnetDataManager.GetDefaultPagingSize();
            }
            return new PagingFilter()
            {
                PageSize = TakeCount,
                Page = SkipCount / TakeCount + 1
            };
        }

        #endregion

        #region Group

        /// <summary>
        /// Add group fields
        /// </summary>
        /// <param name="fieldNames">Group field names</param>
        /// <returns></returns>
        internal QueryableContext GroupBy(params string[] fieldNames)
        {
            return GroupBy(fieldNames?.Select(fn => DataField.Create(fn)).ToArray());
        }

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        internal QueryableContext GroupBy(params ISixnetField[] fields)
        {
            if (!fields.IsNullOrEmpty())
            {
                GroupFields ??= new List<ISixnetField>();
                GroupFields.AddRange(fields);
            }
            return this;
        }

        #endregion

        #region From

        /// <summary>
        /// From other queryable
        /// </summary>
        /// <param name="targetQueryable">Target queryable</param>
        /// <returns></returns>
        internal QueryableContext FromQueryable(ISixnetQueryable targetQueryable)
        {
            FromType = QueryableFromType.Queryable;
            Target = HandleParameterQueryable(targetQueryable);
            return this;
        }

        #endregion

        #region Split table

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitTableBehavior">Split table behavior</param>
        /// <returns></returns>
        internal QueryableContext SplitTable(SplitTableBehavior splitTableBehavior)
        {
            if (splitTableBehavior != null)
            {
                Target = null;
                FromType = QueryableFromType.Table;
                SplitTableBehavior = splitTableBehavior;
            }
            return this;
        }

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitValues">Split values</param>
        /// <param name="selectionPattern">Selection pattern</param>
        /// <returns></returns>
        internal QueryableContext SplitTable(IEnumerable<dynamic> splitValues, SplitTableNameSelectionPattern selectionPattern = SplitTableNameSelectionPattern.Precision)
        {
            splitValues ??= Array.Empty<dynamic>();
            return SplitTable(new SplitTableBehavior()
            {
                SelectionPattern = selectionPattern,
                SplitValues = splitValues
            });
        }

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="tableNameFilter">Table name filter</param>
        /// <returns></returns>
        internal QueryableContext SplitTable(Func<IEnumerable<string>, IEnumerable<string>> tableNameFilter)
        {
            return SplitTable(new SplitTableBehavior()
            {
                SplitTableNameFilter = tableNameFilter
            });
        }

        #endregion

        #region Output

        /// <summary>
        /// Set output
        /// </summary>
        /// <param name="outputType">Output type</param>
        /// <returns></returns>
        internal QueryableContext Output(QueryableOutputType outputType)
        {
            OutputType = outputType;
            return this;
        }

        #endregion

        #region Filter

        /// <summary>
        /// Ignore field role filter
        /// </summary>
        /// <param name="fieldRoles"></param>
        /// <returns></returns>
        internal QueryableContext IgnoreFilter(params FieldRole[] fieldRoles)
        {
            if (!fieldRoles.IsNullOrEmpty())
            {
                foreach (var fieldRole in fieldRoles)
                {
                    IgnoredFilterFieldRole |= fieldRole;
                }
            }
            return this;
        }

        /// <summary>
        /// Ignore type filter
        /// </summary>
        /// <param name="types">Filter types</param>
        /// <returns></returns>
        internal QueryableContext IgnoreFilter(params Type[] types)
        {
            if (!types.IsNullOrEmpty())
            {
                IgnoredFilterTypes ??= new HashSet<Type>();
                foreach (var type in types)
                {
                    if (type != null)
                    {
                        IgnoredFilterTypes.Add(type);
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// Has ignored field role filter
        /// </summary>
        /// <param name="fieldRole"></param>
        /// <returns></returns>
        internal bool HasIgnoredFilter(FieldRole fieldRole)
        {
            return (IgnoredFilterFieldRole & fieldRole) == fieldRole;
        }

        /// <summary>
        /// Has ignored type filter
        /// </summary>
        /// <param name="filterType"></param>
        /// <returns></returns>
        internal bool HasIgnoredFilter(Type filterType)
        {
            if (filterType == null)
            {
                return true;
            }
            return IgnoredFilterTypes?.Contains(filterType) ?? false;
        }

        #endregion

        #region Having

        /// <summary>
        /// Add having queryable
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <returns></returns>
        internal QueryableContext Having(ISixnetQueryable queryable)
        {
            if (queryable != null)
            {
                if (HavingQueryable == null)
                {
                    HavingQueryable = queryable;
                }
                else
                {
                    HavingQueryable.Where(queryable);
                }
            }
            return this;
        }

        #endregion

        #region Negate

        /// <summary>
        /// Negate
        /// </summary>
        internal void Negate()
        {
            Negation = !Negation;
        }

        #endregion

        #region Read only

        /// <summary>
        /// Read only
        /// </summary>
        public void ReadOnly()
        {
            IsReadOnly = true;
        }

        #endregion

        #region Util

        /// <summary>
        /// Check whether is use for group queryable
        /// </summary>
        /// <returns></returns>
        internal bool CheckUseForGroup()
        {
            return ExecutionMode != QueryableExecutionMode.Script && ModelType == null;
        }

        /// <summary>
        /// Set has subquery
        /// </summary>
        /// <param name="hasSubQueryable">Has subquery</param>
        internal void SetHasSubQueryable(bool hasSubQueryable)
        {
            HasSubQueryable = hasSubQueryable;
        }

        /// <summary>
        /// Set has join
        /// </summary>
        /// <param name="hasJoin">Has join</param>
        internal void SetHasJoin(bool hasJoin)
        {
            HasJoin = hasJoin;
        }

        /// <summary>
        /// Set has tree matching
        /// </summary>
        /// <param name="hasTreeMatching">Has tree matching</param>
        internal void SetHasTreeMatching(bool hasTreeMatching)
        {
            HasTreeMatching = hasTreeMatching;
        }

        /// <summary>
        /// Set has combine items
        /// </summary>
        /// <param name="hasCombine">Has combine</param>
        internal void SetHasCombine(bool hasCombine)
        {
            HasCombine = hasCombine;
        }

        /// <summary>
        /// Set has field formatter
        /// </summary>
        /// <param name="hasFieldFormatter">Has field formatter</param>
        internal void SetHasFieldFormatter(bool hasFieldFormatter)
        {
            HasFieldFormatter = hasFieldFormatter;
        }

        /// <summary>
        /// Add sub queryable
        /// </summary>
        /// <param name="subQueryable">Sub queryable</param>
        /// <returns></returns>
        internal QueryableContext AddSubQueryable(ISixnetQueryable subQueryable)
        {
            if (subQueryable != null)
            {
                SetHasSubQueryable(true);
            }
            return this;
        }

        /// <summary>
        /// Add group queryable
        /// </summary>
        /// <param name="groupQueryable">Group queryable</param>
        /// <returns></returns>
        internal QueryableContext AddGroupQueryable(ISixnetQueryable groupQueryable)
        {
            if (groupQueryable != null && !groupQueryable.Criteria.IsNullOrEmpty())
            {
                Criteria ??= new List<Criterion>();
                Criteria.AddRange(groupQueryable.Criteria);
            }
            return this;
        }

        /// <summary>
        /// Merge queryable meta data
        /// </summary>
        /// <param name="targetQueryable">Target queryable</param>
        internal void MergeQueryableMetaData(ISixnetQueryable targetQueryable)
        {
            if (targetQueryable != null)
            {
                SetHasSubQueryable(HasSubQueryable || targetQueryable.HasSubquery);
                SetHasJoin(HasJoin || targetQueryable.HasJoin);
                SetHasTreeMatching(HasTreeMatching || targetQueryable.HasRecurve);
                SetHasFieldFormatter(HasFieldFormatter || targetQueryable.HasFieldFormatter);
            }
        }

        /// <summary>
        /// Gets whether is complex queryable object
        /// </summary>
        /// <returns></returns>
        bool GetIsComplexQueryable()
        {
            return HasSubQueryable || HasTreeMatching || HasJoin || HasCombine || HasFieldFormatter;
        }

        /// <summary>
        /// Handle parameter queryable
        /// </summary>
        /// <param name="parameterQueryable">Parameter queryable</param>
        /// <returns></returns>
        internal TQueryable HandleParameterQueryable<TQueryable>(TQueryable parameterQueryable) where TQueryable : ISixnetQueryable
        {
            if (parameterQueryable != null)
            {
                MergeQueryableMetaData(parameterQueryable);
                parameterQueryable = (TQueryable)parameterQueryable.Clone();
            }
            return parameterQueryable;
        }

        #endregion

        #endregion
    }
}
