using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Domain.Event
{
    /// <summary>
    /// base domain event
    /// </summary>
    public abstract class BaseDomainEvent : IDomainEvent
    {
        #region Propertys

        /// <summary>
        /// event id
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// created date
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        #endregion
    }
}
