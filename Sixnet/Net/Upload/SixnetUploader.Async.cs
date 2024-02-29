using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sixnet.Exceptions;

namespace Sixnet.Net.Upload
{
    public static partial class SixnetUploader
    {
        #region Upload

        /// <summary>
        /// Upload
        /// </summary>
        /// <param name="files">Files</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the upload result</returns>
        public static async Task<UploadResult> UploadAsync(IEnumerable<UploadFile> files, Dictionary<string, string> parameters = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(files.IsNullOrEmpty(), nameof(files));

            var uploadObjectGroups = files.Select(c => c.ObjectName).Distinct().ToList();
            var uploadTasks = new List<Task<UploadResult>>(uploadObjectGroups.Count);
            foreach (var uploadObjectName in uploadObjectGroups)
            {
                var groupFiles = files.Where(c => c.ObjectName == uploadObjectName).ToList();
                var uploadSetting = GetUploadSetting(uploadObjectName);
                uploadTasks.Add(UploadAsync(groupFiles, uploadSetting, parameters));
            }
            var uploadResult = new UploadResult();
            uploadResult.Combine(await Task.WhenAll(uploadTasks).ConfigureAwait(false));
            return uploadResult;
        }

        /// <summary>
        /// Upload
        /// </summary>
        /// <param name="files">Files</param>
        /// <param name="uploadSetting">Upload setting</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the upload result</returns>
        public static Task<UploadResult> UploadAsync(IEnumerable<UploadFile> files, UploadSetting uploadSetting, Dictionary<string, string> parameters = null)
        {
            SixnetDirectThrower.ThrowArgErrorIf(files.IsNullOrEmpty(), nameof(files));
            SixnetDirectThrower.ThrowArgNullIf(uploadSetting == null, nameof(uploadSetting));

            var uploadParameter = new UploadParameter()
            {
                Files = files?.ToList(),
                Properties = parameters,
                Setting = uploadSetting
            };
            var provider = GetUploadProvider(uploadParameter);
            return provider.UploadAsync(uploadParameter);
        }

        #endregion
    }
}
