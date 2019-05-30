using EZNEW.Framework.Paging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using EZNEW.Framework.ExpressionUtil;
using EZNEW.Develop.CQuery.CriteriaConvert;
using EZNEW.Framework.Extension;
using EZNEW.Develop.Entity;

namespace EZNEW.Develop.CQuery
{
    /// <summary>
    /// Condition Implement
    /// </summary>
    internal class QueryInfo : IQuery
    {
        /// <summary>
        /// all criterias
        /// </summary>
        List<Tuple<QueryOperator, IQueryItem>> criterias = new List<Tuple<QueryOperator, IQueryItem>>();
        List<OrderCriteria> orders = new List<OrderCriteria>();//order items
        List<string> queryFields = new List<string>();//specify query fields
        List<string> notQueryFields = new List<string>();//specify not query fields
        Dictionary<string, bool> loadPropertys = new Dictionary<string, bool>();//allow lazy load propertys
        List<Criteria> equalCriteriaList = new List<Criteria>();//equal criterias
        static MethodInfo lambdaMethod = null;
        static MethodInfo stringIndexOfMethod = null;
        static MethodInfo endWithMethod = null;
        static MethodInfo collectionContainsMethod = null;
        Dictionary<Guid, dynamic> queryExpressionDict = new Dictionary<Guid, dynamic>();

        #region Constructor

        /// <summary>
        /// Create a query instance
        /// </summary>
        /// <param name="objectName">object name</param>
        internal QueryInfo()
        {
        }

        /// <summary>
        /// static constructor
        /// </summary>
        static QueryInfo()
        {
            var baseExpressMethods = typeof(Expression).GetMethods(BindingFlags.Public | BindingFlags.Static);
            lambdaMethod = baseExpressMethods.FirstOrDefault(c => c.Name == "Lambda" && c.IsGenericMethod && c.GetParameters()[1].ParameterType.FullName == typeof(ParameterExpression[]).FullName);
            stringIndexOfMethod = typeof(string).GetMethods().FirstOrDefault(c => c.Name == "IndexOf" && c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType.FullName == typeof(string).FullName);
            endWithMethod = typeof(string).GetMethods().FirstOrDefault(c => c.Name == "EndsWith" && c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType.FullName == typeof(string).FullName);
            collectionContainsMethod = typeof(Enumerable).GetMethods().FirstOrDefault(c => c.Name == "Contains" && c.GetParameters().Length == 2);
        }

        #endregion

        #region Propertys

        /// <summary>
        /// all criterias
        /// </summary>
        public List<Tuple<QueryOperator, IQueryItem>> Criterias
        {
            get
            {
                return criterias;
            }
            internal set
            {
                criterias = value ?? new List<Tuple<QueryOperator, IQueryItem>>();
            }
        }

        /// <summary>
        /// all orders
        /// </summary>
        public List<OrderCriteria> Orders
        {
            get
            {
                return orders;
            }
            internal set
            {
                orders = value ?? new List<OrderCriteria>();
            }
        }

        /// <summary>
        /// specific query fields(it's priority greater than NoQueryFields)
        /// </summary>
        public List<string> QueryFields
        {
            get
            {
                return queryFields;
            }
            internal set
            {
                queryFields = value ?? new List<string>();
            }
        }

        /// <summary>
        /// specific not query fields(it's priority less than QueryFields)
        /// </summary>
        public List<string> NotQueryFields
        {
            get
            {
                return notQueryFields;
            }
            internal set
            {
                notQueryFields = value ?? new List<string>();
            }
        }

        /// <summary>
        /// query model type
        /// </summary>
        public Type QueryModelType
        {
            get; set;
        }

        /// <summary>
        /// paging
        /// </summary>
        public PagingFilter PagingInfo { get; set; } = null;

        /// <summary>
        /// query text
        /// </summary>
        public string QueryText { get; internal set; } = string.Empty;

        /// <summary>
        /// query text parameter
        /// </summary>
        public dynamic QueryTextParameters { get; internal set; } = null;

        /// <summary>
        /// query command type
        /// </summary>
        public QueryCommandType QueryType { get; internal set; } = QueryCommandType.QueryObject;

        /// <summary>
        /// return size
        /// </summary>
        public int QuerySize { get; set; } = 0;

        /// <summary>
        /// Allow Load Propertys
        /// </summary>
        public Dictionary<string, bool> LoadPropertys
        {
            get
            {
                return loadPropertys;
            }
            internal set
            {
                loadPropertys = value ?? new Dictionary<string, bool>();
            }
        }

        /// <summary>
        /// Has Sub Query
        /// </summary>
        public bool HasSubQuery { get; private set; } = false;

        /// <summary>
        /// Recurve Criteria
        /// </summary>
        public RecurveCriteria RecurveCriteria
        {
            get; private set;
        }

        /// <summary>
        /// Verify Result Method
        /// </summary>
        public Func<int, bool> VerifyResult { get; set; } = null;

        /// <summary>
        /// direct return if query is obsolete
        /// </summary>
        public bool IsObsolete { get; internal set; } = false;

        #endregion

        #region Functions

        #region And

        /// <summary>
        /// Connect a condition with 'and'
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="operator">condition operator</param>
        /// <param name="value">value</param>
        /// <returns>return newest instance</returns>
        public IQuery And(string fieldName, CriteriaOperator @operator, dynamic value, ICriteriaConvert convert = null)
        {
            AddCriteria(QueryOperator.AND, fieldName, @operator, value);
            return this;
        }

        /// <summary>
        /// Connect a condition with 'and'
        /// </summary>
        /// <param name="eachFieldConnectOperator">each field connect operator</param>
        /// <param name="operator">condition operator</param>
        /// <param name="value">value</param>
        /// <param name="fieldNames">field collection</param>
        /// <returns>return newest instance</returns>
        public IQuery And(QueryOperator eachFieldConnectOperator, CriteriaOperator @operator, dynamic value, ICriteriaConvert convert = null, params string[] fieldNames)
        {
            if (fieldNames == null || !fieldNames.Any())
            {
                return this;
            }
            IQuery groupQuery = QueryFactory.Create();
            foreach (string field in fieldNames)
            {
                switch (eachFieldConnectOperator)
                {
                    case QueryOperator.AND:
                    default:
                        groupQuery.And(field, @operator, value, convert);
                        break;
                    case QueryOperator.OR:
                        groupQuery.Or(field, @operator, value, convert);
                        break;
                }
            }
            AddQueryItem(QueryOperator.AND, groupQuery);
            return this;
        }

