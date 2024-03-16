using Sixnet.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sixnet.Development.Message
{
    /// <summary>
    /// Message template
    /// </summary>
    public class MessageTemplate
    {
        /// <summary>
        /// Gets or sets the title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the content
        /// </summary>
        public string Content { get; set; }
    }
}
