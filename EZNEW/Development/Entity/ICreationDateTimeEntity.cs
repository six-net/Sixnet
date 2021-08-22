using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Development.Entity
{
    /// <summary>
    /// Defines creation time entity contract
    /// </summary>
    public interface ICreationDateTimeEntity
    {
        /// <summary>
        /// Gets or sets the creation datetime
        /// </summary>
        DateTimeOffset CreationDateTime { get; set; }
    }
}
