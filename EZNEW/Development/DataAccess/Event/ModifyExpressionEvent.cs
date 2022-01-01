using System;
using System.Collections.Generic;
using EZNEW.Data.Modification;
using EZNEW.Development.Query;

namespace EZNEW.Development.DataAccess.Event
{
    /// <summary>
    /// Modify expression event
    /// </summary>
    [Serializable]
    public class ModifyExpressionEvent : BaseDataAccessEvent
    {
        public ModifyExpressionEvent()
        {
            EventType = DataAccessEventType.ModifyExpression;
        }

        /// <summary>
        /// Gets or sets the modify values
        /// </summary>
        public Dictionary<string, IModificationValue> ModifyValues { get; set; }

        /// <summary>
        /// Gets or sets the query object
        /// </summary>
        public IQuery Query { get; set; }
    }
}