        /// <summary>
        /// Connect a condition with 'and'
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="criteria">criteria</param>
        /// <returns>return newest instance</returns>
        public IQuery And<T>(Expression<Func<T, bool>> criteria) where T : QueryModel<T>
        {
            var expressQuery = GetExpressionQuery(QueryOperator.AND, criteria.Body);
            if (expressQuery != null)
            {
                AddQueryItem(expressQuery.Item1, expressQuery.Item2);
            }
            return this;
        }

        /// <summary>
        /// Connect a condition with 'and'
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <typeparam name="TProperty">field type</typeparam>
        /// <param name="field">field expression</param>
        /// <param name="operator">condition operator</param>
        /// <param name="value">value</param>
        /// <returns>return newest instance</returns>
        public IQuery And<T>(Expression<Func<T, dynamic>> field, CriteriaOperator @operator, dynamic value, ICriteriaConvert convert = null) where T : QueryModel<T>
        {
            return And(ExpressionHelper.GetExpressionPropertyName(field.Body), @operator, value, convert);
        }

        /// <summary>
        /// Connect a condition with 'and'
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="eachFieldConnectOperator">each field connect operator</param>
        /// <param name="operator">condition operator</param>
        /// <param name="value">value</param>
        /// <param name="fields">field type</param>
        /// <returns>return newest instance</returns>
        public IQuery And<T>(QueryOperator eachFieldConnectOperator, CriteriaOperator @operator, dynamic value, ICriteriaConvert convert = null, params Expression<Func<T, dynamic>>[] fields) where T : QueryModel<T>
        {
            if (fields == null || !fields.Any())
            {
                return this;
            }
            IEnumerable<string> fieldNames = fields.Select(c => ExpressionHelper.GetExpressionPropertyName(c.Body));
            return And(eachFieldConnectOperator, @operator, value, convert, fieldNames.ToArray());
        }

        /// <summary>
        /// Connect a condition with 'and'
        /// </summary>
        /// <param name="subQuery">sub query instance</param>
        /// <returns>return newest instance</returns>
        public IQuery And(IQuery subQuery)
        {
            AddQueryItem(QueryOperator.AND, subQuery);
            return this;
        }

        #endregion

        #region OR

        /// <summary>
        /// Connect a condition with 'or'
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="operator">condition operator</param>
        /// <param name="value">value</param>
        /// <returns>return newest instance</returns>
        public IQuery Or(string fieldName, CriteriaOperator @operator, dynamic value, ICriteriaConvert convert = null)
        {
            AddCriteria(QueryOperator.OR, fieldName, @operator, value, convert);
            return this;
        }

        /// <summary>
        /// Connect a condition with 'or'
        /// </summary>
        /// <param name="eachFieldConnectOperator">each field connect operator</param>
        /// <param name="operator">condition operator</param>
        /// <param name="value">value</param>
        /// <param name="fieldNames">field collection</param>
        /// <returns>return newest instance</returns>
        public IQuery Or(QueryOperator eachFieldConnectOperator, CriteriaOperator @operator, dynamic value, ICriteriaConvert convert = null, params string[] fieldNames)
        {
            if (fieldNames == null || !fieldNames.Any())
            {
                return this;
            }
            IQuery groupQuery = QueryFactory.Create();
            foreach (string field in fieldNames)
            {
                switch (eachFieldConnectOperator)
                {
                    case QueryOperator.AND:
                    default:
                        groupQuery.And(field, @operator, value, convert);
                        break;
                    case QueryOperator.OR:
                        groupQuery.Or(field, @operator, value, convert);
                        break;
                }
            }
            AddQueryItem(QueryOperator.OR, groupQuery);
            return this;
        }

        /// <summary>
        /// Connect a condition with 'or'
        /// </summary>
        /// <typeparam name="T">datatype</typeparam>
        /// <param name="criteria">criteria</param>
        /// <returns>return newest instance</returns>
        public IQuery Or<T>(Expression<Func<T, bool>> criteria) where T : QueryModel<T>
        {
            var expressQuery = GetExpressionQuery(QueryOperator.OR, criteria.Body);
            if (expressQuery != null)
            {
                AddQueryItem(expressQuery.Item1, expressQuery.Item2);
            }
            return this;
        }

        /// <summary>
        /// Connect a condition with 'or'
        /// </summary>
        /// <typeparam name="T">datatype</typeparam>
        /// <typeparam name="TProperty">field type</typeparam>
        /// <param name="field">field</param>
        /// <param name="operator">condition operator</param>
        /// <param name="value">value</param>
        /// <returns>return newest instance</returns>
        public IQuery Or<T>(Expression<Func<T, dynamic>> field, CriteriaOperator @operator, dynamic value, ICriteriaConvert convert = null) where T : QueryModel<T>
        {
            return Or(ExpressionHelper.GetExpressionPropertyName(field.Body), @operator, value, convert);
        }

        /// <summary>
        /// Connect a condition with 'or'
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="eachFieldConnectOperator">each field connect operator</param>
        /// <param name="operator">condition operator</param>
        /// <param name="value">value</param>
        /// <param name="fields">field type</param>
        /// <returns>return newest instance</returns>
        public IQuery Or<T>(QueryOperator eachFieldConnectOperator, CriteriaOperator @operator, dynamic value, ICriteriaConvert convert = null, params Expression<Func<T, dynamic>>[] fields) where T : QueryModel<T>
        {
            if (fields == null || !fields.Any())
            {
                return this;
            }
            IEnumerable<string> fieldNames = fields.Select(c => ExpressionHelper.GetExpressionPropertyName(c.Body));
            return Or(eachFieldConnectOperator, @operator, value, convert, fieldNames);
        }

