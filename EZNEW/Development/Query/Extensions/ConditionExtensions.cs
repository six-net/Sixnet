using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Development.Query
{
    public static class ConditionExtensions
    {
        /// <summary>
        /// Clone a new condition
        /// </summary>
        /// <param name="originalCondition">Original condition</param>
        /// <returns>Return a new condition</returns>
        public static ICondition Clone(this ICondition originalCondition)
        {
            if (originalCondition is Criterion criterion)
            {
                return criterion.Clone();
            }
            if (originalCondition is IQuery query)
            {
                return query?.Clone();
            }
            throw new NotSupportedException($"Cloning operations are not supported for {originalCondition?.GetType()}");
        }
    }
}
