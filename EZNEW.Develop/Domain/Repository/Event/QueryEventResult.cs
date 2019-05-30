using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Domain.Repository.Event
{
    public class QueryEventResult<T> : IRepositoryEventHandleResult
    {
        /// <summary>
        /// datas
        /// </summary>
        public List<T> Datas
        {
            get; set;
        }
    }
}
