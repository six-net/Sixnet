using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.CQuery.Translator
{
    /// <summary>
    /// Translate Result
    /// </summary>
    public class TranslateResult
    {
        #region Propertys

        /// <summary>
        /// Order String
        /// </summary>
        public string OrderString
        {
            get; set;
        }

        /// <summary>
        /// Condition String
        /// </summary>
        public string ConditionString
        {
            get; set;
        }

        /// <summary>
        /// parameters
        /// </summary>
        public object Parameters
        {
            get; set;
        }

        /// <summary>
        /// Pre Script
        /// </summary>
        public string PreScript
        {
            get;set;
        }

        #endregion

        private TranslateResult()
        { }

        #region Static Functions

        /// <summary>
        /// Create a new TranslateResult Instance
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="order">order</param>
        /// <returns></returns>
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
        /// return a empty TranslateResult Object
        /// </summary>
        public static TranslateResult Empty
        {
            get
            {
                return CreateNewResult();
            }
        }

        #endregion
    }
}
