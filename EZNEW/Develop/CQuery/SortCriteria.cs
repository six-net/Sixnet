using System;
using EZNEW.Develop.CQuery.CriteriaConverter;

namespace EZNEW.Develop.CQuery
{
    /// <summary>
    /// Sort criteria
    /// </summary>
    [Serializable]
    public class SortCriteria
    {
        /// <summary>
        /// Gets or sets the field name
        /// </summary>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whether order by desc
        /// </summary>
        public bool Desc
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the converter
        /// </summary>
        public ICriteriaConverter Converter
        {
            get; set;
        }
    }
}
