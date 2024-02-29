using Sixnet.Exceptions;
using Sixnet.Net.Http;
using Sixnet.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Net.Upload
{
    public class DefaultRemoteUploadProvider : ISixnetUploadProvider
    {
        public UploadResult Upload(UploadParameter parameter)
        {
            var remoteUploadInfos = GetRemoteUploadInfos(parameter);
            return SixnetHttp.Upload(remoteUploadInfos.Item1, remoteUploadInfos.Item2, remoteUploadInfos.Item3);
        }

        public Task<UploadResult> UploadAsync(UploadParameter parameter)
        {
            var remoteUploadInfos = GetRemoteUploadInfos(parameter);
            return SixnetHttp.UploadAsync(remoteUploadInfos.Item1, remoteUploadInfos.Item2, remoteUploadInfos.Item3);
        }

        (string, Dictionary<string, byte[]>, Dictionary<string, string>) GetRemoteUploadInfos(UploadParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter == null, nameof(parameter));
            SixnetDirectThrower.ThrowArgNullIf(parameter.Setting == null, "Upload setting is null");
            SixnetDirectThrower.ThrowArgNullIf(parameter.Files.IsNullOrEmpty(), "Files is null or empty");

            var remoteParameter = new RemoteUploadParameter()
            {
                Files = parameter.Files
            };
            var remoteUploadSetting = parameter.Setting.GetRemoteSetting();
            parameter.Properties ??= new Dictionary<string, string>();
            parameter.Properties[RemoteUploadParameter.RequestParameterName] = SixnetJsonSerializer.Serialize(remoteParameter);
            var url = remoteUploadSetting.GetUploadUrl();
            return (url, parameter.Files.ToDictionary(c => c.FileName, c => c.FileContent), parameter.Properties);
        }
    }
}
