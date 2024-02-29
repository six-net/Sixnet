using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task<UploadResult> UploadAsync(UploadParameter parameter);
    }
}
