// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Development.Events
{
    public interface ISixnetEventHandlerAttribute
    {
        /// <summary>
        /// Async
        /// </summary>
        bool Async { get; set; }

        /// <summary>
        /// Trigger time
        /// </summary>
        SixnetEventTriggerTime TriggerTime { get; set; }
    }

    /// <summary>
    /// Event handler attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class SixnetEventHandlerAttribute<TEvent>(bool async = false, SixnetEventTriggerTime triggerTime = SixnetEventTriggerTime.Immediately) : Attribute, ISixnetEventHandlerAttribute where TEvent : ISixnetEvent
    {
        /// <summary>
        /// Async
        /// </summary>
        public bool Async { get; set; } = async;

        /// <summary>
        /// Trigger time
        /// </summary>
        public SixnetEventTriggerTime TriggerTime { get; set; } = triggerTime;
    }
}
