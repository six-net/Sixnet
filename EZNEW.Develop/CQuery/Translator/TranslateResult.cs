using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.CQuery.Translator
{
    /// <summary>
    /// translate result
    /// </summary>
    public class TranslateResult
    {
        #region propertys

        /// <summary>
        /// order string
        /// </summary>
        public string OrderString
        {
            get; set;
        }

        /// <summary>
        /// condition string
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
        /// pre script
        /// </summary>
        public string PreScript
        {
            get;set;
        }

        /// <summary>
        /// join value
        /// </summary>
        public string JoinScript
        {
            get;set;
        }

        /// <summary>
        /// allow join
        /// </summary>
        public bool AllowJoin
        {
            get;set;
        }

        /// <summary>
        /// with scripts
        /// </summary>
        public List<string> WithScripts
        {
            get;set;
        }

        /// <summary>
        /// recurve object name
        /// </summary>
        public string RecurveObjectName
        {
            get;set;
        }

        /// <summary>
        /// recurve pet name
        /// </summary>
        public string RecurvePetName
        {
            get;set;
        }

        #endregion

        private TranslateResult()
        { }

        #region static functions

        /// <summary>
        /// create a new translate result
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
        /// return a empty translate result
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
