// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

namespace Sixnet.IO
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
        SixnetUploadResult Upload(SixnetUploadParameter parameter);

        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns></returns>
        Task<SixnetUploadResult> UploadAsync(SixnetUploadParameter parameter);

        /// <summary>
        /// Store uploaded file
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns></returns>
        List<string> StoreUploadedFile(SixnetStoreUploadedFileParameter parameter);

        /// <summary>
        /// Store uploaded file
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns></returns>
        Task<List<string>> StoreUploadedFileAsync(SixnetStoreUploadedFileParameter parameter);
    }
}
