using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Component.Retry
{
    public struct RetryContext
    {
        /// <summary>
        /// Gets or sets the exception
        /// </summary>
        public Exception Exception { get; set; }
    }
}
