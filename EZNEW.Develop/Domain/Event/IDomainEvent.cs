using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Domain.Event
{
    /// <summary>
    /// domain event
    /// </summary>
    public interface IDomainEvent
    {
        #region Propertys

        /// <summary>
        /// event id
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// created date
        /// </summary>
        DateTime CreatedDate { get; set; }

        #endregion
    }
}
