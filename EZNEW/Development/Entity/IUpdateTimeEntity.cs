using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Development.Entity
{
    /// <summary>
    /// Defines update time entity
    /// </summary>
    public interface IUpdateTimeEntity
    {
        /// <summary>
        /// Gets or sets the update time
        /// </summary>
        DateTimeOffset UpdateTime { get; set; }
    }
}
