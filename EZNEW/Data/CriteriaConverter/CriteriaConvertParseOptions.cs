using EZNEW.Develop.CQuery.CriteriaConverter;

namespace EZNEW.Data.CriteriaConverter
{
    /// <summary>
    /// Criteria converter parse options
    /// </summary>
    public class CriteriaConverterParseOptions
    {
        /// <summary>
        /// Gets or sets the criteria converter
        /// </summary>
        public ICriteriaConverter CriteriaConverter { get; set; }

        /// <summary>
        /// Gets or sets the database server type
        /// </summary>
        public DatabaseServerType ServerType { get; set; }

        /// <summary>
        /// Gets or sets the object name
        /// </summary>
        public string ObjectName { get; set; }

        /// <summary>
        /// Gets or sets the field name
        /// </summary>
        public string FieldName { get; set; }
    }
}
