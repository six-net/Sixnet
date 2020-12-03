using System;
using System.Collections.Generic;
using System.Text;
using EZNEW.Develop.Command;

namespace EZNEW.Data.Cache.Policy
{
    /// <summary>
    /// Remove data context
    /// </summary>
    public class RemoveDataContext<T>
    {
        /// <summary>
        /// Gets or sets the database command
        /// </summary>
        public ICommand DatabaseCommand { get; set; }

        /// <summary>
        /// Gets or sets the datas
        /// </summary>
        public IEnumerable<T> Datas { get; set; }
    }
}
