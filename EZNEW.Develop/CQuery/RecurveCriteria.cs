using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.CQuery
{
    /// <summary>
    /// recurve criteria
    /// </summary>
    public class RecurveCriteria
    {
        /// <summary>
        /// key
        /// </summary>
        public string Key
        {
            get; set;
        }

        /// <summary>
        /// relation key
        /// </summary>
        public string RelationKey
        {
            get; set;
        }

        /// <summary>
        /// recurve direction
        /// </summary>
        public RecurveDirection Direction
        {
            get;set;
        }
    }

    /// <summary>
    /// recurve direction
    /// </summary>
    public enum RecurveDirection
    {
        Up = 210,
        Down = 220
    }
}
