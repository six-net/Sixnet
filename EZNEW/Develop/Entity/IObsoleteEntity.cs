using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Entity
{
    /// <summary>
    /// Defines obsolete entity
    /// </summary>
    public interface IObsoleteEntity
    {
        /// <summary>
        /// Indecates whether is obsolete
        /// </summary>
        public bool IsObsolete { get; set; }
    }
}
