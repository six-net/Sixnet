using System;
using System.Collections.Generic;
using System.Text;
using EZNEW.Development.Entity;
using EZNEW.Development.Query;

namespace EZNEW.Development.Query
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
                if (!string.IsNullOrWhiteSpace(obsoleteField) && !originalQuery.IncludeObsoleteData)
                {
                    originalQuery = originalQuery.Equal(obsoleteField, false);
                }
            }

            return originalQuery;
        }
    }
}
