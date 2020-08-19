using System;
using EZNEW.Develop.Entity;

namespace EZNEW.Develop.CQuery
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
