using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EZNEW.Http;
using EZNEW.Serialization;

namespace EZNEW.Upload
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
        /// <param name="fileOptions">File options</param>
        /// <param name="files">Files</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the upload result</returns>
        public static async Task<UploadResult> UploadByConfigurationAsync(IEnumerable<UploadFile> fileOptions, Dictionary<string, byte[]> files, Dictionary<string, string> parameters = null)
        {
            if (fileOptions.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(fileOptions));
            }
            if (files.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(files));
            }
            if (files == null || files.Count <= 0)
            {
                throw new ArgumentNullException(nameof(files));
            }
            List<string> uploadObjectGroups = fileOptions.Select(c => c.UploadObjectName).Distinct().ToList();
            UploadResult uploadResult = null;
            foreach (var uploadObjectName in uploadObjectGroups)
            {
                var groupFileOptions = fileOptions.Where(c => c.UploadObjectName == uploadObjectName).ToList();
                var groupFiles = files.Where(c => groupFileOptions.Exists(fc => fc.FileName == c.Key)).ToDictionary(c => c.Key, c => c.Value);
                var uploadOptions = GetUploadOptions(uploadObjectName);
                var groupResult = await UploadByOptionsAsync(uploadOptions, groupFileOptions, groupFiles, parameters).ConfigureAwait(false);
                if (groupResult == null)
                {
                    continue;
                }
                if (uploadResult == null)
                {
                    uploadResult = groupResult;
                }
                else
                {
                    uploadResult.Combine(groupResult);
                }
            }
            return uploadResult;
        }

        /// <summary>
        /// Upload by configuration
        /// </summary>
        /// <param name="fileOptions">File options</param>
        /// <param name="files">Files</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the upload result</returns>
        public static UploadResult UploadByConfiguration(IEnumerable<UploadFile> fileOptions, Dictionary<string, byte[]> files, Dictionary<string, string> parameters = null)
        {
            return UploadByConfigurationAsync(fileOptions, files, parameters).Result;
        }

        #endregion

        #region Upload by upload options

        /// <summary>
        /// Upload by upload options
        /// </summary>
        /// <param name="uploadOptions">Upload options</param>
        /// <param name="fileOptions">File options</param>
        /// <param name="files">Files</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the upload result</returns>
        public static async Task<UploadResult> UploadByOptionsAsync(UploadOptions uploadOptions, IEnumerable<UploadFile> fileOptions, Dictionary<string, byte[]> files, Dictionary<string, string> parameters = null)
        {
            if (files == null || files.Count <= 0)
            {
                throw new ArgumentNullException(nameof(files));
            }
            if (uploadOptions == null)
            {
                throw new ArgumentNullException(nameof(uploadOptions));
            }
            UploadResult uploadResult;
            if (uploadOptions.Remote)
            {
                uploadResult = await RemoteUploadAsync(uploadOptions.GetRemoteOption(), fileOptions.ToList(), files, parameters).ConfigureAwait(false);
            }
            else
            {
                uploadResult = await LocalUploadAsync(uploadOptions, fileOptions.ToList(), files).ConfigureAwait(false);
            }
            return uploadResult;
        }

        /// <summary>
        /// Upload by upload option
        /// </summary>
        /// <param name="uploadOption">Upload option</param>
        /// <param name="fileOptions">File options</param>
        /// <param name="files">Files</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the upload result</returns>
        public static UploadResult UploadByOption(UploadOptions uploadOption, IEnumerable<UploadFile> fileOptions, Dictionary<string, byte[]> files, Dictionary<string, string> parameters = null)
        {
            return UploadByOptionsAsync(uploadOption, fileOptions, files, parameters).Result;
        }

        #endregion

        #region Remote upload

        /// <summary>
        /// Remote upload file
        /// </summary>
        /// <param name="remoteOption">Remote option</param>
        /// <param name="fileOptions">File options</param>
        /// <param name="files">Upload files</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the upload result</returns>
        public static async Task<UploadResult> RemoteUploadAsync(RemoteServerOptions remoteOption, List<UploadFile> fileOptions, Dictionary<string, byte[]> files, Dictionary<string, string> parameters = null)
        {
            if (remoteOption == null || string.IsNullOrWhiteSpace(remoteOption.Host))
            {
                throw new ArgumentNullException(nameof(remoteOption));
            }
            if (files.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(files));
            }
            RemoteParameter uploadParameter = new RemoteParameter()
            {
                Files = fileOptions
            };
            parameters ??= new Dictionary<string, string>();
            parameters[RemoteParameter.RequestParameterName] = JsonSerializer.Serialize(uploadParameter);
            string url = remoteOption.GetUploadUrl();
            return await HttpHelper.PostUploadAsync(url, files, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Remote upload file
        /// </summary>
        /// <param name="remoteOption">Remote option</param>
        /// <param name="fileOptions">File options</param>
        /// <param name="files">Upload files</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the upload result</returns>
        public static UploadResult RemoteUpload(RemoteServerOptions remoteOption, List<UploadFile> fileOptions, Dictionary<string, byte[]> files, Dictionary<string, string> parameters = null)
        {
            return RemoteUploadAsync(remoteOption, fileOptions, files, parameters).Result;
        }

        /// <summary>
        /// Remote upload file
        /// </summary>
        /// <param name="remoteOption">Remote option</param>
        /// <param name="fileOption">File option</param>
        /// <param name="file">Upload file</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the upload result</returns>
        public static async Task<UploadResult> RemoteUploadAsync(RemoteServerOptions remoteOption, UploadFile fileOption, byte[] file, object parameters = null)
        {
            if (fileOption == null)
            {
                throw new ArgumentNullException(nameof(fileOption));
            }
            Dictionary<string, string> parameterDic = null;
            if (parameters != null)
            {
                parameterDic = parameters.ObjectToStringDcitionary();
            }
            return await RemoteUploadAsync(remoteOption, new List<UploadFile>() { fileOption }, new Dictionary<string, byte[]>() { { "file1", file } }, parameterDic).ConfigureAwait(false);
        }

        /// <summary>
        /// Remote upload file
        /// </summary>
        /// <param name="remoteOption">remote options</param>
        /// <param name="fileOption">File option</param>
        /// <param name="file">Upload file</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the upload result</returns>
        public static UploadResult RemoteUpload(RemoteServerOptions remoteOption, UploadFile fileOption, byte[] file, object parameters = null)
        {
            return RemoteUploadAsync(remoteOption, fileOption, file, parameters).Result;
        }

        #endregion

        #region Local upload

        /// <summary>
        /// Local upload file
        /// </summary>
        /// <param name="uploadOption">Upload option</param>
        /// <param name="fileOptions">File options</param>
        /// <param name="files">Files</param>
        /// <returns>Return the upload result</returns>
        public static async Task<UploadResult> LocalUploadAsync(UploadOptions uploadOption, List<UploadFile> fileOptions, Dictionary<string, byte[]> files)
        {
            if (fileOptions.IsNullOrEmpty() || files == null || files.Count <= 0)
            {
                return new UploadResult()
                {
                    Code = "400",
                    ErrorMessage = "No upload file configuration or file information is specified",
                    Success = false
                };
            }
            UploadResult result = new UploadResult()
            {
                Success = true,
                Files = new List<UploadFileResult>(0)
            };
            foreach (var fileItem in files)
            {
                string fileName = fileItem.Key;
                var fileConfig = fileOptions.FirstOrDefault(c => c.FileName == fileName);
                if (fileConfig == null)
                {
                    fileConfig = new UploadFile()
                    {
                        FileName = fileName,
                        Rename = false
                    };
                }
                var fileResult = await LocalUploadFileAsync(uploadOption, fileConfig, fileItem.Value).ConfigureAwait(false);
                result.Files.Add(fileResult);
            }
            return result;
        }

        /// <summary>
        /// Local upload file
        /// </summary>
        /// <param name="uploadOption">Upload option</param>
        /// <param name="fileOptions">File options</param>
        /// <param name="files">Files</param>
        /// <returns>Return the upload result</returns>
        public static UploadResult LocalUpload(UploadOptions uploadOption, List<UploadFile> fileOptions, Dictionary<string, byte[]> files)
        {
            return LocalUploadAsync(uploadOption, fileOptions, files).Result;
        }

        /// <summary>
        ///  Local upload file
        /// </summary>
        /// <param name="uploadOption">Upload option</param>
        /// <param name="fileOption">File option</param>
        /// <param name="file">File</param>
        /// <returns>Return the upload result</returns>
        public static async Task<UploadResult> LocalUploadAsync(UploadOptions uploadOption, UploadFile fileOption, byte[] file)
        {
            var fileResult = await LocalUploadFileAsync(uploadOption, fileOption, file).ConfigureAwait(false);
            return new UploadResult()
            {
                Code = "200",
                Success = true,
                Files = new List<UploadFileResult>()
                {
                    fileResult
                }
            };
        }

        /// <summary>
        ///  Local upload file
        /// </summary>
        /// <param name="uploadOption">Upload option</param>
        /// <param name="fileOption">File option</param>
        /// <param name="file">File</param>
        /// <returns>Return the upload result</returns>
        public static UploadResult LocalUpload(UploadOptions uploadOption, UploadFile fileOption, byte[] file)
        {
            return LocalUploadAsync(uploadOption, fileOption, file).Result;
        }

        /// <summary>
        /// Local upload file
        /// </summary>
        /// <param name="uploadOption">Upload option</param>
        /// <param name="fileOption">File option</param>
        /// <param name="file">File</param>
        /// <returns>upload file result</returns>
        static async Task<UploadFileResult> LocalUploadFileAsync(UploadOptions uploadOption, UploadFile fileOption, byte[] file)
        {
            #region verify parameters

            if (uploadOption == null)
            {
                throw new ArgumentNullException(nameof(uploadOption));
            }
            if (fileOption == null)
            {
                throw new ArgumentNullException(nameof(fileOption));
            }

            #endregion

            #region set save path

            string savePath = uploadOption.SavePath ?? string.Empty;
            if (uploadOption.SaveToContentRoot)
            {
                var contentFolder = string.IsNullOrWhiteSpace(uploadOption.ContentRootPath) ? DefaultContentFolder : uploadOption.ContentRootPath;
                savePath = Path.IsPathRooted(savePath) ? Path.Combine(savePath, contentFolder) : Path.Combine(contentFolder, savePath);
            }
            if (!string.IsNullOrWhiteSpace(fileOption.Folder))
            {
                savePath = Path.Combine(savePath, fileOption.Folder);
            }
            string realSavePath = savePath;
            if (!Path.IsPathRooted(realSavePath))
            {
                realSavePath = Path.Combine(Directory.GetCurrentDirectory(), realSavePath);
            }
            if (!Directory.Exists(realSavePath))
            {
                Directory.CreateDirectory(realSavePath);
            }

            #endregion

            #region file suffix

            string suffix = Path.GetExtension(fileOption.FileName).Trim('.');
            if (!string.IsNullOrWhiteSpace(fileOption.Suffix))
            {
                suffix = fileOption.Suffix.Trim('.');
            }

            #endregion

            #region file name

            string fileName = Path.GetFileNameWithoutExtension(fileOption.FileName);
            if (fileOption.Rename)
            {
                fileName = Guid.NewGuid().ToInt64().ToString();
            }
            fileName = string.Format("{0}.{1}", fileName, suffix);

            #endregion

            #region save file

            string fileFullPath = Path.Combine(realSavePath, fileName);
            File.WriteAllBytes(fileFullPath, file);
            string relativePath = Path.Combine(savePath ?? string.Empty, fileName);

            #endregion

            var result = new UploadFileResult()
            {
                FileName = fileName,
                FullPath = fileFullPath,
                Suffix = Path.GetExtension(fileName).Trim('.'),
                RelativePath = relativePath,
                UploadDate = DateTimeOffset.Now,
                OriginalFileName = fileOption.FileName,
                Target = UploadTarget.Local
            };
            return await Task.FromResult(result).ConfigureAwait(false);
        }

        #endregion

        #endregion
    }
}
