// "Company © 2025. All rights reserved."

using System.IO;

using Sixnet.DependencyInjection;
using Sixnet.Exceptions;

namespace Sixnet.IO
{
    /// <summary>
    /// Sixnet file manager
    /// </summary>
    public static partial class SixnetFileManager
    {
        #region files

        internal static string DefaultContentFolder { get; set; } = "wwwroot";
        internal static string DefaultTempFolder { get; set; } = "sntemp";
        static readonly ISixnetUploadProvider _localUploadProvider = new SixnetDefaultLocalUploadProvider();
        static readonly ISixnetUploadProvider _remoteUploadProvider = new SixnetDefaultRemoteUploadProvider();

        #endregion

        #region Access path

        #region Gets file access setting

        /// <summary>
        /// Gets file setting
        /// </summary>
        /// <param name="fileObjectName">File object name</param>
        /// <returns></returns>
        public static SixnetFileSetting GetFileSetting(string fileObjectName)
        {
            SixnetFileSetting fileSetting = null;
            var fileOptions = SixnetContainer.GetOptions<SixnetFileOptions>();
            fileOptions?.Files?.TryGetValue(fileObjectName, out fileSetting);
            fileSetting ??= fileOptions?.Default ?? new SixnetFileSetting();
            return fileSetting;
        }

        #endregion

        #region Get file access path

        /// <summary>
        /// Get file access path
        /// </summary>
        /// <param name="fileObjectName">File object name</param>
        /// <param name="relativePath">File relative path</param>
        /// <returns>Return the file access path</returns>
        public static string GetFileAccessPath(string fileObjectName, string relativePath)
        {
            var fileSetting = GetFileSetting(fileObjectName);
            return GetFileAccessPath(relativePath, fileSetting);
        }

        /// <summary>
        /// Get file access path
        /// </summary>
        /// <param name="relativePath">File path</param>
        /// <param name="fileSetting">File setting</param>
        /// <returns>Return file access path</returns>
        public static string GetFileAccessPath(string relativePath, SixnetFileSetting fileSetting)
        {
            return fileSetting?.GetFileAccessPath(relativePath) ?? relativePath;
        }

        /// <summary>
        /// Get file access path
        /// </summary>
        /// <param name="relativePath">File path</param>
        /// <param name="configure">Configure</param>
        /// <returns>Return file access path</returns>
        public static string GetFileAccessPath(string relativePath, Action<SixnetFileSetting> configure)
        {
            var fileSetting = new SixnetFileSetting();
            configure?.Invoke(fileSetting);
            return fileSetting?.GetFileAccessPath(relativePath) ?? relativePath;
        }

        #endregion

        #endregion

        #region Combine path

        /// <summary>
        /// Combine path
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string CombinePath(params string[] paths)
        {
            if (paths.IsNullOrEmpty())
            {
                return string.Empty;
            }
            var realPaths = new List<string>();
            foreach (var path in paths)
            {
                if (!string.IsNullOrWhiteSpace(path))
                {
                    realPaths.Add(path);
                }
            }
            if (realPaths.IsNullOrEmpty())
            {
                return string.Empty;
            }
            return Path.Combine(realPaths.ToArray());
        }

        #endregion

        #region Upload

        /// <summary>
        /// Upload
        /// </summary>
        /// <param name="files">Files</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the upload result</returns>
        public static SixnetUploadResult Upload(IEnumerable<SixnetUploadFile> files, Dictionary<string, string> parameters = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(files.IsNullOrEmpty(), nameof(files));

            var uploadObjectGroups = files.Select(c => c.ObjectName).Distinct().ToList();
            var uploadResult = SixnetUploadResult.SuccessResult();
            foreach (var uploadObjectName in uploadObjectGroups)
            {
                var groupFiles = files.Where(c => c.ObjectName == uploadObjectName).ToList();
                var fileSetting = GetFileSetting(uploadObjectName);
                uploadResult.Combine(Upload(groupFiles, fileSetting, parameters));
            }
            return uploadResult;
        }

        /// <summary>
        /// Upload
        /// </summary>
        /// <param name="files">Files</param>
        /// <param name="fileSetting">File setting</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the upload result</returns>
        public static SixnetUploadResult Upload(IEnumerable<SixnetUploadFile> files, SixnetFileSetting fileSetting, Dictionary<string, string> parameters = null)
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
            return provider.Upload(uploadParameter);
        }

        /// <summary>
        /// Store upload file
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns></returns>
        public static List<string> StoreUploadedFile(SixnetStoreUploadedFileParameter parameter)
        {
            var uploadParameter = new SixnetUploadParameter()
            {
                Setting = GetFileSetting(parameter?.FileObjectName)
            };
            var provider = GetUploadProvider(uploadParameter);
            return provider.StoreUploadedFile(parameter);
        }

        /// <summary>
        /// Store upload file
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static List<string> StoreUploadedFile(Action<SixnetStoreUploadedFileParameter> configure)
        {
            var parameter = new SixnetStoreUploadedFileParameter();
            configure?.Invoke(parameter);
            return StoreUploadedFile(parameter);
        }

        /// <summary>
        /// Get upload provider
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        internal static ISixnetUploadProvider GetUploadProvider(SixnetUploadParameter parameter)
        {
            var fileOptions = SixnetContainer.GetOptions<SixnetFileOptions>();
            ISixnetUploadProvider provider = null;
            if (fileOptions?.GetUploadProvider != null)
            {
                provider = fileOptions.GetUploadProvider(parameter);
            }
            if (provider == null)
            {
                if (parameter.Setting?.UseRemoteUpload ?? false)
                {
                    provider = _remoteUploadProvider;
                }
                else
                {
                    provider = _localUploadProvider;
                }
            }
            return provider;
        }

        #endregion
    }
}
