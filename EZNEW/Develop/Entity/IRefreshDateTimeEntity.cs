using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Entity
{
    /// <summary>
    /// Defines refresh time entity
    /// </summary>
    public interface IRefreshDateTimeEntity
    {
        /// <summary>
        /// Gets or sets the refresh datetime
        /// </summary>
        public DateTimeOffset RefreshDateTime { get; set; }
    }
}
