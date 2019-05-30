using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.CQuery
{
    /// <summary>
    /// Recurve Criteria
    /// </summary>
    public class RecurveCriteria
    {
        /// <summary>
        /// Key
        /// </summary>
        public string Key
        {
            get; set;
        }

        /// <summary>
        /// Relation Key
        /// </summary>
        public string RelationKey
        {
            get; set;
        }

        /// <summary>
        /// Recurve Direction
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
