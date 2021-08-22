using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Development.Entity
{
    /// <summary>
    /// Defines creation time entity contract
    /// </summary>
    public interface ICreationTimeEntity
    {
        /// <summary>
        /// Gets or sets the creation time
        /// </summary>
        DateTimeOffset CreationTime { get; set; }
    }
}
