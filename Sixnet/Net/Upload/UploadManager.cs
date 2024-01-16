using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Sixnet.Development.Data.Event;
using Sixnet.Exceptions;
using Sixnet.Net.Http;
using Sixnet.Serialization;

namespace Sixnet.Net.Upload
{
    /// <summary>
    /// Upload manager
    /// </summary>
    public static class UploadManager
    {
        static UploadManager()
        {
            Default = new UploadOptions()
            {
                Remote = false
            };
        }

        #region Properties

        /// <summary>
        /// default upload option
        /// </summary>
        static UploadOptions Default = null;

        /// <summary>
        /// Upload object configurations
        /// </summary>
        static readonly Dictionary<string, UploadObject> UploadObjectConfigurations = new Dictionary<string, UploadObject>();

        /// <summary>
        /// Gets or sets the default content folder
        /// </summary>
        public static string DefaultContentFolder { get; set; } = "wwwroot";

        #endregion

        #region Methods

        #region Configure upload

        /// <summary>
        /// Configure upload information
        /// </summary>
        /// <param name="uploadConfiguration">Upload configuration</param>
        public static void Configure(UploadConfiguration uploadConfiguration)
        {
            if (uploadConfiguration == null)
            {
                return;
            }
            if (uploadConfiguration.Default != null)
            {
                ConfigureDefault(uploadConfiguration.Default);
            }
            if (!uploadConfiguration.UploadObjects.IsNullOrEmpty())
            {
                ConfigureUploadObject(uploadConfiguration.UploadObjects.ToArray());
            }
        }

        #endregion

        #region Configure default upload

        /// <summary>
        /// Configure default upload option
        /// </summary>
        /// <param name="uploadOptions">Upload options</param>
        public static void ConfigureDefault(UploadOptions uploadOptions)
        {
            Default = uploadOptions;
        }

        #endregion

        #region Configure upload object

        /// <summary>
        /// Configure upload object
        /// </summary>
        /// <param name="uploadObjects">Upload objects</param>
        public static void ConfigureUploadObject(params UploadObject[] uploadObjects)
        {
            if (uploadObjects.IsNullOrEmpty())
            {
                return;
            }
            foreach (var uploadObject in uploadObjects)
            {
                UploadObjectConfigurations[uploadObject.Name] = uploadObject;
            }
        }

        #endregion

        #region Gets upload options

        /// <summary>
        /// Gets upload option
        /// </summary>
        /// <param name="uploadObjectName">Upload object name</param>
        /// <returns>Return the upload option</returns>
        static UploadOptions GetUploadOptions(string uploadObjectName)
        {
            var currentOption = Default;
            if (UploadObjectConfigurations.TryGetValue(uploadObjectName ?? string.Empty, out var uploadObject) || uploadObject?.UploadOption != null)
            {
                currentOption = uploadObject.UploadOption;
            }
            return currentOption;
        }

        #endregion

        #region Upload by configuration

        /// <summary>
        /// Upload by configuration
        /// </summary>
        /// <param name="files">Files</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the upload result</returns>
        public static async Task<UploadResult> UploadByConfigurationAsync(IEnumerable<UploadFile> files, Dictionary<string, string> parameters = null)
        {
            ThrowHelper.ThrowArgNullIf(files.IsNullOrEmpty(), nameof(files));

            var uploadObjectGroups = files.Select(c => c.ObjectName).Distinct().ToList();
            var uploadTasks = new List<Task<UploadResult>>(uploadObjectGroups.Count);
            foreach (var uploadObjectName in uploadObjectGroups)
            {
                var groupFiles = files.Where(c => c.ObjectName == uploadObjectName).ToList();
                var uploadOptions = GetUploadOptions(uploadObjectName);
                uploadTasks.Add(UploadByOptionsAsync(uploadOptions, groupFiles, parameters));
            }
            var uploadResult = new UploadResult();
            uploadResult.Combine(await Task.WhenAll(uploadTasks).ConfigureAwait(false));
            return uploadResult;
        }

        /// <summary>
        /// Upload by configuration
        /// </summary>
        /// <param name="files">Files</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the upload result</returns>
        public static UploadResult UploadByConfiguration(IEnumerable<UploadFile> files, Dictionary<string, string> parameters = null)
        {
            ThrowHelper.ThrowArgNullIf(files.IsNullOrEmpty(), nameof(files));

            var uploadObjectGroups = files.Select(c => c.ObjectName).Distinct().ToList();
            var uploadResult = new UploadResult();
            foreach (var uploadObjectName in uploadObjectGroups)
            {
                var groupFiles = files.Where(c => c.ObjectName == uploadObjectName).ToList();
                var uploadOptions = GetUploadOptions(uploadObjectName);
                uploadResult.Combine(UploadByOptions(uploadOptions, groupFiles, parameters));
            }
            return uploadResult;
        }

        #endregion

        #region Upload by upload options

