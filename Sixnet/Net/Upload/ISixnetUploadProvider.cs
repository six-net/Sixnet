using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Sixnet.Exceptions;
using Sixnet.Net.Http;

namespace Sixnet.Net.Upload
{
    /// <summary>
    /// Upload provider
    /// </summary>
    public interface ISixnetUploadProvider
    {
        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns></returns>
        UploadResult Upload(UploadParameter parameter);

        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns></returns>
        Task<UploadResult> UploadAsync(UploadParameter parameter);

        /// <summary>
        /// Move file
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns></returns>
        List<string> Move(MoveUploadFileParameter parameter);

        /// <summary>
        /// Move file
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns></returns>
        Task<List<string>> MoveAsync(MoveUploadFileParameter parameter);
    }
}
