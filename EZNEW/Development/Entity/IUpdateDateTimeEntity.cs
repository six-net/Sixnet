using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Development.Entity
{
    /// <summary>
    /// Defines update time entity
    /// </summary>
    public interface IUpdateDateTimeEntity
    {
        /// <summary>
        /// Gets or sets the update datetime
        /// </summary>
        public DateTimeOffset UpdateDateTime { get; set; }
    }
}
