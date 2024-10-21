using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sixnet.Exceptions;

namespace Sixnet.IO
{
    public static partial class SixnetFileManager
    {
        #region Upload

        /// <summary>
        /// Upload
        /// </summary>
        /// <param name="files">Files</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the upload result</returns>
        public static async Task<SixnetUploadResult> UploadAsync(IEnumerable<SixnetUploadFile> files, Dictionary<string, string> parameters = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(files.IsNullOrEmpty(), nameof(files));

            var uploadObjectGroups = files.Select(c => c.ObjectName).Distinct().ToList();
            var uploadTasks = new List<Task<SixnetUploadResult>>(uploadObjectGroups.Count);
            foreach (var uploadObjectName in uploadObjectGroups)
            {
                var groupFiles = files.Where(c => c.ObjectName == uploadObjectName).ToList();
                var fileSetting = GetFileSetting(uploadObjectName);
                uploadTasks.Add(UploadAsync(groupFiles, fileSetting, parameters));
            }
            var uploadResult = SixnetUploadResult.SuccessResult();
            uploadResult.Combine(await Task.WhenAll(uploadTasks).ConfigureAwait(false));
            return uploadResult;
        }

        /// <summary>
        /// Upload
        /// </summary>
        /// <param name="files">Files</param>
        /// <param name="fileSetting">Upload setting</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the upload result</returns>
        public static Task<SixnetUploadResult> UploadAsync(IEnumerable<SixnetUploadFile> files, SixnetFileSetting fileSetting, Dictionary<string, string> parameters = null)
        {
            SixnetDirectThrower.ThrowArgErrorIf(files.IsNullOrEmpty(), nameof(files));
            SixnetDirectThrower.ThrowArgNullIf(fileSetting == null, nameof(fileSetting));

            var uploadParameter = new SixnetUploadParameter()
            {
                Files = files?.ToList(),
                Properties = parameters,
                Setting = fileSetting
            };
            var provider = GetUploadProvider(uploadParameter);
            return provider.UploadAsync(uploadParameter);
        }

        /// <summary>
        /// Store upload file
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns></returns>
        public static Task<List<string>> StoreUploadedFileAsync(SixnetStoreUploadedFileParameter parameter)
        {
            var uploadParameter = new SixnetUploadParameter()
            {
                Setting = GetFileSetting(parameter?.ObjectName)
            };
            var provider = GetUploadProvider(uploadParameter);
            return provider.StoreUploadedFileAsync(parameter);
        }

        /// <summary>
        /// Store upload file
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static Task<List<string>> StoreUploadedFileAsync(Action<SixnetStoreUploadedFileParameter> configure)
        {
            var parameter = new SixnetStoreUploadedFileParameter();
            configure?.Invoke(parameter);
            return StoreUploadedFileAsync(parameter);
        }

        #endregion
    }
}