        /// <summary>
        /// Connect a condition with 'or'
        /// </summary>
        /// <param name="subQuery">sub query instance</param>
        /// <returns>return newest instance</returns>
        public IQuery Or(IQuery subQuery)
        {
            AddQueryItem(QueryOperator.OR, subQuery);
            return this;
        }

        #endregion

        #region Equal

        /// <summary>
        /// Equal Condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>return newest instance</returns>
        public IQuery Equal(string fieldName, dynamic value, bool or = false, ICriteriaConvert convert = null)
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.Equal, value, convert);
            return this;
        }

        /// <summary>
        /// Equal Condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>return newest instance</returns>
        public IQuery Equal<T>(Expression<Func<T, dynamic>> field, dynamic value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, ExpressionHelper.GetExpressionPropertyName(field.Body), CriteriaOperator.Equal, value, convert);
            return this;
        }

        #endregion

        #region Not Equal

        /// <summary>
        /// Not Equal Condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>return newest instance</returns>
        public IQuery NotEqual(string fieldName, dynamic value, bool or = false, ICriteriaConvert convert = null)
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.NotEqual, value, convert);
            return this;
        }

        /// <summary>
        /// Not Equal Condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>return newest instance</returns>
        public IQuery NotEqual<T>(Expression<Func<T, dynamic>> field, dynamic value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, ExpressionHelper.GetExpressionPropertyName(field.Body), CriteriaOperator.NotEqual, value, convert);
            return this;
        }

        #endregion

        #region LessThan

        /// <summary>
        /// Less Than Condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>return newest instance</returns>
        public IQuery LessThan(string fieldName, dynamic value, bool or = false, ICriteriaConvert convert = null)
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.LessThan, value, convert);
            return this;
        }

        /// <summary>
        /// Less Than Condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>return newest instance</returns>
        public IQuery LessThan<T>(Expression<Func<T, dynamic>> field, dynamic value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, ExpressionHelper.GetExpressionPropertyName(field.Body), CriteriaOperator.LessThan, value, convert);
            return this;
        }

        #endregion

        #region LessThanOrEqual

        /// <summary>
        /// Less Than Or Equal Condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>return newest instance</returns>
        public IQuery LessThanOrEqual(string fieldName, dynamic value, bool or = false, ICriteriaConvert convert = null)
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.LessThanOrEqual, value, convert);
            return this;
        }

        /// <summary>
        /// Less Than Or Equal Condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>return newest instance</returns>
        public IQuery LessThanOrEqual<T>(Expression<Func<T, dynamic>> field, dynamic value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, ExpressionHelper.GetExpressionPropertyName(field.Body), CriteriaOperator.LessThanOrEqual, value, convert);
            return this;
        }

        #endregion

        #region GreaterThan

        /// <summary>
        /// Greater Than Condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>return newest instance</returns>
        public IQuery GreaterThan(string fieldName, dynamic value, bool or = false, ICriteriaConvert convert = null)
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.GreaterThan, value, convert);
            return this;
        }

        /// <summary>
        /// Greater Than Condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>return newest instance</returns>
        public IQuery GreaterThan<T>(Expression<Func<T, dynamic>> field, dynamic value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, ExpressionHelper.GetExpressionPropertyName(field.Body), CriteriaOperator.GreaterThan, value, convert);
            return this;
        }

        #endregion

        #region GreaterThanOrEqual

        /// <summary>
        /// Greater Than Or Equal Condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>return newest instance</returns>
        public IQuery GreaterThanOrEqual(string fieldName, dynamic value, bool or = false, ICriteriaConvert convert = null)
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.GreaterThanOrEqual, value, convert);
            return this;
        }

        /// <summary>
        /// Greater Than Or Equal Condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>return newest instance</returns>
        public IQuery GreaterThanOrEqual<T>(Expression<Func<T, dynamic>> field, dynamic value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, ExpressionHelper.GetExpressionPropertyName(field.Body), CriteriaOperator.GreaterThanOrEqual, value, convert);
            return this;
        }

        #endregion

        #region IN

        /// <summary>
        /// Include Condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>return newest instance</returns>
        public IQuery In(string fieldName, IEnumerable value, bool or = false, ICriteriaConvert convert = null)
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.In, value, convert);
            return this;
        }

        /// <summary>
        /// Include Condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>return newest instance</returns>
        public IQuery In<T>(Expression<Func<T, dynamic>> field, IEnumerable value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, ExpressionHelper.GetExpressionPropertyName(field.Body), CriteriaOperator.In, value, convert);
            return this;
        }

        #endregion

        #region Not In

        /// <summary>
        /// Not Include
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>return newest instance</returns>
        public IQuery NotIn(string fieldName, IEnumerable value, bool or = false, ICriteriaConvert convert = null)
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.NotIn, value, convert);
            return this;
        }

        /// <summary>
        /// Not Include
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>return newest instance</returns>
        public IQuery NotIn<T>(Expression<Func<T, dynamic>> field, IEnumerable value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, ExpressionHelper.GetExpressionPropertyName(field.Body), CriteriaOperator.NotIn, value, convert);
            return this;
        }

        #endregion

        #region Like

        /// <summary>
        /// Like Condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>return newest instance</returns>
        public IQuery Like(string fieldName, string value, bool or = false, ICriteriaConvert convert = null)
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.Like, value, convert);
            return this;
        }

        /// <summary>
        /// Like Condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>return newest instance</returns>
        public IQuery Like<T>(Expression<Func<T, dynamic>> field, string value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, ExpressionHelper.GetExpressionPropertyName(field.Body), CriteriaOperator.Like, value, convert);
            return this;
        }

        #endregion

        #region BeginLike

        /// <summary>
        /// Begin Like Condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>return newest instance</returns>
        public IQuery BeginLike(string fieldName, string value, bool or = false, ICriteriaConvert convert = null)
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.BeginLike, value, convert);
            return this;
        }

        /// <summary>
        /// Begin Like Condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>return newest instance</returns>
        public IQuery BeginLike<T>(Expression<Func<T, dynamic>> field, string value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, ExpressionHelper.GetExpressionPropertyName(field.Body), CriteriaOperator.BeginLike, value, convert);
            return this;
        }

        #endregion

        #region EndLike

        /// <summary>
        /// End Like Condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>return newest instance</returns>
        public IQuery EndLike(string fieldName, string value, bool or = false, ICriteriaConvert convert = null)
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.EndLike, value, convert);
            return this;
        }

        /// <summary>
        /// EndLike
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>return newest instance</returns>
        public IQuery EndLike<T>(Expression<Func<T, dynamic>> field, string value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, ExpressionHelper.GetExpressionPropertyName(field.Body), CriteriaOperator.EndLike, value, convert);
            return this;
        }

        #endregion

        #region ASC

        /// <summary>
        /// Order By ASC
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <typeparam name="TProperty">field type</typeparam>
        /// <param name="field">field</param>
        /// <returns>return newest instance</returns>
        public IQuery Asc(string field, ICriteriaConvert convert = null)
        {
            AddOrderItem(field, false, convert);
            return this;
        }

        /// <summary>
        /// Order By ASC
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <returns>return newest instance</returns>
        public IQuery Asc<T>(Expression<Func<T, dynamic>> field, ICriteriaConvert convert = null) where T : QueryModel<T>
        {
            AddOrderItem(ExpressionHelper.GetExpressionPropertyName(field.Body), false, convert);
            return this;
        }

        #endregion

        #region DESC

        /// <summary>
        /// Order By DESC
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <typeparam name="TProperty">field type</typeparam>
        /// <param name="field">field</param>
        /// <returns>return newest instance</returns>
        public IQuery Desc(string field, ICriteriaConvert convert = null)
        {
            AddOrderItem(field, true, convert);
            return this;
        }

        /// <summary>
        /// Order By DESC
        /// </summary>
        /// <param name="field">field</param>
        /// <returns>return newest instance</returns>
        public IQuery Desc<T>(Expression<Func<T, dynamic>> field, ICriteriaConvert convert = null) where T : QueryModel<T>
        {
            AddOrderItem(ExpressionHelper.GetExpressionPropertyName(field.Body), true, convert);
            return this;
        }

        #endregion

        #region Fields

        /// <summary>
        /// Add Special Fields Need To Query
        /// </summary>
        /// <param name="fields">fields</param>
        /// <returns>return newest instance</returns>
        public IQuery AddQueryFields(params string[] fields)
        {
            queryFields.AddRange(fields);
            return this;
        }

        /// <summary>
        /// Add Special Fields Need To Query
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <typeparam name="TProperty">field type</typeparam>
        /// <param name="fieldExpression">field expression</param>
        /// <returns>return newest instance</returns>
        public IQuery AddQueryFields<T>(Expression<Func<T, dynamic>> fieldExpression) where T : QueryModel<T>
        {
            return AddQueryFields(ExpressionHelper.GetExpressionPropertyName(fieldExpression.Body));
        }

        /// <summary>
        /// Add Special Fields That don't Query
        /// </summary>
        /// <param name="fields">fields</param>
        /// <returns>return newest instance</returns>
        public IQuery AddNotQueryFields(params string[] fields)
        {
            notQueryFields.AddRange(fields);
            return this;
        }

        /// <summary>
        /// Add Special Fields That don't Query
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <typeparam name="TProperty">field type</typeparam>
        /// <param name="fieldExpression">field expression</param>
        /// <returns>return newest instance</returns>
        public IQuery AddNotQueryFields<T>(Expression<Func<T, dynamic>> fieldExpression) where T : QueryModel<T>
        {
            return AddNotQueryFields(ExpressionHelper.GetExpressionPropertyName(fieldExpression.Body));
        }

        /// <summary>
        /// get actually query fields
        /// </summary>
        /// <returns></returns>
        public List<string> GetActuallyQueryFields<ET>(bool forcePrimaryKey = true, bool forceVersionKey = true)
        {
            return GetActuallyQueryFields(typeof(ET), forcePrimaryKey, forceVersionKey);
        }

        /// <summary>
        /// get actually query fields
        /// </summary>
        /// <returns></returns>
        public List<string> GetActuallyQueryFields(Type entityType, bool forcePrimaryKey = true, bool forceVersionKey = true)
        {
            List<string> actuallyFields = queryFields?.Select(c => c).ToList();
            if (queryFields.IsNullOrEmpty())
            {
                actuallyFields = EntityManager.GetEntityQueryFields(entityType).Select<EntityField, string>(c => c).ToList();
            }
            else
            {
                if (forcePrimaryKey)
                {
                    var primaryKeys = EntityManager.GetPrimaryKeys(entityType);
                    actuallyFields.AddRange(primaryKeys.Select(c => c.PropertyName));
                }
                if (forceVersionKey)
                {
                    actuallyFields.Add(EntityManager.GetVersionField(entityType));
                }
            }
            if (!notQueryFields.IsNullOrEmpty())
            {
                actuallyFields = actuallyFields.Except(notQueryFields).ToList();
            }
            return actuallyFields;
        }

        #endregion

        #region QueryText

        /// <summary>
        /// Set QueryText
        /// </summary>
        /// <param name="queryText">query text</param>
        /// <param name="parameters">parameters</param>
        /// <returns>return newest instance</returns>
        public IQuery SetQueryText(string queryText, dynamic parameters = null)
        {
            QueryText = queryText;
            QueryTextParameters = parameters;
            QueryType = QueryCommandType.Text;
            return this;
        }

        #endregion

        #region Load Propertys

        /// <summary>
        /// Set Load Propertys
        /// </summary>
        /// <param name="propertys">load propertys</param>
        public void SetLoadPropertys(Dictionary<string, bool> propertys)
        {
            if (propertys == null)
            {
                return;
            }
            foreach (var property in propertys)
            {
                if (loadPropertys.ContainsKey(property.Key))
                {
                    loadPropertys[property.Key] = property.Value;
                }
                else
                {
                    loadPropertys.Add(property.Key, property.Value);
                }
            }
        }

        /// <summary>
        /// Set Load Propertys
        /// </summary>
        /// <typeparam name="T">Data Type</typeparam>
        /// <param name="allowLoad">allow load</param>
        /// <param name="propertys">propertys</param>
        public void SetLoadPropertys<T>(bool allowLoad, params Expression<Func<T, dynamic>>[] propertys)
        {
            if (propertys == null)
            {
                return;
            }
            Dictionary<string, bool> propertyDic = new Dictionary<string, bool>(propertys.Length);
            foreach (var property in propertys)
            {
                propertyDic.Add(ExpressionHelper.GetExpressionPropertyName(property.Body), allowLoad);
            }
            SetLoadPropertys(propertyDic);
        }

        /// <summary>
        /// property is allow load data
        /// </summary>
        /// <param name="propertyName">propertyName</param>
        /// <returns>allow load data</returns>
        public bool AllowLoad(string propertyName)
        {
            return !string.IsNullOrWhiteSpace(propertyName) && loadPropertys != null && loadPropertys.ContainsKey(propertyName) && loadPropertys[propertyName];
        }

        /// <summary>
        /// property is allow load data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property">propertyName</param>
        /// <returns>allow load data</returns>
        public bool AllowLoad<T>(Expression<Func<T, dynamic>> property)
        {
            if (property == null)
            {
                return false;
            }
            string propertyName = ExpressionHelper.GetExpressionPropertyName(property);
            return AllowLoad(propertyName);
        }

        #endregion

        #region Get Special Keys Equal Values

        /// <summary>
        /// Get Special Keys Equal Values
        /// </summary>
        /// <param name="keys">keys</param>
        /// <returns></returns>
        public List<Dictionary<string, dynamic>> GetKeysEqualValue(IEnumerable<string> keys)
        {
            if (QueryType == QueryCommandType.Text || keys == null || !keys.Any() || equalCriteriaList == null || equalCriteriaList.Count <= 0)
            {
                return new List<Dictionary<string, dynamic>>(0);
            }
            if (HasSubQuery)
            {
                return new List<Dictionary<string, dynamic>>(0);
            }
            List<string> keyList = keys.Distinct().ToList();
            int index = 0;
            return GetChildKeyValues(ref index, keyList);
        }

        List<Dictionary<string, dynamic>> GetChildKeyValues(ref int index, List<string> keys)
        {
            List<Dictionary<string, dynamic>> keyValuesDic = new List<Dictionary<string, dynamic>>();
            for (int i = index; i < keys.Count; index = i++)
            {
                string nowKey = keys[i];
                var nowKeyCriteriaList = equalCriteriaList.Where(c => c.Name == nowKey).ToList();
                if (nowKeyCriteriaList == null || nowKeyCriteriaList.Count <= 0)
                {
                    return keyValuesDic;
                }
                foreach (var criteria in nowKeyCriteriaList)
                {
                    if (criteria.Operator == CriteriaOperator.In)
                    {
                        foreach (var val in criteria.Value)
                        {
                            Dictionary<string, dynamic> criteriaValues = new Dictionary<string, dynamic>();
                            criteriaValues.Add(criteria.Name, val);
                            keyValuesDic.Add(criteriaValues);
                        }
                    }
                    else
                    {
                        Dictionary<string, dynamic> criteriaValues = new Dictionary<string, dynamic>();
                        criteriaValues.Add(criteria.Name, criteria.GetCriteriaRealValue());
                        keyValuesDic.Add(criteriaValues);
                    }
                }
                i++;
                if (i < keys.Count)
                {
                    List<Dictionary<string, dynamic>> childKeyValueDic = GetChildKeyValues(ref i, keys);
                    if (childKeyValueDic == null || childKeyValueDic.Count <= 0)
                    {
                        return new List<Dictionary<string, dynamic>>(0);
                    }
                    var maxKeyValues = keyValuesDic;
                    var minKeyValues = childKeyValueDic;
                    if (minKeyValues.Count > maxKeyValues.Count)
                    {
                        minKeyValues = maxKeyValues;
                        keyValuesDic = maxKeyValues = childKeyValueDic;
                    }
                    List<Dictionary<string, dynamic>> newKeyValues = new List<Dictionary<string, dynamic>>();
                    foreach (var nowKeyValue in maxKeyValues)
                    {
                        foreach (var childKeyValue in minKeyValues)
                        {
                            foreach (var childValue in childKeyValue)
                            {
                                if (nowKeyValue.ContainsKey(childValue.Key))
                                {
                                    Dictionary<string, dynamic> copyNewValue = nowKeyValue.Where(c => c.Key != childValue.Key).ToDictionary(c => c.Key, c => c.Value);
                                    copyNewValue.Add(childValue.Key, childValue.Value);
                                    newKeyValues.Add(copyNewValue);
                                }
                                else
                                {
                                    nowKeyValue.Add(childValue.Key, childValue.Value);
                                }
                            }
                        }
                    }
                    keyValuesDic.AddRange(newKeyValues);
                }
            }
            return keyValuesDic;
        }

        #endregion

        #region Get Expression

        /// <summary>
        /// Get Query Expression
        /// </summary>
        /// <typeparam name="T">Data Type</typeparam>
        /// <returns></returns>
        public Func<T, bool> GetQueryExpression<T>()
        {
            Type modelType = typeof(T);
            if (queryExpressionDict.ContainsKey(modelType.GUID))
            {
                return queryExpressionDict[modelType.GUID];
            }
            if (HasSubQuery)
            {
                Func<T, bool> falseFunc = (data) => false;
                queryExpressionDict.Add(modelType.GUID, falseFunc);
                return falseFunc;
            }
            if (criterias.IsNullOrEmpty())
            {
                Func<T, bool> trueFunc = (data) => true;
                queryExpressionDict.Add(modelType.GUID, trueFunc);
                return trueFunc;
            }
            Type funcType = typeof(Func<,>).MakeGenericType(modelType, typeof(bool));
            ParameterExpression parExp = Expression.Parameter(modelType);//parameter model type
            Array parameterArray = Array.CreateInstance(typeof(ParameterExpression), 1);
            parameterArray.SetValue(parExp, 0);
            Expression conditionExpression = null;
            foreach (var queryItem in criterias)
            {
                Expression childExpression = GenerateExpression(parExp, queryItem.Item2);
                if (childExpression == null)
                {
                    continue;
                }
                if (conditionExpression == null)
                {
                    conditionExpression = childExpression;
                    continue;
                }
                if (queryItem.Item1 == QueryOperator.AND)
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
            var genericLambdaMethod = lambdaMethod.MakeGenericMethod(funcType);
            var lambdaExpression = genericLambdaMethod.Invoke(null, new object[]
            {
                conditionExpression,parameterArray
            });
            Func<T, bool> func = ((Expression<Func<T, bool>>)lambdaExpression).Compile();
            queryExpressionDict.Add(modelType.GUID, func);
            return func;
        }

        Expression GenerateExpression(Expression parameter, IQueryItem queryItem)
        {
            if (queryItem is Criteria)
            {
                return GenerateSingleExpression(parameter, queryItem as Criteria);
            }
            IQuery query = queryItem as IQuery;
            if (query.Criterias == null || query.Criterias.Count <= 0)
            {
                return null;
            }
            if (query.Criterias.Count == 1 && query.Criterias[0].Item2 is Criteria)
            {
                return GenerateSingleExpression(parameter, query.Criterias[0].Item2 as Criteria);
            }
            Expression conditionExpression = null;
            foreach (var criteriaItem in query.Criterias)
            {
                var childExpression = GenerateExpression(parameter, criteriaItem.Item2);
                if (conditionExpression == null)
                {
                    conditionExpression = childExpression;
                    continue;
                }
                if (criteriaItem.Item1 == QueryOperator.AND)
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

        Expression GenerateSingleExpression(Expression parameter, Criteria criteria)
        {
            Expression property = Expression.PropertyOrField(parameter, criteria.Name);
            object criteriaValue = criteria.GetCriteriaRealValue();
            Expression valueExpression = Expression.Constant(criteriaValue, criteriaValue.GetType());
            switch (criteria.Operator)
            {
                case CriteriaOperator.Equal:
                    property = Expression.Equal(property, valueExpression);
                    break;
                case CriteriaOperator.NotEqual:
                    property = Expression.NotEqual(property, valueExpression);
                    break;
                case CriteriaOperator.GreaterThan:
                    property = Expression.GreaterThan(property, valueExpression);
                    break;
                case CriteriaOperator.GreaterThanOrEqual:
                    property = Expression.GreaterThanOrEqual(property, valueExpression);
                    break;
                case CriteriaOperator.LessThan:
                    property = Expression.LessThan(property, valueExpression);
                    break;
                case CriteriaOperator.LessThanOrEqual:
                    property = Expression.LessThanOrEqual(property, valueExpression);
                    break;
                case CriteriaOperator.BeginLike:
                    Expression beginLikeExpression = Expression.Call(property, stringIndexOfMethod, valueExpression);
                    property = Expression.Equal(beginLikeExpression, Expression.Constant(0));
                    break;
                case CriteriaOperator.Like:
                    Expression likeExpression = Expression.Call(property, stringIndexOfMethod, valueExpression);
                    property = Expression.GreaterThanOrEqual(likeExpression, Expression.Constant(0));
                    break;
                case CriteriaOperator.EndLike:
                    property = Expression.Call(property, endWithMethod, valueExpression);
                    break;
                case CriteriaOperator.In:
                    Type valueType = criteriaValue.GetType();
                    if (valueType != null && valueType.GenericTypeArguments != null && valueType.GenericTypeArguments.Length > 0)
                    {
                        valueType = valueType.GenericTypeArguments[valueType.GenericTypeArguments.Length - 1];
                    }
                    else if (valueType.IsArray)
                    {
                        Array arrayValue = criteriaValue as Array;
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
                    var inMethod = collectionContainsMethod.MakeGenericMethod(valueType);
                    property = Expression.Call(inMethod, valueExpression, property);
                    break;
                case CriteriaOperator.NotIn:
                    Type notInType = criteriaValue.GetType();
                    if (notInType != null && notInType.GenericTypeArguments != null)
                    {
                        notInType = notInType.GenericTypeArguments[0];
                    }
                    else
                    {
                        notInType = typeof(object);
                    }
                    var notInMethod = collectionContainsMethod.MakeGenericMethod(notInType);
                    property = Expression.Not(Expression.Call(notInMethod, valueExpression, property));
                    break;
                default:
                    property = null;
                    break;
            }
            if (property == null)
            {
                return null;
            }
            return property;
        }

        #endregion

        #region Order Datas

        /// <summary>
        /// Order Datas
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public IEnumerable<T> Order<T>(IEnumerable<T> datas)
        {
            if (orders == null || orders.Count <= 0 || datas == null || !datas.Any())
            {
                return datas;
            }
            IOrderedEnumerable<T> orderedDatas = null;
            foreach (var orderItem in orders)
            {
                var orderFun = ExpressionHelper.GetPropertyOrFieldFunction<T>(orderItem.Name);
                if (orderFun == null)
                {
                    continue;
                }
                if (orderedDatas == null)
                {
                    orderedDatas = orderItem.Desc ? datas.OrderByDescending(orderFun) : datas.OrderBy(orderFun);
                }
                else
                {
                    orderedDatas = orderItem.Desc ? orderedDatas.ThenByDescending(orderFun) : orderedDatas.ThenBy(orderFun);
                }
            }
            return orderedDatas ?? datas;
        }

        #endregion

        #region Recurve

        /// <summary>
        /// set recurve criteria
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="relationKey">relation key</param>
        /// <param name="direction">recurve direction</param>
        /// <returns></returns>
        public IQuery SetRecurve(string key, string relationKey, RecurveDirection direction = RecurveDirection.Down)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(relationKey))
            {
                throw new Exception("key or relationKey is null or empty");
            }
            if (key == relationKey)
            {
                throw new Exception("key and relationKey can not be the same value");
            }
            RecurveCriteria = new RecurveCriteria()
            {
                Key = key,
                RelationKey = relationKey,
                Direction = direction
            };
            return null;
        }

        /// <summary>
        /// set recurve criteria
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="relationKey">relation key</param>
        /// <param name="direction">recurve direction</param>
        /// <returns></returns>
        public IQuery SetRecurve<T>(Expression<Func<T, dynamic>> key, Expression<Func<T, dynamic>> relationKey, RecurveDirection direction = RecurveDirection.Down)
        {
            return SetRecurve(ExpressionHelper.GetExpressionPropertyName(key), ExpressionHelper.GetExpressionPropertyName(relationKey), direction);
        }

        #endregion

        #region Obsolete

        /// <summary>
        /// obsolete query
        /// </summary>
        public void Obsolete()
        {
            IsObsolete = true;
        }

        /// <summary>
        /// cancel obsolete
        /// </summary>
        public void Activate()
        {
            IsObsolete = false;
        }

        #endregion

        #region Clone

        /// <summary>
        /// clone
        /// </summary>
        /// <returns></returns>
        public IQuery Clone()
        {
            QueryInfo newQuery = new QueryInfo();
            newQuery.criterias = new List<Tuple<QueryOperator, IQueryItem>>(this.criterias);
            newQuery.orders = new List<OrderCriteria>(this.orders);
            newQuery.queryFields = new List<string>(this.queryFields);
            newQuery.notQueryFields = new List<string>(this.notQueryFields);
            newQuery.loadPropertys = new Dictionary<string, bool>(this.loadPropertys);
            newQuery.equalCriteriaList = new List<Criteria>(this.equalCriteriaList);
            newQuery.queryExpressionDict = new Dictionary<Guid, dynamic>(this.queryExpressionDict);
            newQuery.QueryModelType = this.QueryModelType;
            newQuery.PagingInfo = this.PagingInfo;
            newQuery.QueryText = this.QueryText;
            newQuery.QueryTextParameters = this.QueryTextParameters;
            newQuery.QueryType = this.QueryType;
            newQuery.QuerySize = this.QuerySize;
            newQuery.HasSubQuery = this.HasSubQuery;
            newQuery.RecurveCriteria = this.RecurveCriteria;
            newQuery.VerifyResult = this.VerifyResult;

            return newQuery;
        }

        #endregion

        #endregion

        #region Util

        /// <summary>
        /// set single criteria
        /// </summary>
        /// <param name="queryOperator">connect operator</param>
        /// <param name="fieldName">field</param>
        /// <param name="criteriaOperator">condition operator</param>
        /// <param name="value">value</param>
        void AddCriteria(QueryOperator queryOperator, string fieldName, CriteriaOperator criteriaOperator, dynamic value, ICriteriaConvert convert = null)
        {
            if (string.IsNullOrWhiteSpace(fieldName) || value == null)
            {
                return;
            }
            Criteria newCriteria = Criteria.CreateNewCriteria(fieldName, criteriaOperator, value);
            newCriteria.Convert = convert;
            AddQueryItem(queryOperator, newCriteria);
        }

        /// <summary>
        /// add query item to all criterias
        /// </summary>
        /// <param name="queryOperator">connect operator</param>
        /// <param name="queryItem">query item</param>
        void AddQueryItem(QueryOperator queryOperator, IQueryItem queryItem)
        {
            if (queryItem == null)
            {
                return;
            }
            if (queryItem is Criteria)
            {
                Criteria criteria = queryItem as Criteria;
                var queryValue = criteria.Value is IQueryItem;
                HasSubQuery = HasSubQuery || queryValue;
                if (!queryValue)
                {
                    switch (criteria.Operator)
                    {
                        case CriteriaOperator.Equal:
                        case CriteriaOperator.In:
                            equalCriteriaList.Add(criteria);
                            break;
                    }
                }
            }
            else if (queryItem is QueryInfo)
            {
                QueryInfo valueQuery = queryItem as QueryInfo;
                HasSubQuery = HasSubQuery || valueQuery.HasSubQuery;
                if (!HasSubQuery)
                {
                    equalCriteriaList.AddRange(valueQuery.equalCriteriaList);
                }
            }
            //clear data
            queryExpressionDict.Clear();
            criterias.Add(new Tuple<QueryOperator, IQueryItem>(queryOperator, queryItem));
        }

        /// <summary>
        /// add order item
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="desc">asc or desc</param>
        void AddOrderItem(string fieldName, bool desc = false, ICriteriaConvert convert = null)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                return;
            }
            orders.Add(new OrderCriteria()
            {
                Name = fieldName,
                Desc = desc,
                Convert = convert
            });
        }

        /// <summary>
        /// get query item
        /// </summary>
        /// <param name="queryOperator">connect operator</param>
        /// <param name="expression">condition expression</param>
        /// <returns></returns>
        Tuple<QueryOperator, IQueryItem> GetExpressionQuery(QueryOperator queryOperator, Expression expression)
        {
            var nodeType = expression.NodeType;
            ExpressionType queryNodeType = queryOperator == QueryOperator.OR ? ExpressionType.OrElse : ExpressionType.AndAlso;
            if (ExpressionHelper.IsCompareNodeType(nodeType))
            {
                return GetSingleExpressionQueryItem(queryNodeType, expression);
            }
            else if (ExpressionHelper.IsBoolNodeType(nodeType))
            {
                BinaryExpression binExpression = expression as BinaryExpression;
                if (binExpression == null)
                {
                    throw new Exception("expression is error");
                }
                QueryInfo query = new QueryInfo();
                var leftQuery = GetExpressionQuery(queryOperator, binExpression.Left);
                if (leftQuery != null)
                {
                    query.AddQueryItem(leftQuery.Item1, leftQuery.Item2);
                }
                QueryOperator rightQueryOperator = nodeType == ExpressionType.OrElse ? QueryOperator.OR : QueryOperator.AND;
                var rightQuery = GetExpressionQuery(rightQueryOperator, binExpression.Right);
                if (rightQuery != null)
                {
                    query.AddQueryItem(rightQuery.Item1, rightQuery.Item2);
                }
                return new Tuple<QueryOperator, IQueryItem>(queryOperator, query);
            }
            else if (nodeType == ExpressionType.Call)
            {
                return GetCallExpressionQueryItem(queryOperator, CriteriaOperator.In, expression);
            }
            else if (nodeType == ExpressionType.Not)
            {
                UnaryExpression unaryExpress = expression as UnaryExpression;
                if (unaryExpress != null && unaryExpress.Operand is MethodCallExpression)
                {
                    return GetCallExpressionQueryItem(queryOperator, CriteriaOperator.NotIn, unaryExpress.Operand);
                }
            }
            return null;
        }

        /// <summary>
        /// get single query item
        /// </summary>
        /// <param name="expressionType">expression node type</param>
        /// <param name="expression">expression</param>
        /// <returns></returns>
        Tuple<QueryOperator, IQueryItem> GetCallExpressionQueryItem(QueryOperator queryOperator, CriteriaOperator criteriaOperator, Expression expression)
        {
            MethodCallExpression callExpression = expression as MethodCallExpression;
            Criteria criteria = null;
            switch (callExpression.Method.Name)
            {
                case "Contains":
                    MemberExpression memberArg = null;
                    Expression parameterExpression = null;
                    string parameterName = string.Empty;
                    if (callExpression.Object != null)
                    {
                        memberArg = callExpression.Object as MemberExpression;
                        parameterExpression = callExpression.Arguments[0];
                    }
                    else if (callExpression.Arguments.Count == 2)
                    {
                        memberArg = callExpression.Arguments[0] as MemberExpression;
                        parameterExpression = callExpression.Arguments[1];
                    }
                    if (memberArg == null || parameterExpression == null)
                    {
                        return null;
                    }
                    IEnumerable values = Expression.Lambda(memberArg)?.Compile().DynamicInvoke() as IEnumerable;
                    if (values == null)
                    {
                        return null;
                    }
                    if (parameterExpression is ParameterExpression)
                    {
                        parameterName = (parameterExpression as ParameterExpression)?.Name;
                    }
                    else if (parameterExpression is MemberExpression)
                    {
                        parameterName = ExpressionHelper.GetExpressionPropertyName(parameterExpression as MemberExpression);
                    }
                    if (string.IsNullOrWhiteSpace(parameterName))
                    {
                        return null;
                    }
                    criteria = Criteria.CreateNewCriteria(parameterName, criteriaOperator, values);
                    break;
            }
            if (criteria != null)
            {
                return new Tuple<QueryOperator, IQueryItem>(queryOperator, criteria);
            }
            return null;
        }

        /// <summary>
        /// get single query item
        /// </summary>
        /// <param name="expressionType">expression node type</param>
        /// <param name="expression">expression</param>
        /// <returns></returns>
        Tuple<QueryOperator, IQueryItem> GetSingleExpressionQueryItem(ExpressionType expressionType, Expression expression)
        {
            if (expression == null)
            {
                throw new Exception("expression is null");
            }
            BinaryExpression binaryExpression = expression as BinaryExpression;
            if (binaryExpression == null)
            {
                throw new Exception("expression is error");
            }
            QueryOperator qOperator = expressionType == ExpressionType.OrElse ? QueryOperator.OR : QueryOperator.AND;
            Tuple<Expression, Expression> nameAndValue = GetNameAndValueExpression(binaryExpression.Left, binaryExpression.Right);
            if (nameAndValue == null)
            {
                return null;
            }
            string name = ExpressionHelper.GetExpressionPropertyName(nameAndValue.Item1);
            object value = nameAndValue.Item2;
            if (string.IsNullOrEmpty(name) || value == null)
            {
                return null;
            }
            CriteriaOperator cOperator = GetCriteriaOperator(binaryExpression.NodeType);
            return new Tuple<QueryOperator, IQueryItem>(qOperator, Criteria.CreateNewCriteria(name, cOperator, value));
        }

        /// <summary>
        /// get field name and value expression
        /// </summary>
        /// <param name="firstExpression">first expression</param>
        /// <param name="secondExpression">second expression</param>
        /// <returns></returns>
        Tuple<Expression, Expression> GetNameAndValueExpression(Expression firstExpression, Expression secondExpression)
        {
            Tuple<Expression, Expression> result = null;
            bool firstIsNameExp = IsNameExpression(firstExpression);
            bool secondIsNameExp = IsNameExpression(secondExpression);
            if (!firstIsNameExp && !secondIsNameExp)
            {
                return result;
            }
            if (firstIsNameExp && secondIsNameExp)
            {
                Expression firstChildExp = ExpressionHelper.GetLastChildExpression(firstExpression);
                Expression secondChildExp = ExpressionHelper.GetLastChildExpression(secondExpression);
                result = firstChildExp.NodeType >= secondChildExp.NodeType ? new Tuple<Expression, Expression>(firstExpression, secondExpression) : new Tuple<Expression, Expression>(secondExpression, firstExpression);
                return result;
            }
            result = firstIsNameExp ? new Tuple<Expression, Expression>(firstExpression, secondExpression) : new Tuple<Expression, Expression>(secondExpression, firstExpression);
            return result;
        }

        /// <summary>
        /// is field name expression
        /// </summary>
        /// <param name="exp">expression</param>
        /// <returns></returns>
        bool IsNameExpression(Expression exp)
        {
            if (exp == null)
            {
                return false;
            }
            bool result = false;
            switch (exp.NodeType)
            {
                case ExpressionType.MemberAccess:
                    result = true;
                    break;
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    UnaryExpression unaryExp = exp as UnaryExpression;
                    if (unaryExp.Operand.NodeType == ExpressionType.MemberAccess)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        /// <summary>
        /// get condition operator by expression type
        /// </summary>
        /// <param name="expressType">expression type</param>
        /// <returns></returns>
        CriteriaOperator GetCriteriaOperator(ExpressionType expressType)
        {
            CriteriaOperator cOperator = CriteriaOperator.Equal;
            switch (expressType)
            {
                case ExpressionType.Equal:
                default:
                    cOperator = CriteriaOperator.Equal;
                    break;
                case ExpressionType.NotEqual:
                    cOperator = CriteriaOperator.NotEqual;
                    break;
                case ExpressionType.LessThanOrEqual:
                    cOperator = CriteriaOperator.LessThanOrEqual;
                    break;
                case ExpressionType.LessThan:
                    cOperator = CriteriaOperator.LessThan;
                    break;
                case ExpressionType.GreaterThan:
                    cOperator = CriteriaOperator.GreaterThan;
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    cOperator = CriteriaOperator.GreaterThanOrEqual;
                    break;
            }
            return cOperator;
        }

        #endregion
    }
}
