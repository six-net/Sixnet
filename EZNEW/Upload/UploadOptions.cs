using System;
using System.Collections.Generic;
using EZNEW.Selection;

namespace EZNEW.Upload
{
    /// <summary>
    /// Upload option
    /// </summary>
    [Serializable]
    public class UploadOptions
    {
        /// <summary>
        /// remote selection provider
        /// </summary>
        DataSelectionProvider<RemoteServerOptions> remoteSelectionProvider = null;

        /// <summary>
        /// remote server options
        /// </summary>
        List<RemoteServerOptions> remoteServerOptions = null;

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
        public string ContentRootPath { get; set; } = UploadManager.DefaultContentFolder;

        /// <summary>
        /// Gets or sets remote configs
        /// </summary>
        public List<RemoteServerOptions> RemoteConfigurations
        {
            get
            {
                return remoteServerOptions;
            }
            set
            {
                remoteServerOptions = value;
                remoteSelectionProvider = new DataSelectionProvider<RemoteServerOptions>(remoteServerOptions);
            }
        }

        /// <summary>
        /// Gets or sets upload server choice pattern
        /// default value is 'Random' pattern
        /// </summary>
        public SelectMatchMode RemoteServerChoicePattern { get; set; } = SelectMatchMode.EquiprobableRandom;

        /// <summary>
        /// Gets remote option
        /// </summary>
        /// <returns></returns>
        public RemoteServerOptions GetRemoteOption()
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
            return remoteSelectionProvider.Get(RemoteServerChoicePattern);
        }
    }
}
