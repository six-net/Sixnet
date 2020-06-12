using System;

namespace EZNEW.Develop.CQuery
{
    /// <summary>
    /// Query model
    /// </summary>
    [Serializable]
    public abstract class QueryModel<T> where T : QueryModel<T>
    {
        static QueryModel()
        {
            QueryManager.ConfigureQueryModelRelationEntity<T>();
        }

        /// <summary>
        /// Init
        /// </summary>
        internal static void Init()
        {
        }
    }
}
