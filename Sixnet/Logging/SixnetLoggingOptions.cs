using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Sixnet.Logging
{
    /// <summary>
    /// Logging options
    /// </summary>
    public class SixnetLoggingOptions
    {
        /// <summary>
        /// Gets or sets the http log options
        /// </summary>
        public SixnetHttpLogOptions Http { get; set; }
    }

    /// <summary>
    // Sixnet http request logging options
    /// </summary>
    public class SixnetHttpLogOptions
    {
        /// <summary>
        /// Whether enable record header
        /// </summary>
        public bool RecordHeader { get; set; } = true;

        /// <summary>
        /// Whether record request body
        /// </summary>
        public bool RecordRequestBody { get; set; } = true;

        /// <summary>
        /// Whether record response body
        /// </summary>
        public bool RecordResponseBody { get; set; } = true;

        /// <summary>
        /// Allowed record body content types
        /// Default is "application/json, application/xml, text/xml, text/plain"
        /// </summary>
        public List<string> RecordBodyContentTypes { get; set; } = new List<string> { "application/json", "application/xml", "text/xml", "text/plain" };

        /// <summary>
        /// Gets or sets the request body limit
        /// Less than 0 incates no limit.
        /// Default is -1
        /// </summary>
        public int RequestBodyLimitSize { get; set; } = -1;

        /// <summary>
        /// Gets or sets the response body limit
        /// Less than 0 incates no limit.
        /// Default is -1
        /// </summary>
        public int ResponseBodyLimitSize { get; set; } = -1;

        /// <summary>
        /// Gets or sets the ignore paths
        /// Default includes 'swagger'
        /// </summary>
        public List<string> IgnorePaths { get; set; } = new List<string>() { "swagger" };

        /// <summary>
        /// Whether record root path
        /// </summary>
        public bool RecordRootPath { get; set; }

        /// <summary>
        /// Gets or sets the ignore methods
        /// Default includes 'OPTIONS'
        /// </summary>
        public List<string> IgnoreMethods { get; set; } = new List<string>() { "OPTIONS" };

        /// <summary>
        /// Gets or sets the log level
        /// Default is 'Information'
        /// </summary>
        public LogLevel LogLevel { get; set; } = LogLevel.Information;

        /// <summary>
        /// Gets or sets the encoding
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;
    }
}
