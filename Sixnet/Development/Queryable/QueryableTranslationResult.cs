using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Sixnet.Development.Data.Field;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Defines queryable translation result
    /// </summary>
    [Serializable]
    public class QueryableTranslationResult
    {
        #region Constructor

        private QueryableTranslationResult() { }

        #endregion

        #region Fields

        /// <summary>
        /// Original query
        /// </summary>
        ISixnetQueryable originalQueryable;

        /// <summary>
        /// Conditions
        /// </summary>
        StringBuilder conditions;

        /// <summary>
        /// Join script
        /// </summary>
        string join;

        /// <summary>
        /// Indecates whether use join
        /// </summary>
        bool allowJoin { get; set; } = true;

        /// <summary>
        /// Join connection
        /// </summary>
        string joinConnection;

        /// <summary>
        /// Having connection
        /// </summary>
        string havingCondition;

        /// <summary>
        /// Sort
        /// </summary>
        string sort;

        /// <summary>
        /// Combine
        /// </summary>
        string combine;

        /// <summary>
        /// Group
        /// </summary>
        string group;

        /// <summary>
        /// Pre output
        /// </summary>
        string preOutput;

        /// <summary>
        /// Pre output fields
        /// </summary>
        IEnumerable<ISixnetField> preOutputFields;

        /// <summary>
        /// Negation func
        /// </summary>
        Func<string, string> negationFunc = null;

        #endregion

        #region Methods

        #region Create

        /// <summary>
        /// Create a new translation result
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns></returns>
        public static QueryableTranslationResult Create(ISixnetQueryable originalQuery)
        {
            return new QueryableTranslationResult()
            {
                originalQueryable = originalQuery
            };
        }

        /// <summary>
        /// Return a empty translation result
        /// </summary>
        public static QueryableTranslationResult Empty => Create(null);

        #endregion

        #region Condition

        /// <summary>
        /// Append condtion
        /// </summary>
        /// <param name="newConditionString">Condition string</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        public QueryableTranslationResult AddCondition(string newConditionString, string connector)
        {
            if (!string.IsNullOrWhiteSpace(newConditionString))
            {
                if (conditions == null)
                {
                    conditions = new StringBuilder(newConditionString);
                }
                else
                {
                    conditions.Append($"{(conditions.Length > 0 ? $" {connector} {newConditionString}" : newConditionString)}");
                }
            }
            return this;
        }

        /// <summary>
        /// Gets the condition string
        /// </summary>
        /// <returns></returns>
        public string GetCondition(string conditionStatementStartKeyword = "")
        {
            var conditionString = conditions?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(conditionString))
            {
                return string.Empty;
            }
            if(negationFunc!=null)
            {
                conditionString = negationFunc(conditionString);
            }
            if (!string.IsNullOrWhiteSpace(conditionStatementStartKeyword))
            {
                return $"{conditionStatementStartKeyword}{conditionString}";
            }
            return conditionString;
        }

        /// <summary>
        /// Negate condition
        /// </summary>
        /// <param name="negationFunc">Negation func</param>
        /// <returns></returns>
        public QueryableTranslationResult Negate(Func<string,string> negationFunc)
        {
            this.negationFunc = negationFunc;
            return this;
        }

        /// <summary>
        /// Clear the conditions
        /// </summary>
        /// <returns></returns>
        public QueryableTranslationResult ClearCondition()
        {
            conditions?.Clear();
            return this;
        }

        #endregion

        #region Join

        /// <summary>
        /// Set join script
        /// </summary>
        /// <param name="joinScript">Join script</param>
        /// <returns></returns>
        public QueryableTranslationResult SetJoin(string joinScript)
        {
            join = joinScript;
            return this;
        }

        /// <summary>
        /// Get join script
        /// </summary>
        /// <returns></returns>
        public string GetJoin()
        {
            return (allowJoin ? join : string.Empty) ?? string.Empty;
        }

        /// <summary>
        /// Set join connection
        /// </summary>
        /// <param name="joinConnection">Join connection</param>
        /// <returns></returns>
        public QueryableTranslationResult SetJoinConnection(string joinConnection)
        {
            this.joinConnection = joinConnection;
            return this;
        }

        /// <summary>
        /// Get join connection
        /// </summary>
        /// <returns></returns>
        public string GetJoinConnection()
        {
            return joinConnection;
        }

        ///// <summary>
        ///// Set join extra condition
        ///// </summary>
        ///// <param name="joinExtraCondition">Join extra condition</param>
        ///// <returns></returns>
        //public QueryTranslationResult SetJoinExtraCondition(string joinExtraCondition)
        //{
        //    this.joinExtraCondition = joinExtraCondition;
        //    return this;
        //}

        ///// <summary>
        ///// Get the join extra condition
        ///// </summary>
        ///// <returns></returns>
        //public string GetJoinExtraCondition()
        //{
        //    return joinExtraCondition ?? string.Empty;
        //}

        /// <summary>
        /// Set having condition
        /// </summary>
        /// <param name="havingCondition"></param>
        /// <returns></returns>
        public QueryableTranslationResult SetHavingCondition(string havingCondition)
        {
            this.havingCondition = havingCondition;
            return this;
        }

        /// <summary>
        /// Get having condition
        /// </summary>
        /// <returns></returns>
        public string GetHavingCondition()
        {
            return havingCondition;
        }

        #endregion

        #region Sort

        /// <summary>
        /// Get sort
        /// </summary>
        /// <returns></returns>
        public string GetSort()
        {
            return sort;
        }

        /// <summary>
        /// Sets the sort
        /// </summary>
        /// <param name="sort">Sort string</param>
        /// <returns></returns>
        public QueryableTranslationResult SetSort(string sort, string sortStatementStartKeyword)
        {
            if (!string.IsNullOrWhiteSpace(sortStatementStartKeyword))
            {
                this.sort = $"{sortStatementStartKeyword}{sort}";
            }
            this.sort ??= string.Empty;
            return this;
        }

        #endregion

        #region Combine

        /// <summary>
        /// Gets the combine string
        /// </summary>
        /// <returns></returns>
        public string GetCombine()
        {
            return combine;
        }

        /// <summary>
        /// Sets the combine string
        /// </summary>
        /// <param name="combine">Combine string</param>
        /// <returns></returns>
        public QueryableTranslationResult SetCombine(string combine)
        {
            this.combine = combine;
            return this;
        }

        #endregion

        #region Group

        /// <summary>
        /// Get group
        /// </summary>
        /// <returns></returns>
        public string GetGroup()
        {
            return group;
        }

        /// <summary>
        /// Set group
        /// </summary>
        /// <param name="group">Group</param>
        /// <returns></returns>
        public QueryableTranslationResult SetGroup(string group)
        {
            this.group = group;
            return this;
        }

        #endregion

        #region Original queryable

        /// <summary>
        /// Gets th original query
        /// </summary>
        /// <returns></returns>
        public ISixnetQueryable GetOriginalQueryable()
        {
            return originalQueryable;
        }

        #endregion

        #region Pre Output

        /// <summary>
        /// Set output
        /// </summary>
        /// <param name="statement">Output statement</param>
        /// <param name="fields">Output fiels</param>
        /// <returns></returns>
        public QueryableTranslationResult SetPreOutput(string statement, IEnumerable<ISixnetField> fields = null)
        {
            preOutput = statement;
            preOutputFields = fields;
            return this;
        }

        /// <summary>
        /// Get output
        /// </summary>
        /// <returns></returns>
        public string GetPreOutputStatement()
        {
            return preOutput;
        }

        /// <summary>
        /// Get output fields
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ISixnetField> GetPreOutputFields()
        {
            return preOutputFields;
        }

        #endregion

        #endregion
    }
}
