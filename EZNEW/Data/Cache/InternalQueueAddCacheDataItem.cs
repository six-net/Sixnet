using System;
using System.Collections.Generic;
using System.Text;
using EZNEW.Cache;
using EZNEW.Data.Cache.Policy;
using EZNEW.DataValidation;
using EZNEW.Develop.CQuery;
using EZNEW.Queue;

namespace EZNEW.Data.Cache
{
    /// <summary>
    /// Add cache data internal queue item
    /// </summary>
    public class InternalQueueAddCacheDataItem<T> : IInternalQueueItem
    {
        readonly Action<QueryDataCallbackContext<T>> addCacheProxy = null;
        readonly QueryDataCallbackContext<T> queryDataCallbackContext = null;

        internal InternalQueueAddCacheDataItem(Action<QueryDataCallbackContext<T>> addCacheDataProxy, QueryDataCallbackContext<T> queryDataCallbackContext)
        {
            addCacheProxy = addCacheDataProxy;
            this.queryDataCallbackContext = queryDataCallbackContext;
        }

        public void Execute()
        {
            if (addCacheProxy == null)
            {
                return;
            }
            addCacheProxy(queryDataCallbackContext);
        }
    }
}
