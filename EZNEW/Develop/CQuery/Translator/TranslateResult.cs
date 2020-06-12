using System;
using System.Collections.Generic;

namespace EZNEW.Develop.CQuery.Translator
{
    /// <summary>
    /// translate result
    /// </summary>
    [Serializable]
    public class TranslateResult
    {
        #region Properties

        /// <summary>
        /// Gets or sets the order string
        /// </summary>
        public string OrderString
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the condition string
        /// </summary>
        public string ConditionString
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the parameters
        /// </summary>
        public object Parameters
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the pre script
        /// </summary>
        public string PreScript
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the join value
        /// </summary>
        public string JoinScript
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whether allow join
        /// </summary>
        public bool AllowJoin
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the with scripts
        /// </summary>
        public List<string> WithScripts
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the recurve object name
        /// </summary>
        public string RecurveObjectName
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the recurve pet name
        /// </summary>
        public string RecurvePetName
        {
            get; set;
        }

        #endregion

        private TranslateResult() { }

        #region Static functions

        /// <summary>
        /// Create a new translate result
        /// </summary>
        /// <param name="condition">Condition string</param>
        /// <param name="order">Order string</param>
        /// <returns>Return the translate result</returns>
        public static TranslateResult CreateNewResult(string condition = "", string order = "", object parameters = null)
        {
            return new TranslateResult()
            {
                ConditionString = condition,
                OrderString = order,
                Parameters = parameters
            };
        }

        /// <summary>
        /// Return a empty translate result
        /// </summary>
        public static TranslateResult Empty => CreateNewResult();

        #endregion
    }
}
