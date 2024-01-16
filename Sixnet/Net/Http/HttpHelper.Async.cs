using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using Sixnet.Net.Upload;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Linq;
using Sixnet.Serialization;

namespace Sixnet.Net.Http
{
    /// <summary>
    /// Http helper
    /// </summary>
    public static partial class HttpHelper
    {
        #region Send

        /// <summary>
        /// Send http request
        /// </summary>
        /// <param name="httpRequestOptions">Http request options</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> SendAsync(HttpRequestOptions httpRequestOptions)
        {
            var httpClient = GetHttpClient(httpRequestOptions?.HttpClientConfigName);
            httpRequestOptions = HandleHttpRequestOptions(httpClient, httpRequestOptions);
            return await httpClient.SendAsync(httpRequestOptions.HttpRequestMessage, httpRequestOptions.HttpCompletionOption, httpRequestOptions.CancellationToken).ConfigureAwait(false);
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
        static async Task<HttpResponseMessage> SendAsync(HttpMethod httpMethod, string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token, IDictionary<string, byte[]> files, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = httpMethod,
                RequestUri = string.IsNullOrWhiteSpace(url) ? null : new Uri(url, UriKind.RelativeOrAbsolute),
            };
            return await SendAsync(new HttpRequestOptions()
            {
                CancellationToken = cancellationToken,
                HttpClientConfigName = httpClientConfigName,
                HttpCompletionOption = completionOption,
                Parameters = parameters,
                Headers = headers,
                Files = files,
                Token = token,
                HttpRequestMessage = httpRequestMessage
            }).ConfigureAwait(false);
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
        public static async Task<HttpResponseMessage> GetAsync(string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            return await SendAsync(HttpMethod.Get, httpClientConfigName, url, parameters, headers, token, null, completionOption, cancellationToken).ConfigureAwait(false);
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
        public static async Task<HttpResponseMessage> GetAsync(string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token, HttpCompletionOption completionOption)
        {
            return await GetAsync(httpClientConfigName, url, parameters, headers, token, completionOption, CancellationToken.None).ConfigureAwait(false);
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
        public static async Task<HttpResponseMessage> GetAsync(string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token)
        {
            return await GetAsync(httpClientConfigName, url, parameters, headers, token, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> GetAsync(string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers)
        {
            return await GetAsync(httpClientConfigName, url, parameters, headers, string.Empty).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> GetAsync(string httpClientConfigName, string url, IDictionary<string, string> parameters, string token)
        {
            return await GetAsync(httpClientConfigName, url, parameters, null, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> GetAsync(string httpClientConfigName, string url, IDictionary<string, string> parameters)
        {
            return await GetAsync(httpClientConfigName, url, parameters, string.Empty).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> GetAsync(string httpClientConfigName, string url, string token)
        {
            return await GetAsync(httpClientConfigName, url, null, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> GetAsync(string httpClientConfigName, string url)
        {
            return await GetAsync(httpClientConfigName, url, string.Empty).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> GetAsync(string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token)
        {
            return await GetAsync(string.Empty, url, parameters, headers, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> GetAsync(string url, IDictionary<string, string> parameters, IDictionary<string, string> headers)
        {
            return await GetAsync(url, parameters, headers, string.Empty).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> GetAsync(string url, IDictionary<string, string> parameters, string token)
        {
            return await GetAsync(url, parameters, null, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> GetAsync(string url, IDictionary<string, string> parameters)
        {
            return await GetAsync(url, parameters, string.Empty).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="url">Request url</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> GetAsync(string url)
        {
            return await GetAsync(string.Empty, url).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static async Task<string> GetStringAsync(string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token)
        {
            using (var responseMessage = await GetAsync(string.Empty, url, parameters, headers, token).ConfigureAwait(false))
            {
                return await ReadResponseStringAsync(responseMessage).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <returns>Return the http response message</returns>
        public static async Task<string> GetStringAsync(string url, IDictionary<string, string> parameters, IDictionary<string, string> headers)
        {
            return await GetStringAsync(url, parameters, headers, string.Empty).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static async Task<string> GetStringAsync(string url, IDictionary<string, string> parameters, string token)
        {
            return await GetStringAsync(url, parameters, null, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the http response message</returns>
        public static async Task<string> GetStringAsync(string url, IDictionary<string, string> parameters)
        {
            return await GetStringAsync(url, parameters, string.Empty).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static async Task<string> GetStringAsync(string url, string token)
        {
            return await GetStringAsync(url, null, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a GET request to the specified Uri
        /// </summary>
        /// <param name="url">Request url</param>
        /// <returns>Return the http response message</returns>
        public static async Task<string> GetStringAsync(string url)
        {
            return await GetStringAsync(url, string.Empty).ConfigureAwait(false);
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
        public static async Task<byte[]> GetDataAsync(string httpClientConfigName, string url, IDictionary<string, string> parameters = null, IDictionary<string, string> headers = null, string token = "")
        {
            var response = await GetAsync(httpClientConfigName, url, parameters, headers, token).ConfigureAwait(false);
            if (response?.Content == null)
            {
                return Array.Empty<byte>();
            }
            return await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Send a Get request to get data
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <returns>Return bytes</returns>
        public static async Task<byte[]> GetDataAsync(string url, IDictionary<string, string> parameters = null, IDictionary<string, string> headers = null, string token = "")
        {
            return await GetDataAsync(string.Empty, url, parameters, headers, token).ConfigureAwait(false);
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
        public static async Task<HttpResponseMessage> DeleteAsync(string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            return await SendAsync(HttpMethod.Delete, httpClientConfigName, url, parameters, headers, token, null, completionOption, cancellationToken).ConfigureAwait(false);
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
        public static async Task<HttpResponseMessage> DeleteAsync(string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token, HttpCompletionOption completionOption)
        {
            return await DeleteAsync(httpClientConfigName, url, parameters, headers, token, completionOption, CancellationToken.None).ConfigureAwait(false);
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
        public static async Task<HttpResponseMessage> DeleteAsync(string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token)
        {
            return await DeleteAsync(httpClientConfigName, url, parameters, headers, token, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a DELETE request to the specified Uri
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> DeleteAsync(string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers)
        {
            return await DeleteAsync(httpClientConfigName, url, parameters, headers, string.Empty).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a DELETE request to the specified Uri
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> DeleteAsync(string httpClientConfigName, string url, IDictionary<string, string> parameters, string token)
        {
            return await DeleteAsync(httpClientConfigName, url, parameters, null, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a DELETE request to the specified Uri
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> DeleteAsync(string httpClientConfigName, string url, IDictionary<string, string> parameters)
        {
            return await DeleteAsync(httpClientConfigName, url, parameters, string.Empty).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a DELETE request to the specified Uri
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> DeleteAsync(string httpClientConfigName, string url, string token)
        {
            return await DeleteAsync(httpClientConfigName, url, null, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a DELETE request to the specified Uri
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Request url</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> DeleteAsync(string httpClientConfigName, string url)
        {
            return await DeleteAsync(httpClientConfigName, url, string.Empty).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a DELETE request to the specified Uri
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> DeleteAsync(string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token)
        {
            return await DeleteAsync(string.Empty, url, parameters, headers, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a DELETE request to the specified Uri
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> DeleteAsync(string url, IDictionary<string, string> parameters, IDictionary<string, string> headers)
        {
            return await DeleteAsync(url, parameters, headers, string.Empty).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a DELETE request to the specified Uri
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> DeleteAsync(string url, IDictionary<string, string> parameters, string token)
        {
            return await DeleteAsync(url, parameters, null, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a DELETE request to the specified Uri
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> DeleteAsync(string url, IDictionary<string, string> parameters)
        {
            return await DeleteAsync(url, parameters, string.Empty).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a DELETE request to the specified Uri
        /// </summary>
        /// <param name="url">Request url</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            return await DeleteAsync(string.Empty, url).ConfigureAwait(false);
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
        public static async Task<HttpResponseMessage> PostAsync(string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token, IDictionary<string, byte[]> files, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            return await SendAsync(HttpMethod.Post, httpClientConfigName, url, parameters, headers, token, files, completionOption, cancellationToken).ConfigureAwait(false);
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
        public static async Task<HttpResponseMessage> PostAsync(string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token, IDictionary<string, byte[]> files, HttpCompletionOption completionOption)
        {
            return await PostAsync(httpClientConfigName, url, parameters, headers, token, files, completionOption, CancellationToken.None).ConfigureAwait(false);
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
        public static async Task<HttpResponseMessage> PostAsync(string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token, IDictionary<string, byte[]> files)
        {
            return await PostAsync(httpClientConfigName, url, parameters, headers, token, files, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
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
        public static async Task<HttpResponseMessage> PostAsync(string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token)
        {
            return await PostAsync(httpClientConfigName, url, parameters, headers, token, null).ConfigureAwait(false);
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
        public static async Task<HttpResponseMessage> PostAsync(string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token, IDictionary<string, byte[]> files)
        {
            return await PostAsync(string.Empty, url, parameters, headers, token, files).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a POST request
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> PostAsync(string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token)
        {
            return await PostAsync(url, parameters, headers, token, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a POST request
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> PostAsync(string url, IDictionary<string, string> parameters, string token)
        {
            return await PostAsync(url, parameters, null, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a POST request
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> PostAsync(string url, IDictionary<string, string> parameters)
        {
            return await PostAsync(url, parameters, string.Empty).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a POST request
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> PostAsync(string url, string token)
        {
            return await PostAsync(url, null, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a POST request
        /// </summary>
        /// <param name="url">Request url</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> PostAsync(string url)
        {
            return await PostAsync(url, string.Empty).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a POST request using JSON data
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Url</param>
        /// <param name="jsonData">Json data</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> PostJsonAsync(string httpClientConfigName, string url, string jsonData, string token = "")
        {
            var content = new StringContent(jsonData, Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
            return await SendAsync(new HttpRequestOptions()
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
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a POST request using JSON data
        /// </summary>
        /// <param name="httpClientConfigName">Http client config name</param>
        /// <param name="url">Url</param>
        /// <param name="jsonData">Json data</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static async Task<TResult> PostJsonAsync<TResult>(string httpClientConfigName, string url, string jsonData, string token = "")
        {
            var response = await PostJsonAsync(httpClientConfigName, url, jsonData, token).ConfigureAwait(false);
            var valueAsString = await ReadResponseStringAsync(response).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(valueAsString))
            {
                return default;
            }
            return JsonSerializer.Deserialize<TResult>(valueAsString);
        }

        /// <summary>
        /// Send a POST request using JSON data
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="jsonData">Json data</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> PostJsonAsync(string url, string jsonData, string token = "")
        {
            return await PostJsonAsync(string.Empty, url, jsonData, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a POST request using JSON data
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="jsonData">Json data</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static async Task<TResult> PostJsonAsync<TResult>(string url, string jsonData, string token = "")
        {
            return await PostJsonAsync<TResult>(string.Empty, url, jsonData, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a POST request using object
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="data">Request data</param>
        /// <returns>Return http response message</returns>
        public static async Task<HttpResponseMessage> PostJsonAsync(string url, object data, string token = "")
        {
            string jsonData = string.Empty;
            if (data != null)
            {
                jsonData = JsonSerializer.Serialize(data);
            }
            return await PostJsonAsync(url, jsonData, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a POST request using object
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="data">Request data</param>
        /// <returns>Return http response message</returns>
        public static async Task<TResult> PostJsonAsync<TResult>(string url, object data, string token = "")
        {
            string jsonData = string.Empty;
            if (data != null)
            {
                jsonData = JsonSerializer.Serialize(data);
            }
            return await PostJsonAsync<TResult>(url, jsonData, token).ConfigureAwait(false);
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
        public static async Task<UploadResult> PostUploadAsync(string httpClientConfigName, string url, IDictionary<string, byte[]> files, IDictionary<string, string> parameters = null, IDictionary<string, string> headers = null, string token = "")
        {
            var response = await PostAsync(httpClientConfigName, url, parameters, headers, token, files).ConfigureAwait(false);
            string valueAsString = await ReadResponseStringAsync(response).ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<UploadResult>(valueAsString);
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
        public static async Task<UploadResult> PostUploadAsync(string url, IDictionary<string, byte[]> files, IDictionary<string, string> parameters = null, IDictionary<string, string> headers = null, string token = "")
        {
            return await PostUploadAsync(string.Empty, url, files, parameters, headers, token).ConfigureAwait(false);
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
        public static async Task<UploadResult> PostUploadAsync(string httpClientConfigName, string url, byte[] file, object parameters, IDictionary<string, string> headers = null, string token = "")
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
            return await PostUploadAsync(httpClientConfigName, url, new Dictionary<string, byte[]>() { { "file1", file } }, parameterDict, headers, token).ConfigureAwait(false);
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
        public static async Task<UploadResult> PostUploadAsync(string url, byte[] file, object parameters, IDictionary<string, string> headers = null, string token = "")
        {
            return await PostUploadAsync(string.Empty, url, file, parameters, headers, token).ConfigureAwait(false);
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
        public static async Task<HttpResponseMessage> PutAsync(string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token, IDictionary<string, byte[]> files, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            return await SendAsync(HttpMethod.Put, httpClientConfigName, url, parameters, headers, token, files, completionOption, cancellationToken).ConfigureAwait(false);
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
        public static async Task<HttpResponseMessage> PutAsync(string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token, IDictionary<string, byte[]> files, HttpCompletionOption completionOption)
        {
            return await PutAsync(httpClientConfigName, url, parameters, headers, token, files, completionOption, CancellationToken.None).ConfigureAwait(false);
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
        public static async Task<HttpResponseMessage> PutAsync(string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token, IDictionary<string, byte[]> files)
        {
            return await PutAsync(httpClientConfigName, url, parameters, headers, token, files, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
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
        public static async Task<HttpResponseMessage> PutAsync(string httpClientConfigName, string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token)
        {
            return await PutAsync(httpClientConfigName, url, parameters, headers, token, null).ConfigureAwait(false);
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
        public static async Task<HttpResponseMessage> PutAsync(string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token, IDictionary<string, byte[]> files)
        {
            return await PutAsync(string.Empty, url, parameters, headers, token, files).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a PUT request
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="headers">Headers</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> PutAsync(string url, IDictionary<string, string> parameters, IDictionary<string, string> headers, string token)
        {
            return await PutAsync(url, parameters, headers, token, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a PUT request
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> PutAsync(string url, IDictionary<string, string> parameters, string token)
        {
            return await PutAsync(url, parameters, null, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a PUT request
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> PutAsync(string url, IDictionary<string, string> parameters)
        {
            return await PutAsync(url, parameters, string.Empty).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a PUT request
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="token">Token</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> PutAsync(string url, string token)
        {
            return await PutAsync(url, null, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a PUT request
        /// </summary>
        /// <param name="url">Request url</param>
        /// <returns>Return the http response message</returns>
        public static async Task<HttpResponseMessage> PutAsync(string url)
        {
            return await PutAsync(url, string.Empty).ConfigureAwait(false);
        }

        #endregion

        #region Util

        #region Read string

        /// <summary>
        /// Read string from response message
        /// </summary>
        /// <param name="responseMessage">Response message</param>
        /// <returns>Return the string value</returns>
        static async Task<string> ReadResponseStringAsync(HttpResponseMessage responseMessage)
        {
            if (responseMessage?.Content == null)
            {
                return string.Empty;
            }
            return await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        #endregion

        #endregion
    }
}
