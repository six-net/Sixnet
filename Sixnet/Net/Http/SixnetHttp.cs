using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sixnet.DependencyInjection;
using Sixnet.Net.Upload;
using Sixnet.Serialization.Json;

namespace Sixnet.Net.Http
{
    /// <summary>
    /// Http helper
    /// </summary>
    public static partial class SixnetHttp
    {
        #region Fields

        /// <summary>
        /// Http method handlers
        /// </summary>
        static readonly Dictionary<HttpMethod, Action<HttpClient, HttpRequestMessage, HttpRequestOptions>> HttpMethodRequestMessageHandlers = null;

        /// <summary>
        /// Default http client
        /// </summary>
        static HttpClient DefaultHttpClient = null;

        /// <summary>
        /// Default http client handler
        /// </summary>
        private static readonly HttpClientHandler DefaultHttpClientHandler = new HttpClientHandler();

        static SixnetHttp()
        {
            HttpMethodRequestMessageHandlers = new Dictionary<HttpMethod, Action<HttpClient, HttpRequestMessage, HttpRequestOptions>>()
            {
                [HttpMethod.Post] = HttpRequestMessageSetFileAndParameter,
                [HttpMethod.Put] = HttpRequestMessageSetFileAndParameter,
                [new HttpMethod("PATCH")] = HttpRequestMessageSetFileAndParameter,
                [HttpMethod.Get] = AppendUrlParameter,
                [HttpMethod.Delete] = AppendUrlParameter
            };
        }

        #endregion

        #region Configuration

        /// <summary>
        /// Configure default http behavior
        /// </summary>
        /// <param name="configure">Configure http client handler</param>
        public static void ConfigureDefaultHttpBehavior(Action<HttpClientHandler> configure)
        {
            configure?.Invoke(DefaultHttpClientHandler);
        }

        #endregion

        #region Get http client

        /// <summary>
        /// Get http client
        /// </summary>
        /// <param name="clientConfigName">Client configuration name</param>
        /// <returns>Return a http client</returns>
        static HttpClient GetHttpClient(string clientConfigName = "")
        {
            var httpClientFactory = SixnetContainer.GetService<IHttpClientFactory>();
            if (string.IsNullOrWhiteSpace(clientConfigName) || httpClientFactory == null)
            {
                if (DefaultHttpClient != null)
                {
                    return DefaultHttpClient;
                }
                lock (DefaultHttpClientHandler)
                {
                    if (DefaultHttpClient != null)
                    {
                        return DefaultHttpClient;
                    }
                    DefaultHttpClient = new HttpClient(DefaultHttpClientHandler);
                }
                return DefaultHttpClient;
            }
            return string.IsNullOrWhiteSpace(clientConfigName) ? httpClientFactory.CreateClient() : httpClientFactory.CreateClient(clientConfigName);
        }

        #endregion

        #region Send

        /// <summary>
        /// Send http request
        /// </summary>
        /// <param name="httpRequestOptions">Http request options</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Send(HttpRequestOptions httpRequestOptions)
        {
            return SendAsync(httpRequestOptions).Result;
        }

        /// <summary>
        /// Send http request
        /// </summary>
        /// <param name="httpMethod">Http method</param>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <param name="files">Files</param>
        /// <param name="completionOption">Completion option</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Return thee http response message</returns>
        static HttpResponseMessage Send(HttpMethod httpMethod, string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token, IDictionary<string, byte[]> files, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            return SendAsync(httpMethod, httpClientConfigName, url, parameters, headers, token, files, completionOption, cancellationToken).Result;
        }

        #endregion

        #region Get

        /// <summary>
        /// Send a GET request to the specified Uri with an HTTP completion option and a
        /// cancellation token as an asynchronous operation.
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <param name="completionOption">Completion option</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Get(string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            return Send(HttpMethod.Get, httpClientConfigName, url, parameters, headers, token, null, completionOption, cancellationToken);
        }

