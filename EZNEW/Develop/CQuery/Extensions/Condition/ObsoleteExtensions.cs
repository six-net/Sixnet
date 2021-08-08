using EZNEW.Develop.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.CQuery
{
    public static class ObsoleteExtensions
    {
        /// <summary>
        /// Exclude obsolete data
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns></returns>
        public static IQuery ExcludeObsoleteData(this IQuery originalQuery)
        {
            if (originalQuery == null)
            {
                throw new ArgumentNullException($"{nameof(originalQuery)}");
            }
            var entityType = originalQuery.GetEntityType();
            if (entityType == null)
            {
                return originalQuery;
            }

            if (QueryManager.FilterObsoleteData)
            {
                var obsoleteField = EntityManager.GetObsoleteField(entityType);
                if (!string.IsNullOrWhiteSpace(obsoleteField) && !originalQuery.IncludeObsolete)
                {
                    originalQuery = originalQuery.Equal(obsoleteField, false);
                }
            }

            return originalQuery;
        }
    }
}
