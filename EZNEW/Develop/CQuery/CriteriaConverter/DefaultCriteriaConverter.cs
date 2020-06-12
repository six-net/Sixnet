using System;

namespace EZNEW.Develop.CQuery.CriteriaConverter
{
    /// <summary>
    /// Default criteria convert
    /// </summary>
    [Serializable]
    public class DefaultCriteriaConverter : ICriteriaConverter
    {
        /// <summary>
        /// Gets or sets the convert name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the data
        /// </summary>
        public object Data { get; set; }
    }
}
