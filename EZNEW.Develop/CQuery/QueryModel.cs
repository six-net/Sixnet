using EZNEW.Develop.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.CQuery
{
    /// <summary>
    /// QueryModel
    /// </summary>
    public abstract class QueryModel<T> where T : QueryModel<T>
    {
        static QueryModel()
        {
            QueryManager.ConfigQueryModelRelationEntity<T>();
        }

        internal static void Init()
        {
        }
    }
}
