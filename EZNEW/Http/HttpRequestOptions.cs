using System.Collections.Generic;
using System.Net.Http;
using System.Threading;

namespace EZNEW.Http
{
    /// <summary>
    /// Http request message options
    /// </summary>
    public class HttpRequestOptions
    {
        /// <summary>
        /// Gets or sets the http request message
        /// </summary>
        public HttpRequestMessage HttpRequestMessage { get; set; }

        /// <summary>
        /// Gets or sets the http request parameters
        /// </summary>
        public IDictionary<string, string> Parameters { get; set; }

        /// <summary>
        ///  Get
        /// </summary>
        public IDictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Gets or sets the http request files
        /// </summary>
        public IDictionary<string, byte[]> Files { get; set; }

        /// <summary>
        /// Gets or sets the http completion option
        /// </summary>
        public HttpCompletionOption HttpCompletionOption { get; set; }

        /// <summary>
        /// Gets or sets the cancellation token
        /// </summary>
        public CancellationToken CancellationToken { get; set; }

        /// <summary>
        /// Gets or sets the http client config name
        /// </summary>
        public string HttpClientConfigName { get; set; }

        /// <summary>
        /// Gets or sets the authorization token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the authorization scheme
        /// </summary>
        public string AuthorizationScheme { get; set; } = HttpConstants.AuthorizationSchemes.Bearer;
    }
}
