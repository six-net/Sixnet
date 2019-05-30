using EZNEW.Develop.CQuery;
using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Domain.Repository.Warehouse
{
    /// <summary>
    /// compute result
    /// </summary>
    public class ComputeResult<VT>
    {
        /// <summary>
        /// value
        /// </summary>
        public VT Value
        {
            get;set;
        }

        /// <summary>
        /// compute query
        /// </summary>
        public IQuery ComputeQuery
        {
            get;set;
        }
    }
}
