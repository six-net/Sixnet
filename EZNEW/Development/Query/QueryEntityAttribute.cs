using System;
using EZNEW.Development.Entity;

namespace EZNEW.Development.Query
{
    /// <summary>
    /// Query model entity
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class QueryEntityAttribute : Attribute
    {
        public QueryEntityAttribute(Type relevanceType)
        {
            RelevanceType = relevanceType;
        }

        /// <summary>
        /// Gets or sets the relevance type
        /// </summary>
        public Type RelevanceType { get; set; }
    }
}
