using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Primitives;

namespace Sixnet.Logging
{
    /// <summary>
    /// Sixnet http request trace message
    /// </summary>
    public class SixnetHttpRequestTraceMessage
    {
        /// <summary>
        /// Gets or sets the address
        /// </summary>
        public SixnetHttpRequestAddressInfo Address { get; set; }

        /// <summary>
        /// Gets or sets the request
        /// </summary>
        public SixnetHttpRequestInfo Request { get; set; }

        /// <summary>
        /// Gets or sets the response
        /// </summary>
        public SixnetHttpResponseInfo Response { get; set; }

        /// <summary>
        /// Gets or sets the exception
        /// </summary>
        public SixnetHttpExceptionInfo Exception { get; set; }

        /// <summary>
        /// Gets or sets the time info
        /// </summary>
        public SixnetHttpRequestTimeInfo Time { get; set; }
    }

    /// <summary>
    /// Sixnet http request info
    /// </summary>
    public class SixnetHttpRequestInfo
    {
        /// <summary>
        /// Gets or sets the request path
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the request method
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the request body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the content length
        /// </summary>
        public long? ContentLength { get; set; }

        /// <summary>
        /// Gets or sets the content type
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the headers
        /// </summary>
        public IDictionary<string, StringValues> Headers { get; set; }
    }

    /// <summary>
    /// Sixnet http response info
    /// </summary>
    public class SixnetHttpResponseInfo
    {
        /// <summary>
        /// Gets or sets the response body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the content length
        /// </summary>
        public long? ContentLength { get; set; }

        /// <summary>
        /// Gets or sets the status code
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the content type
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the headers
        /// </summary>
        public IDictionary<string, StringValues> Headers { get; set; }
    }

    /// <summary>
    /// Sixnet http exception info
    /// </summary>
    public class SixnetHttpExceptionInfo
    {
        public string StackTrace { get; set; }

        public string Message { get; set; }

        [JsonIgnore]
        public Exception OriginalException { set; get; }
    }

    /// <summary>
    /// Sixnet http address info
    /// </summary>
    public class SixnetHttpRequestAddressInfo
    {
        /// <summary>
        /// Gets or sets the client address
        /// </summary>
        public string Client { get; set; }

        /// <summary>
        /// Gets or sets the server address
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the original url
        /// </summary>
        public string RawUrl { get; set; }

        /// <summary>
        /// Gets or sets the protocol
        /// </summary>
        public string Protocol { get; set; }
    }

    /// <summary>
    /// Sixnet http request time info
    /// </summary>
    public class SixnetHttpRequestTimeInfo
    {
        /// <summary>
        /// Gets or sets the request time
        /// </summary>
        public DateTimeOffset RequestTime { get; set; }

        /// <summary>
        /// Gets or sets the response time
        /// </summary>
        public DateTimeOffset ResponseTime { get; set; }

        /// <summary>
        /// Gets or sets the time span
        /// </summary>
        public double TimeSpan { get; set; }
    }
}