        /// <summary>
        /// Upload by upload options
        /// </summary>
        /// <param name="uploadOptions">Upload options</param>
        /// <param name="files">Files</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the upload result</returns>
        public static async Task<UploadResult> UploadByOptionsAsync(UploadOptions uploadOptions, IEnumerable<UploadFile> files, Dictionary<string, string> parameters = null)
        {
            ThrowHelper.ThrowArgErrorIf(files.IsNullOrEmpty(), nameof(files));
            ThrowHelper.ThrowArgNullIf(uploadOptions == null, nameof(uploadOptions));
            UploadResult uploadResult;
            if (uploadOptions.Remote)
            {
                uploadResult = await RemoteUploadAsync(uploadOptions.GetRemoteOptions(), files, parameters).ConfigureAwait(false);
            }
            else
            {
                uploadResult = await LocalUploadAsync(files, uploadOptions).ConfigureAwait(false);
            }
            return uploadResult;
        }

        /// <summary>
        /// Upload by upload options
        /// </summary>
        /// <param name="uploadOption">Upload options</param>
        /// <param name="files">Files</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the upload result</returns>
        public static UploadResult UploadByOptions(UploadOptions uploadOptions, IEnumerable<UploadFile> files, Dictionary<string, string> parameters = null)
        {
            ThrowHelper.ThrowArgErrorIf(files.IsNullOrEmpty(), nameof(files));
            ThrowHelper.ThrowArgNullIf(uploadOptions == null, nameof(uploadOptions));
            UploadResult uploadResult;
            if (uploadOptions.Remote)
            {
                uploadResult = RemoteUpload(uploadOptions.GetRemoteOptions(), files, parameters);
            }
            else
            {
                uploadResult = LocalUpload(files, uploadOptions);
            }
            return uploadResult;
        }

        #endregion

        #region Remote upload

