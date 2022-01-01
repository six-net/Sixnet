using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Development.Entity
{
    /// <summary>
    /// Defines obsolete entity
    /// </summary>
    public interface IObsoleteEntity
    {
        /// <summary>
        /// Indicates whether is obsolete
        /// </summary>
        bool IsObsolete { get; set; }
    }
}
