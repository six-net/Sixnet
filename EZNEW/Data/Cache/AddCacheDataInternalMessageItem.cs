using System;
using System.Collections.Generic;
using System.Text;
using EZNEW.Cache;
using EZNEW.Data.Cache.Policy;
using EZNEW.DataValidation;
using EZNEW.Develop.CQuery;
using EZNEW.Internal.MessageQueue;

namespace EZNEW.Data.Cache
{
    /// <summary>
    /// Add cache data internal message item
    /// </summary>
    public class AddCacheDataInternalMessageItem<T> : IInternalMessageQueueCommand
    {
        readonly Action<QueryDataCallbackContext<T>> addCacheProxy = null;
        readonly QueryDataCallbackContext<T> queryDataCallbackContext = null;

        internal AddCacheDataInternalMessageItem(Action<QueryDataCallbackContext<T>> addCacheDataProxy, QueryDataCallbackContext<T> queryDataCallbackContext)
        {
            addCacheProxy = addCacheDataProxy;
            this.queryDataCallbackContext = queryDataCallbackContext;
        }

        public void Run()
        {
            if (addCacheProxy == null)
            {
                return;
            }
            addCacheProxy(queryDataCallbackContext);
        }
    }
}
