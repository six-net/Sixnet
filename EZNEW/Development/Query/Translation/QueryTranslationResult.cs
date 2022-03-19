using System;
using System.Collections.Generic;

namespace EZNEW.Development.Query.Translation
{
    /// <summary>
    /// Defines query translation result
    /// </summary>
    [Serializable]
    public class QueryTranslationResult
    {
        #region Properties

        /// <summary>
        /// Gets or sets the sort string
        /// </summary>
        public string SortString { get; set; }

        /// <summary>
        /// Gets or sets the sort field string
        /// </summary>
        public List<string> SortFields { get; set; }

        /// <summary>
        /// Gets or sets the condition string
        /// </summary>
        public string ConditionString { get; set; }

        /// <summary>
        /// Gets or sets the parameters
        /// </summary>
        public object Parameters { get; set; }

        /// <summary>
        /// Gets or sets the pre script
        /// </summary>
        public string PreScript { get; set; }

        /// <summary>
        /// Gets or sets the join value
        /// </summary>
        public string JoinScript { get; set; }

        /// <summary>
        /// Gets or sets whether allow join
        /// </summary>
        public bool AllowJoin { get; set; }

        /// <summary>
        /// Gets or sets the with scripts
        /// </summary>
        public List<string> WithScripts { get; set; }

        /// <summary>
        /// Gets or sets the recurve object name
        /// </summary>
        public string RecurveObjectName { get; set; }

        /// <summary>
        /// Gets or sets the recurve pet name
        /// </summary>
        public string RecurvePetName { get; set; }

        /// <summary>
        /// Combine script
        /// </summary>
        public string CombineScript { get; set; }

        /// <summary>
        /// Join extra condition strinng
        /// </summary>
        public string JoinExtraConditionString { get; set; }

        #endregion

        private QueryTranslationResult() { }

        #region Static functions

        /// <summary>
        /// Create a new translation result
        /// </summary>
        /// <param name="conditionString">Condition string</param>
        /// <param name="sortString">Sort string</param>
        /// <returns>Return a translation result</returns>
        public static QueryTranslationResult Create(string conditionString = "", string sortString = "", object parameters = null)
        {
            return new QueryTranslationResult()
            {
                ConditionString = conditionString,
                SortString = sortString,
                Parameters = parameters
            };
        }

        /// <summary>
        /// Return a empty translation result
        /// </summary>
        public static QueryTranslationResult Empty => Create();

        #endregion
    }
}
