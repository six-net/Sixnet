// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Exceptions;
using Sixnet.Net.Http;
using Sixnet.Serialization.Json;

namespace Sixnet.IO
{
    public class SixnetDefaultRemoteUploadProvider : ISixnetUploadProvider
    {
        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public SixnetUploadResult Upload(SixnetUploadParameter parameter)
        {
            var remoteUploadInfos = GetRemoteUploadInfos(parameter);
            return SixnetHttp.Upload(remoteUploadInfos.Item1, remoteUploadInfos.Item2, remoteUploadInfos.Item3);
        }

        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public Task<SixnetUploadResult> UploadAsync(SixnetUploadParameter parameter)
        {
            var remoteUploadInfos = GetRemoteUploadInfos(parameter);
            return SixnetHttp.UploadAsync(remoteUploadInfos.Item1, remoteUploadInfos.Item2, remoteUploadInfos.Item3);
        }

        /// <summary>
        /// Store uploaded file
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns></returns>
        public List<string> StoreUploadedFile(SixnetStoreUploadedFileParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter == null, nameof(parameter));

            var uploadSetting = SixnetFileManager.GetFileSetting(parameter.FileObjectName);
            var remoteUploadSetting = uploadSetting.GetRemoteUploadSetting();
            return SixnetHttp.PostJson<List<string>>(remoteUploadSetting.GetStoreFileUrl(), parameter);
        }

        /// <summary>
        /// Store uploaded file
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns></returns>
        public Task<List<string>> StoreUploadedFileAsync(SixnetStoreUploadedFileParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter == null, nameof(parameter));

            var uploadSetting = SixnetFileManager.GetFileSetting(parameter.FileObjectName);
            var remoteUploadSetting = uploadSetting.GetRemoteUploadSetting();
            return SixnetHttp.PostJsonAsync<List<string>>(remoteUploadSetting.GetStoreFileUrl(), parameter);
        }

        /// <summary>
        /// Get remote upload infos
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        (string, Dictionary<string, byte[]>, Dictionary<string, string>) GetRemoteUploadInfos(SixnetUploadParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter == null, nameof(parameter));
            SixnetDirectThrower.ThrowArgNullIf(parameter.Setting == null, "Upload setting is null");
            SixnetDirectThrower.ThrowArgNullIf(parameter.Files.IsNullOrEmpty(), "Files is null or empty");

            var remoteParameter = new SixnetRemoteUploadParameter()
            {
                Items = parameter.Files
            };
            var remoteUploadSetting = parameter.Setting.GetRemoteUploadSetting();
            parameter.Properties ??= new Dictionary<string, string>();
            // parameter.Properties[SixnetRemoteUploadParameter.RequestParameterName] = SixnetJsonSerializer.Serialize(remoteParameter);
            var url = remoteUploadSetting.GetUploadUrl();
            return (url, null, parameter.Properties); // ToDO
        }
    }
}
