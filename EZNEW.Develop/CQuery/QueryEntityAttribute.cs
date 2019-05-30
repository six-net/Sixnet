using EZNEW.Develop.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.CQuery
{
    /// <summary>
    /// query model entity
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class QueryEntityAttribute : Attribute
    {
        public QueryEntityAttribute(Type relevanceType)
        {
            EntityManager.ConfigEntity(relevanceType);
            RelevanceType = relevanceType;
        }

        /// <summary>
        /// 关联类型全名
        /// </summary>
        public Type RelevanceType
        {
            get; set;
        }
    }
}
