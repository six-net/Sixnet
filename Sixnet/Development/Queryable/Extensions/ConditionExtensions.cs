using System;

namespace Sixnet.Development.Queryable
{
    public static class ConditionExtensions
    {
        /// <summary>
        /// Clone a new condition
        /// </summary>
        /// <param name="originalCondition">Original condition</param>
        /// <returns>Return a new condition</returns>
        public static ISixnetCondition Clone(this ISixnetCondition originalCondition)
        {
            if (originalCondition is Criterion criterion)
            {
                return criterion.Clone();
            }
            if (originalCondition is ISixnetQueryable queryable)
            {
                return queryable?.Clone();
            }
            throw new NotSupportedException($"Cloning operations are not supported for {originalCondition?.GetType()}");
        }
    }
}
