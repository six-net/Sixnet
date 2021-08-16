using System;

namespace EZNEW.Development.Query
{
    /// <summary>
    /// Recurve criteria
    /// </summary>
    [Serializable]
    public class RecurveCriteria
    {
        /// <summary>
        /// Gets or sets the data key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the relation key
        /// </summary>
        public string RelationKey { get; set; }

        /// <summary>
        /// Gets or sets the recurve direction
        /// </summary>
        public RecurveDirection Direction { get; set; }
    }
}
