using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Domain.Repository.Event
{
    /// <summary>
    /// save object event
    /// </summary>
    public class SaveObjectEvent<T> : IRepositoryEvent
    {
        /// <summary>
        /// datas
        /// </summary>
        public IEnumerable<T> Datas { get; set; }
    }
}
