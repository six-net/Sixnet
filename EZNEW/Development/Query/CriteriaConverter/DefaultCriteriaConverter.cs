using System;

namespace EZNEW.Development.Query.CriteriaConverter
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

        /// <summary>
        /// Clone a new converter
        /// </summary>
        /// <returns></returns>
        public ICriteriaConverter Clone()
        {
            return MemberwiseClone() as ICriteriaConverter;
        }
    }
}
