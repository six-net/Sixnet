using System;
using System.Collections.Generic;
using Sixnet.Algorithm.Selection;

namespace Sixnet.Net.Upload
{
    /// <summary>
    /// Upload setting
    /// </summary>
    [Serializable]
    public class UploadSetting
    {
        /// <summary>
        /// remote selection provider
        /// </summary>
        SixnetDataSelecter<RemoteUploadSetting> remoteUploadServerSelecter = null;

        /// <summary>
        /// remote server options
        /// </summary>
        List<RemoteUploadSetting> remoteServerOptions = null;

        /// <summary>
        /// Gets or sets whether to use remote upload
        /// </summary>
        public bool Remote { get; set; }

        /// <summary>
        /// Gets or sets file save path
        /// </summary>
        public string SavePath { get; set; }

        /// <summary>
        /// Gets or sets whether save to content root folder
        /// default is true
        /// </summary>
        public bool SaveToContentRoot { get; set; } = true;

        /// <summary>
        /// Gets or sets upload content root folder path
        /// default value is 'wwwroot'
        /// </summary>
        public string ContentRootPath { get; set; } = SixnetUploader.DefaultContentFolder;

        /// <summary>
        /// Gets or sets remote configs
        /// </summary>
        public List<RemoteUploadSetting> RemoteConfigurations
        {
            get
            {
                return remoteServerOptions;
            }
            set
            {
                remoteServerOptions = value;
                remoteUploadServerSelecter = new SixnetDataSelecter<RemoteUploadSetting>(remoteServerOptions);
            }
        }

        /// <summary>
        /// Gets or sets upload server choice pattern
        /// default value is 'Random' pattern
        /// </summary>
        public SelectionMatchPattern RemoteServerChoicePattern { get; set; } = SelectionMatchPattern.EquiprobableRandom;

        /// <summary>
        /// Gets remote setting
        /// </summary>
        /// <returns></returns>
        public RemoteUploadSetting GetRemoteSetting()
        {
            if (RemoteConfigurations.IsNullOrEmpty())
            {
                return null;
            }
            int serverCount = RemoteConfigurations.Count;
            if (serverCount == 1)
            {
                return RemoteConfigurations[0];
            }
            return remoteUploadServerSelecter.Get(RemoteServerChoicePattern);
        }
    }
}
