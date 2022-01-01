using System;

namespace EZNEW.Development.Query
{
    /// <summary>
    /// Recursion entry
    /// </summary>
    [Serializable]
    public class Recurve
    {
        /// <summary>
        /// Gets or sets the data field
        /// </summary>
        public string DataField { get; set; }

        /// <summary>
        /// Gets or sets the relation field
        /// </summary>
        public string RelationField { get; set; }

        /// <summary>
        /// Gets or sets the recurve direction
        /// </summary>
        public RecurveDirection Direction { get; set; }

        /// <summary>
        /// Clone a new recure
        /// </summary>
        /// <returns></returns>
        public Recurve Clone()
        {
            return new Recurve()
            {
                DataField = DataField,
                Direction = Direction,
                RelationField = RelationField
            };
        }
    }
}
