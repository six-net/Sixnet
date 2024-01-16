using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Development.Events
{
    /// <summary>
    /// Time sixnet event handler options
    /// </summary>
    /// <remarks>作者: dingbin.li, 时间: 2023/8/31 22:12:58, 版本: 1.0, 描述: 创建</remarks>
    public class TimeSixnetEventHandlerOptions
    {
        /// <summary>
        /// Whether execution async
        /// </summary>
        public bool Async { get; set; }

        /// <summary>
        /// Gets or sets the event trigger time
        /// </summary>
        public EventTriggerTime TriggerTime { get; set; } = EventTriggerTime.Immediately;
    }
}
