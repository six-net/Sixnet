using System.Collections.Generic;
using System.Linq;
using Sixnet.DependencyInjection;
using Sixnet.Exceptions;

namespace Sixnet.Net.Upload
{
    /// <summary>
    /// Upload manager
    /// </summary>
    public static partial class SixnetUploader
    {
        #region Fields

        /// <summary>
        /// Gets or sets the default content folder
        /// </summary>
        internal static string DefaultContentFolder { get; set; } = "wwwroot";
        static readonly ISixnetUploadProvider _localUploadProvider = new DefaultLocalUploadProvider();
        static readonly ISixnetUploadProvider _remoteUploadProvider = new DefaultRemoteUploadProvider();

        #endregion

        #region Methods

        #region Upload

        /// <summary>
        /// Upload
        /// </summary>
        /// <param name="files">Files</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the upload result</returns>
        public static UploadResult Upload(IEnumerable<UploadFile> files, Dictionary<string, string> parameters = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(files.IsNullOrEmpty(), nameof(files));

            var uploadObjectGroups = files.Select(c => c.ObjectName).Distinct().ToList();
            var uploadResult = new UploadResult();
            foreach (var uploadObjectName in uploadObjectGroups)
            {
                var groupFiles = files.Where(c => c.ObjectName == uploadObjectName).ToList();
                var uploadSetting = GetUploadSetting(uploadObjectName);
                uploadResult.Combine(Upload(groupFiles, uploadSetting, parameters));
            }
            return uploadResult;
        }

        /// <summary>
        /// Upload
        /// </summary>
        /// <param name="files">Files</param>
        /// <param name="uploadSetting">Upload setting</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the upload result</returns>
        public static UploadResult Upload(IEnumerable<UploadFile> files, UploadSetting uploadSetting, Dictionary<string, string> parameters = null)
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
            return provider.Upload(uploadParameter);
        }

        #endregion

        #region Gets upload setting

        /// <summary>
        /// Gets upload setting
        /// </summary>
        /// <param name="uploadObjectName">Upload object name</param>
        /// <returns>Return the upload setting</returns>
        static UploadSetting GetUploadSetting(string uploadObjectName)
        {
            UploadSetting uploadSetting = null;
            var uploadOptions = SixnetContainer.GetOptions<UploadOptions>();
            uploadOptions?.UploadObjects?.TryGetValue(uploadObjectName ?? string.Empty, out uploadSetting);
            uploadSetting ??= uploadOptions?.Default ?? new UploadSetting();
            return uploadSetting;
        }

        #endregion

        #region Get upload provider

        static ISixnetUploadProvider GetUploadProvider(UploadParameter parameter)
        {
            var uploadOptions = SixnetContainer.GetOptions<UploadOptions>();
            ISixnetUploadProvider provider = null;
            if (uploadOptions?.GetUploadProvider != null)
            {
                provider = uploadOptions.GetUploadProvider(parameter);
            }
            if (provider == null)
            {
                if (parameter.Setting?.Remote ?? false)
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

        #endregion
    }
}