        /// <summary>
        /// Remote upload file
        /// </summary>
        /// <param name="remoteServerOptions">Remote server options</param>
        /// <param name="files">Files</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the upload result</returns>
        public static async Task<UploadResult> RemoteUploadAsync(RemoteServerOptions remoteServerOptions, IEnumerable<UploadFile> files, Dictionary<string, string> parameters = null)
        {
            ThrowHelper.ThrowArgNullIf(remoteServerOptions == null, nameof(remoteServerOptions));
            ThrowHelper.ThrowArgNullIf(files.IsNullOrEmpty(), nameof(files));

            var uploadParameter = new RemoteParameter()
            {
                Files = files?.ToList()
            };
            parameters ??= new Dictionary<string, string>();
            parameters[RemoteParameter.RequestParameterName] = JsonSerializer.Serialize(uploadParameter);
            var url = remoteServerOptions.GetUploadUrl();
            return await HttpHelper.PostUploadAsync(url, files.ToDictionary(c => c.FileName, c => c.FileContent), parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Remote upload file
        /// </summary>
        /// <param name="remoteServerOptions">Remote server options</param>
        /// <param name="file">File</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the upload result</returns>
        public static async Task<UploadResult> RemoteUploadAsync(RemoteServerOptions remoteServerOptions, UploadFile file, object parameters = null)
        {
            ThrowHelper.ThrowArgNullIf(file == null, nameof(file));

            return await RemoteUploadAsync(remoteServerOptions, new List<UploadFile>() { file }, parameters?.ToStringDictionary()).ConfigureAwait(false);
        }

        /// <summary>
        /// Remote upload file
        /// </summary>
        /// <param name="remoteServerOptions">Remote server options</param>
        /// <param name="files">Files</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the upload result</returns>
        public static UploadResult RemoteUpload(RemoteServerOptions remoteServerOptions, IEnumerable<UploadFile> files, Dictionary<string, string> parameters = null)
        {
            ThrowHelper.ThrowArgNullIf(remoteServerOptions == null, nameof(remoteServerOptions));
            ThrowHelper.ThrowArgNullIf(files.IsNullOrEmpty(), nameof(files));

            var uploadParameter = new RemoteParameter()
            {
                Files = files?.ToList()
            };
            parameters ??= new Dictionary<string, string>();
            parameters[RemoteParameter.RequestParameterName] = JsonSerializer.Serialize(uploadParameter);
            var url = remoteServerOptions.GetUploadUrl();
            return HttpHelper.PostUpload(url, files.ToDictionary(c => c.FileName, c => c.FileContent), parameters);
        }

        /// <summary>
        /// Remote upload file
        /// </summary>
        /// <param name="remoteServerOptions">Remote server options</param>
        /// <param name="file">Upload file</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the upload result</returns>
        public static UploadResult RemoteUpload(RemoteServerOptions remoteServerOptions, UploadFile file, object parameters = null)
        {
            ThrowHelper.ThrowArgNullIf(file == null, nameof(file));

            return RemoteUpload(remoteServerOptions, new List<UploadFile>() { file }, parameters?.ToStringDictionary());
        }

        #endregion

        #region Local upload

        /// <summary>
        /// Local upload file
        /// </summary>
        /// <param name="files">File options</param>
        /// <param name="uploadOptions">Upload option</param>
        /// <returns>Return the upload result</returns>
        public static async Task<UploadResult> LocalUploadAsync(IEnumerable<UploadFile> files, UploadOptions uploadOptions)
        {
            ThrowHelper.ThrowArgErrorIf(files.IsNullOrEmpty(), "Files is null or empty");

            var uploadTasks = files.Select(f => LocalUploadFileAsync(f, uploadOptions));
            return UploadResult.SuccessResult(await Task.WhenAll(uploadTasks).ConfigureAwait(false));
        }

        /// <summary>
        ///  Local upload file
        /// </summary>
        /// <param name="file">File</param>
        /// <param name="uploadOptions">Upload options</param>
        /// <returns>Return the upload result</returns>
        public static async Task<UploadResult> LocalUploadAsync(UploadFile file, UploadOptions uploadOptions)
        {
            var fileResult = await LocalUploadFileAsync(file, uploadOptions).ConfigureAwait(false);
            return UploadResult.SuccessResult(new List<UploadFileResult>() { fileResult });
        }

        /// <summary>
        /// Local upload file
        /// </summary>
        /// <param name="file">File</param>
        /// <param name="uploadOptions">Upload options</param>
        /// <returns>upload file result</returns>
        static async Task<UploadFileResult> LocalUploadFileAsync(UploadFile file, UploadOptions uploadOptions)
        {
            var fileResult = HandleFile(file, uploadOptions);
            await File.WriteAllBytesAsync(fileResult.FullPath, file.FileContent).ConfigureAwait(false);
            return fileResult;
        }

        /// <summary>
        /// Local upload file
        /// </summary>
        /// <param name="files">Files</param>
        /// <param name="uploadOptions">Upload options</param>
        /// <returns>Return the upload result</returns>
        public static UploadResult LocalUpload(IEnumerable<UploadFile> files, UploadOptions uploadOptions)
        {
            ThrowHelper.ThrowArgErrorIf(files.IsNullOrEmpty(), "Files is null or empty");

            return UploadResult.SuccessResult(files.Select(f => LocalUploadFile(f, uploadOptions)));
        }

        /// <summary>
        ///  Local upload file
        /// </summary>
        /// <param name="file">File</param>
        /// <param name="uploadOptions">Upload option</param>
        /// <returns>Return the upload result</returns>
        public static UploadResult LocalUpload(UploadFile file, UploadOptions uploadOptions)
        {
            return UploadResult.SuccessResult(new List<UploadFileResult>(1) { LocalUploadFile(file, uploadOptions) });
        }

        /// <summary>
        /// Local upload file
        /// </summary>
        /// <param name="file">File</param>
        /// <param name="uploadOptions">Upload options</param>
        /// <returns>upload file result</returns>
        static UploadFileResult LocalUploadFile(UploadFile file, UploadOptions uploadOptions)
        {
            var fileResult = HandleFile(file, uploadOptions);
            File.WriteAllBytes(fileResult.FullPath, file.FileContent);
            return fileResult;
        }

        /// <summary>
        /// Handle file
        /// </summary>
        /// <param name="file">File</param>
        /// <param name="uploadOptions">Upload options</param>
        /// <returns></returns>
        static UploadFileResult HandleFile(UploadFile file, UploadOptions uploadOptions)
        {
            #region verify parameters

            ThrowHelper.ThrowArgNullIf(uploadOptions == null, nameof(uploadOptions));
            ThrowHelper.ThrowArgNullIf(file == null, nameof(file));

            #endregion

            #region set save path

            string relativePath = string.Empty;
            if (uploadOptions.SaveToContentRoot)
            {
                relativePath = string.IsNullOrWhiteSpace(uploadOptions.ContentRootPath) ? DefaultContentFolder : uploadOptions.ContentRootPath;
            }
            string savePath = uploadOptions.SavePath ?? string.Empty;
            if (Path.IsPathRooted(savePath))
            {
                savePath = Path.Combine(savePath, relativePath);
            }
            else
            {
                relativePath = Path.Combine(relativePath, savePath);
                savePath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
            }
            if (!string.IsNullOrWhiteSpace(file.Folder))
            {
                relativePath = Path.Combine(relativePath, file.Folder);
                savePath = Path.Combine(savePath, file.Folder);
            }
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            #endregion

            #region file suffix

            string suffix = Path.GetExtension(file.FileName).Trim('.');
            if (!string.IsNullOrWhiteSpace(file.Suffix))
            {
                suffix = file.Suffix.Trim('.');
            }

            #endregion

            #region file name

            string fileName = Path.GetFileNameWithoutExtension(file.FileName);
            if (file.Rename)
            {
                fileName = Guid.NewGuid().ToInt64().ToString();
            }
            fileName = string.Format("{0}.{1}", fileName, suffix);

            #endregion

            #region save file

            var fileFullPath = Path.Combine(savePath, fileName);
            relativePath = Path.Combine(relativePath, fileName);

            #endregion

            return new UploadFileResult()
            {
                FileName = fileName,
                FullPath = fileFullPath,
                Suffix = Path.GetExtension(fileName).Trim('.'),
                RelativePath = relativePath,
                UploadDate = DateTimeOffset.Now,
                OriginalFileName = file.FileName,
                Target = UploadTarget.Local
            };
        }

        #endregion

        #endregion
    }
}
