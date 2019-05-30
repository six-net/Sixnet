using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Domain.Repository.Event
{
    /// <summary>
    /// event handler
    /// </summary>
    public interface IRepositoryEventHandler
    {
        /// <summary>
        /// event type
        /// </summary>
        EventType EventType { get; set; }

        /// <summary>
        /// handler repository type
        /// </summary>
        Type HandlerRepositoryType { get; set; }

        /// <summary>
        /// object type
        /// </summary>
        Type ObjectType { get; set; }
    }
}