        /// <summary>
        /// Send a GET request to the specified Uri with an HTTP completion option
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <param name="completionOption">Completion option</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Get(string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token, HttpCompletionOption completionOption)
        {
            return Get(httpClientConfigName, url, parameters, headers, token, completionOption, CancellationToken.None);
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Get(string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token)
        {
            return Get(httpClientConfigName, url, parameters, headers, token, HttpCompletionOption.ResponseHeadersRead);
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Get(string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers)
        {
            return Get(httpClientConfigName, url, parameters, headers, string.Empty);
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Get(string httpClientConfigName, string url, IDictionary<string, string> parameters, string token)
        {
            return Get(httpClientConfigName, url, parameters, null, token);
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Get(string httpClientConfigName, string url, IDictionary<string, string> parameters)
        {
            return Get(httpClientConfigName, url, parameters, string.Empty);
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Get(string httpClientConfigName, string url, string token)
        {
            return Get(httpClientConfigName, url, null, token);
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Get(string httpClientConfigName, string url)
        {
            return Get(httpClientConfigName, url, string.Empty);
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Get(string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token)
        {
            return Get(string.Empty, url, parameters, headers, token);
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Get(string url, IDictionary<string, string> parameters, IDictionary<string, string> headers)
        {
            return Get(url, parameters, headers, string.Empty);
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Get(string url, IDictionary<string, string> parameters, string token)
        {
            return Get(url, parameters, null, token);
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Get(string url, IDictionary<string, string> parameters)
        {
            return Get(url, parameters, string.Empty);
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="url">Request url</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Get(string url)
        {
            return Get(string.Empty, url);
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static string GetString(string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token)
        {
            using (var responseMessage = Get(string.Empty, url, parameters, headers, token))
            {
                return ReadResponseString(responseMessage);
            }
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <returns>Return the http response message</returns>
        public static string GetString(string url, IDictionary<string, string> parameters, IDictionary<string, string> headers)
        {
            return GetString(url, parameters, headers, string.Empty);
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static string GetString(string url, IDictionary<string, string> parameters, string token)
        {
            return GetString(url, parameters, null, token);
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the http response message</returns>
        public static string GetString(string url, IDictionary<string, string> parameters)
        {
            return GetString(url, parameters, string.Empty);
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static string GetString(string url, string token)
        {
            return GetString(url, null, token);
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="url">Request url</param>
        /// <returns>Return the http response message</returns>
        public static string GetString(string url)
        {
            return GetString(url, string.Empty);
        }

        /// <summary>
        /// Send a Get request to get data
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <returns>Return bytes</returns>
        public static byte[] GetData(string httpClientConfigName, string url, IDictionary<string, string> parameters = null, IDictionary<string, string> headers = null, string token = "")
        {
            return GetDataAsync(httpClientConfigName, url, parameters, headers, token).Result;
        }

        /// <summary>
        /// Send a Get request to get data
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <returns>Return bytes</returns>
        public static byte[] GetData(string url, IDictionary<string, string> parameters = null, IDictionary<string, string> headers = null, string token = "")
        {
            return GetData(string.Empty, url, parameters, headers, token);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Send a DELETE request to the specified Uri with an HTTP completion option and a
        /// cancellation token as an asynchronous operation.
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <param name="completionOption">Completion option</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Delete(string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            return Send(HttpMethod.Delete, httpClientConfigName, url, parameters, headers, token, null, completionOption, cancellationToken);
        }

        /// <summary>
        /// Send a DELETE request to the specified Uri with an HTTP completion option
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <param name="completionOption">Completion option</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Delete(string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token, HttpCompletionOption completionOption)
        {
            return Delete(httpClientConfigName, url, parameters, headers, token, completionOption, CancellationToken.None);
        }

        /// <summary>
        /// Send a DELETE request to the specified Uri
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Delete(string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token)
        {
            return Delete(httpClientConfigName, url, parameters, headers, token, HttpCompletionOption.ResponseHeadersRead);
        }

        /// <summary>
        /// Send a DELETE request to the specified Uri
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Delete(string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers)
        {
            return Delete(httpClientConfigName, url, parameters, headers, string.Empty);
        }

        /// <summary>
        /// Send a DELETE request to the specified Uri
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Delete(string httpClientConfigName, string url, IDictionary<string, string> parameters, string token)
        {
            return Delete(httpClientConfigName, url, parameters, null, token);
        }

        /// <summary>
        /// Send a DELETE request to the specified Uri
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Delete(string httpClientConfigName, string url, IDictionary<string, string> parameters)
        {
            return Delete(httpClientConfigName, url, parameters, string.Empty);
        }

        /// <summary>
        /// Send a DELETE request to the specified Uri
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Delete(string httpClientConfigName, string url, string token)
        {
            return Delete(httpClientConfigName, url, null, token);
        }

        /// <summary>
        /// Send a DELETE request to the specified Uri
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Delete(string httpClientConfigName, string url)
        {
            return Delete(httpClientConfigName, url, string.Empty);
        }

        /// <summary>
        /// Send a DELETE request to the specified Uri
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Delete(string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token)
        {
            return Delete(string.Empty, url, parameters, headers, token);
        }

        /// <summary>
        /// Send a DELETE request to the specified Uri
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Delete(string url, IDictionary<string, string> parameters, IDictionary<string, string> headers)
        {
            return Delete(url, parameters, headers, string.Empty);
        }

        /// <summary>
        /// Send a DELETE request to the specified Uri
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Delete(string url, IDictionary<string, string> parameters, string token)
        {
            return Delete(url, parameters, null, token);
        }

        /// <summary>
        /// Send a DELETE request to the specified Uri
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Delete(string url, IDictionary<string, string> parameters)
        {
            return Delete(url, parameters, string.Empty);
        }

        /// <summary>
        /// Send a DELETE request to the specified Uri
        /// </summary>
        /// <param name="url">Request url</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Delete(string url)
        {
            return Delete(string.Empty, url);
        }

        #endregion

        #region Post

        /// <summary>
        /// Send a POST request with a cancellation token as an asynchronous operation.
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <param name="files">Files</param>
        /// <param name="completionOption">Completion option</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Post(string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token, IDictionary<string, byte[]> files, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            return Send(HttpMethod.Post, httpClientConfigName, url, parameters, headers, token, files, completionOption, cancellationToken);
        }

        /// <summary>
        /// Send a POST request
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <param name="files">Files</param>
        /// <param name="completionOption">Completion option</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Post(string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token, IDictionary<string, byte[]> files, HttpCompletionOption completionOption)
        {
            return Post(httpClientConfigName, url, parameters, headers, token, files, completionOption, CancellationToken.None);
        }

        /// <summary>
        /// Send a POST request
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <param name="files">Files</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Post(string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token, IDictionary<string, byte[]> files)
        {
            return Post(httpClientConfigName, url, parameters, headers, token, files, HttpCompletionOption.ResponseHeadersRead);
        }

        /// <summary>
        /// Send a POST request
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Post(string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token)
        {
            return Post(httpClientConfigName, url, parameters, headers, token, null);
        }

        /// <summary>
        /// Send a POST request
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <param name="files">Files</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Post(string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token, IDictionary<string, byte[]> files)
        {
            return Post(string.Empty, url, parameters, headers, token, files);
        }

        /// <summary>
        /// Send a POST request
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Post(string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token)
        {
            return Post(url, parameters, headers, token, null);
        }

        /// <summary>
        /// Send a POST request
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Post(string url, IDictionary<string, string> parameters, string token)
        {
            return Post(url, parameters, null, token);
        }

        /// <summary>
        /// Send a POST request
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Post(string url, IDictionary<string, string> parameters)
        {
            return Post(url, parameters, string.Empty);
        }

        /// <summary>
        /// Send a POST request
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Post(string url, string token)
        {
            return Post(url, null, token);
        }

        /// <summary>
        /// Send a POST request
        /// </summary>
        /// <param name="url">Request url</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Post(string url)
        {
            return Post(url, string.Empty);
        }

        /// <summary>
        /// Send a POST request using JSON data
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Url</param>
        /// <param name="jsonData">Json data</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage PostJson(string httpClientConfigName, string url, string jsonData, string token = "")
        {
            var content = new StringContent(jsonData, Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
            return Send(new HttpRequestOptions()
            {
                HttpClientConfigName = httpClientConfigName,
                HttpCompletionOption = HttpCompletionOption.ResponseHeadersRead,
                Token = token,
                HttpRequestMessage = new HttpRequestMessage()
                {
                    Content = content,
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Post
                }
            });
        }

        /// <summary>
        /// Send a POST request using JSON data
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Url</param>
        /// <param name="jsonData">Json data</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static TResult PostJson<TResult>(string httpClientConfigName, string url, string jsonData, string token = "")
        {
            var response = PostJson(httpClientConfigName, url, jsonData, token);
            var valueAsString = ReadResponseString(response);
            if (string.IsNullOrWhiteSpace(valueAsString))
            {
                return default;
            }
            return SixnetJsonSerializer.Deserialize<TResult>(valueAsString);
        }

        /// <summary>
        /// Send a POST request using JSON data
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="jsonData">Json data</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage PostJson(string url, string jsonData, string token = "")
        {
            return PostJson(string.Empty, url, jsonData, token);
        }

        /// <summary>
        /// Send a POST request using JSON data
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="jsonData">Json data</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static TResult PostJson<TResult>(string url, string jsonData, string token = "")
        {
            return PostJson<TResult>(string.Empty, url, jsonData, token);
        }

        /// <summary>
        /// Send a POST request using object
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="data">Request data</param>
        /// <returns>Return http response message</returns>
        public static HttpResponseMessage PostJson(string url, object data, string token = "")
        {
            string jsonData = string.Empty;
            if (data != null)
            {
                jsonData = SixnetJsonSerializer.Serialize(data);
            }
            return PostJson(url, jsonData, token);
        }

        /// <summary>
        /// Send a POST request using object
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="data">Request data</param>
        /// <returns>Return http response message</returns>
        public static TResult PostJson<TResult>(string url, object data, string token = "")
        {
            string jsonData = string.Empty;
            if (data != null)
            {
                jsonData = SixnetJsonSerializer.Serialize(data);
            }
            return PostJson<TResult>(url, jsonData, token);
        }

        /// <summary>
        /// Upload file by POST
        /// </summary>
        /// <param name="httpClientConfigName">httpClientConfigName</param>
        /// <param name="url">Url</param>
        /// <param name="files">Files</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <returns>Return upload result</returns>
        public static UploadResult Upload(string httpClientConfigName, string url, IDictionary<string, byte[]> files, IDictionary<string, string> parameters = null, IDictionary<string, string> headers = null, string token = "")
        {
            var response = Post(httpClientConfigName, url, parameters, headers, token, files);
            string valueAsString = ReadResponseString(response);
            var result = SixnetJsonSerializer.Deserialize<UploadResult>(valueAsString);
            result?.Files?.ForEach(file =>
            {
                file.Target = UploadTarget.Remote;
            });
            return result;
        }

        /// <summary>
        /// Upload file by POST
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="files">Files</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <returns>Return upload result</returns>
        public static UploadResult Upload(string url, IDictionary<string, byte[]> files, IDictionary<string, string> parameters = null, IDictionary<string, string> headers = null, string token = "")
        {
            return Upload(string.Empty, url, files, parameters, headers, token);
        }

        /// <summary>
        /// Upload file by POST
        /// </summary>
        /// <param name="httpClientConfigName">httpClientConfigName</param>
        /// <param name="url">Url</param>
        /// <param name="file">File</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <returns>Return upload result</returns>
        public static UploadResult Upload(string httpClientConfigName, string url, byte[] file, object parameters, IDictionary<string, string> headers = null, string token = "")
        {
            if (file == null || file.Length <= 0)
            {
                return UploadResult.FailResult("No file is specified for upload");
            }
            Dictionary<string, string> parameterDict = null;
            if (parameters != null)
            {
                parameterDict = parameters.ToStringDictionary();
            }
            return Upload(httpClientConfigName, url, new Dictionary<string, byte[]>() { { "file1", file } }, parameterDict, headers, token);
        }

        /// <summary>
        /// Upload file by POST
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="file">File</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <returns>Return upload result</returns>
        public static UploadResult Upload(string url, byte[] file, object parameters, IDictionary<string, string> headers = null, string token = "")
        {
            return Upload(string.Empty, url, file, parameters, headers, token);
        }

        #endregion

        #region Put

        /// <summary>
        /// Send a PUT request with a cancellation token as an asynchronous operation.
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <param name="files">Files</param>
        /// <param name="completionOption">Completion option</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Put(string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token, IDictionary<string, byte[]> files, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            return Send(HttpMethod.Put, httpClientConfigName, url, parameters, headers, token, files, completionOption, cancellationToken);
        }

        /// <summary>
        /// Send a PUT request
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <param name="files">Files</param>
        /// <param name="completionOption">Completion option</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Put(string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token, IDictionary<string, byte[]> files, HttpCompletionOption completionOption)
        {
            return Put(httpClientConfigName, url, parameters, headers, token, files, completionOption, CancellationToken.None);
        }

        /// <summary>
        /// Send a PUT request
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <param name="files">Files</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Put(string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token, IDictionary<string, byte[]> files)
        {
            return Put(httpClientConfigName, url, parameters, headers, token, files, HttpCompletionOption.ResponseHeadersRead);
        }

        /// <summary>
        /// Send a PUT request
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Put(string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token)
        {
            return Put(httpClientConfigName, url, parameters, headers, token, null);
        }

        /// <summary>
        /// Send a PUT request
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <param name="files">Files</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Put(string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token, IDictionary<string, byte[]> files)
        {
            return Put(string.Empty, url, parameters, headers, token, files);
        }

        /// <summary>
        /// Send a PUT request
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Put(string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token)
        {
            return Put(url, parameters, headers, token, null);
        }

        /// <summary>
        /// Send a PUT request
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Put(string url, IDictionary<string, string> parameters, string token)
        {
            return Put(url, parameters, null, token);
        }

        /// <summary>
        /// Send a PUT request
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Put(string url, IDictionary<string, string> parameters)
        {
            return Put(url, parameters, string.Empty);
        }

        /// <summary>
        /// Send a PUT request
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Put(string url, string token)
        {
            return Put(url, null, token);
        }

        /// <summary>
        /// Send a PUT request
        /// </summary>
        /// <param name="url">Request url</param>
        /// <returns>Return the http response message</returns>
        public static HttpResponseMessage Put(string url)
        {
            return Put(url, string.Empty);
        }

        #endregion

        #region Util

        #region Handle http request options

        static HttpRequestOptions HandleHttpRequestOptions(HttpClient httpClient, HttpRequestOptions httpRequestOptions)
        {
            httpRequestOptions.HttpRequestMessage ??= new HttpRequestMessage();
            var httpRequestMessage = httpRequestOptions.HttpRequestMessage;
            if (HttpMethodRequestMessageHandlers.TryGetValue(httpRequestMessage.Method, out var handler) && handler != null)
            {
                handler(httpClient, httpRequestMessage, httpRequestOptions);
            }
            //headers
            if (!httpRequestOptions.Headers.IsNullOrEmpty())
            {
                foreach (var header in httpRequestOptions.Headers)
                {
                    httpRequestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
            //token
            if (httpRequestMessage.Headers.Authorization == null && !string.IsNullOrWhiteSpace(httpRequestOptions.Token))
            {
                httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(httpRequestOptions.AuthorizationScheme, httpRequestOptions.Token);
            }
            return httpRequestOptions;
        }

        #endregion

        #region Set file and parameter

        /// <summary>
        /// Set file and parameter
        /// </summary>
        /// <param name="httpClient">Http client</param>
        /// <param name="httpRequestMessage">Http request message</param>
        /// <param name="httpRequestOptions">Http request options</param>
        private static void HttpRequestMessageSetFileAndParameter(HttpClient httpClient, HttpRequestMessage httpRequestMessage, HttpRequestOptions httpRequestOptions)
        {
            if (httpRequestMessage == null || httpRequestOptions == null)
            {
                return;
            }
            var originalContent = httpRequestMessage.Content;
            MultipartFormDataContent multipartFormDataContent = originalContent as MultipartFormDataContent;
            if ((!httpRequestOptions.Files.IsNullOrEmpty() || !httpRequestOptions.Parameters.IsNullOrEmpty()) && multipartFormDataContent == null)
            {
                multipartFormDataContent = new MultipartFormDataContent();
                if (originalContent != null)
                {
                    multipartFormDataContent.Add(originalContent);
                }
            }

            if (multipartFormDataContent == null)
            {
                return;
            }

            #region Files

            if (!httpRequestOptions.Files.IsNullOrEmpty())
            {
                int fileCount = 0;
                foreach (var item in httpRequestOptions.Files)
                {
                    if (item.Value == null || item.Value.Length <= 0)
                    {
                        continue;
                    }
                    HttpContent content = new StreamContent(new MemoryStream(item.Value));
                    multipartFormDataContent.Add(content, "file" + fileCount.ToString(), item.Key);
                    fileCount++;
                }
            }

            #endregion

            #region Parameters

            if (!httpRequestOptions.Parameters.IsNullOrEmpty())
            {
                foreach (string key in httpRequestOptions.Parameters.Keys)
                {
                    var stringContent = new StringContent(httpRequestOptions.Parameters[key]);
                    multipartFormDataContent.Add(stringContent, key);
                }
            }

            #endregion

            httpRequestMessage.Content = multipartFormDataContent;
        }

        #endregion

        #region Append url parameters

        /// <summary>
        /// Append url parameters
        /// </summary>
        /// <param name="httpClient">Http client</param>
        /// <param name="httpRequestMessage">Http request message</param>
        /// <param name="httpRequestOption">Http request option</param>
        static void AppendUrlParameter(HttpClient httpClient, HttpRequestMessage httpRequestMessage, HttpRequestOptions httpRequestOption)
        {
            if (httpRequestMessage == null || httpRequestOption == null || httpRequestOption.Parameters.IsNullOrEmpty())
            {
                return;
            }

            string url = httpRequestMessage.RequestUri?.AbsoluteUri;
            if (string.IsNullOrWhiteSpace(url))
            {
                url = httpClient.BaseAddress?.AbsoluteUri;
            }
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException($"{nameof(httpRequestMessage.RequestUri)} is null or empty");
            }

            List<string> queryParameters = new List<string>();
            queryParameters.AddRange(httpRequestOption.Parameters.Select(c => string.Format("{0}={1}", c.Key, c.Value)));
            url = url.Trim('?', '&', '/');
            url = $"{url}{(url.IndexOf('?') > 0 ? "&" : "?")}{string.Join("&", queryParameters)}";
            httpRequestMessage.RequestUri = new Uri(url);
        }

        #endregion   

        #region Read string

        /// <summary>
        /// Read string from response message
        /// </summary>
        /// <param name="responseMessage">Response message</param>
        /// <returns>Return the string value</returns>
        static string ReadResponseString(HttpResponseMessage responseMessage)
        {
            return ReadResponseStringAsync(responseMessage).Result;
        }

        #endregion

        #endregion
    }
}
