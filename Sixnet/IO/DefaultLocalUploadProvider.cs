// "Company © 2025. All rights reserved."

using System.IO;
using System.Threading.Tasks;

using Sixnet.Code;
using Sixnet.DependencyInjection;
using Sixnet.Exceptions;

namespace Sixnet.IO
{
    internal class DefaultLocalUploadProvider : ISixnetUploadProvider
    {
        #region Upload

        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public SixnetUploadResult Upload(SixnetUploadParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter == null, nameof(parameter));
            SixnetDirectThrower.ThrowArgErrorIf(parameter.Files.IsNullOrEmpty(), "Files is null or empty");
            var fileOptions = SixnetContainer.GetOptions<SixnetFileOptions>();
            return SixnetUploadResult.SuccessResult(parameter.Files.Select(f => SaveFile(f, fileOptions, parameter.Setting)));
        }

        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public async Task<SixnetUploadResult> UploadAsync(SixnetUploadParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter == null, nameof(parameter));
            SixnetDirectThrower.ThrowArgErrorIf(parameter.Files.IsNullOrEmpty(), "Files is null or empty");

            var fileOptions = SixnetContainer.GetOptions<SixnetFileOptions>();
            var uploadTasks = parameter.Files.Select(f => SaveFileAsync(f, fileOptions, parameter.Setting));
            return SixnetUploadResult.SuccessResult(await Task.WhenAll(uploadTasks).ConfigureAwait(false));
        }

        #endregion

        #region Store

        /// <summary>
        /// Store uploaded file
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns></returns>
        public List<string> StoreUploadedFile(SixnetStoreUploadedFileParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter?.RelativeFilePaths.IsNullOrEmpty() ?? true, nameof(SixnetStoreUploadedFileParameter.RelativeFilePaths));
            var fileSetting = SixnetFileManager.GetFileSetting(parameter.FileObjectName);

            if (fileSetting.UploadToTempFirst)
            {
                var fileOptions = SixnetContainer.GetOptions<SixnetFileOptions>();
                var uploadRootPath = GetUploadRootPath(fileOptions, fileSetting);
                (var storeRelativePath, var storePath) = GetStorePath(parameter.FileObjectName, fileOptions, fileSetting);
                var storedFiles = new List<string>();
                foreach (var file in parameter.RelativeFilePaths)
                {
                    var fileName = Path.GetFileName(file);
                    var orginalFile = SixnetFileManager.CombinePath(uploadRootPath, file);
                    var targetFile = SixnetFileManager.CombinePath(storePath, fileName);
                    storedFiles.Add(SixnetFileManager.CombinePath(storeRelativePath, fileName));
                    if (File.Exists(targetFile) || !File.Exists(orginalFile))
                    {
                        continue;
                    }
                    var targetDir = Path.GetDirectoryName(targetFile);
                    if (!Directory.Exists(targetDir))
                    {
                        Directory.CreateDirectory(targetDir);
                    }
                    File.Move(orginalFile, targetFile);
                }
                return storedFiles;
            }
            return parameter.RelativeFilePaths.Select(c => c).ToList();
        }

        /// <summary>
        /// Store uploaded file
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns></returns>
        public Task<List<string>> StoreUploadedFileAsync(SixnetStoreUploadedFileParameter parameter)
        {
            return Task.FromResult(StoreUploadedFile(parameter));
        }

        #endregion

        #region Save file

        /// <summary>
        /// Save file
        /// </summary>
        /// <param name="file">File</param>
        /// <param name="fileSetting">File setting</param>
        /// <returns>upload file result</returns>
        SixnetUploadFileResult SaveFile(SixnetUploadFile file, SixnetFileOptions fileOptions, SixnetFileSetting fileSetting)
        {
            var fileResult = HandleFile(file, fileOptions, fileSetting);
            File.WriteAllBytes(fileResult.FullPath, file.FileContent);
            return fileResult;
        }

        /// <summary>
        /// Save file
        /// </summary>
        /// <param name="file">File</param>
        /// <param name="fileSetting">File setting</param>
        /// <returns>upload file result</returns>
        async Task<SixnetUploadFileResult> SaveFileAsync(SixnetUploadFile file, SixnetFileOptions uploadOptions, SixnetFileSetting fileSetting)
        {
            var fileResult = HandleFile(file, uploadOptions, fileSetting);
            await File.WriteAllBytesAsync(fileResult.FullPath, file.FileContent).ConfigureAwait(false);
            return fileResult;
        }

        #endregion

        #region Handle file

        /// <summary>
        /// Handle file
        /// </summary>
        /// <param name="file">File</param>
        /// <param name="fileSetting">File setting</param>
        /// <returns></returns>
        SixnetUploadFileResult HandleFile(SixnetUploadFile file, SixnetFileOptions fileOptions, SixnetFileSetting fileSetting)
        {
            SixnetDirectThrower.ThrowArgNullIf(fileSetting == null, nameof(fileSetting));
            SixnetDirectThrower.ThrowArgNullIf(file == null, nameof(file));

            #region save path

            (string relativePath, string savePath) = GetUploadPath(file.ObjectName, fileOptions, fileSetting);

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
            if (fileSetting.Rename)
            {
                fileName = Guid.NewGuid().ToInt64().ToString();
            }
            fileName = string.Format("{0}.{1}", fileName, suffix);

            #endregion

            #region save file

            var fileFullPath = Path.Combine(savePath, fileName);
            relativePath = Path.Combine(relativePath, fileName);

            #endregion

            return new SixnetUploadFileResult()
            {
                FileName = fileName,
                FullPath = fileFullPath,
                Suffix = Path.GetExtension(fileName).Trim('.'),
                RelativePath = relativePath,
                UploadDate = DateTimeOffset.Now,
                OriginalFileName = file.FileName,
                Location = UploadLocation.Local
            };
        }

        /// <summary>
        /// Get upload root path
        /// </summary>
        /// <param name="fileOptions"></param>
        /// <param name="fileSetting"></param>
        /// <returns></returns>
        string GetUploadRootPath(SixnetFileOptions fileOptions, SixnetFileSetting fileSetting)
        {
            var rootPath = fileSetting.UploadPath;
            if (Path.IsPathRooted(rootPath))
            {
                return rootPath;
            }
            rootPath = fileOptions.UploadRootFolder;
            if (Path.IsPathRooted(rootPath))
            {
                return rootPath;
            }
            return SixnetFileManager.CombinePath(Directory.GetCurrentDirectory(), rootPath);
        }

        /// <summary>
        /// Get save path
        /// Item1: relative path
        /// Item2: save path
        /// </summary>
        /// <param name="fileSetting"></param>
        /// <returns></returns>
        (string, string) GetUploadPath(string fileObjectName, SixnetFileOptions fileOptions, SixnetFileSetting fileSetting)
        {
            // relative path
            var relativePath = string.Empty;

            // root path
            var savePath = GetUploadRootPath(fileOptions, fileSetting);

            // temp folder
            if (fileSetting.UploadToTempFirst && !string.IsNullOrWhiteSpace(fileOptions.UploadTempFolder))
            {
                relativePath = fileOptions.UploadTempFolder;
                savePath = SixnetFileManager.CombinePath(savePath, fileOptions.UploadTempFolder);
            }

            // fileObjectPath
            var fileObjectPath = GetFileObjectSavePath(fileObjectName, fileOptions, fileSetting);
            if (!string.IsNullOrWhiteSpace(fileObjectPath))
            {
                relativePath = SixnetFileManager.CombinePath(relativePath, fileObjectPath);
                savePath = SixnetFileManager.CombinePath(savePath, fileObjectPath);
            }

            // create folder
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            return (relativePath, savePath);
        }

        /// <summary>
        /// Get the store path
        /// </summary>
        /// <param name="fileSetting"></param>
        /// <returns></returns>
        (string, string) GetStorePath(string fileObjectName, SixnetFileOptions fileOptions, SixnetFileSetting fileSetting)
        {
            var storePath = fileSetting.StorePath;
            var storeRelativePath = string.Empty;
            if (!Path.IsPathRooted(storePath))
            {
                storeRelativePath = storePath;
                var uploadRootPath = GetUploadRootPath(fileOptions, fileSetting);
                storePath = SixnetFileManager.CombinePath(uploadRootPath, storePath);
            }

            // file object path
            var fileObjectPath = GetFileObjectSavePath(fileObjectName, fileOptions, fileSetting);
            if (!string.IsNullOrWhiteSpace(fileObjectPath))
            {
                storeRelativePath = SixnetFileManager.CombinePath(storeRelativePath, fileObjectPath);
                storePath = SixnetFileManager.CombinePath(storePath, fileObjectPath);
            }

            return (storeRelativePath, storePath);
        }

        /// <summary>
        /// Get file object save path
        /// </summary>
        /// <param name="fileObjectName"></param>
        /// <param name="fileOptions"></param>
        /// <param name="fileSetting"></param>
        /// <returns></returns>
        string GetFileObjectSavePath(string fileObjectName, SixnetFileOptions fileOptions, SixnetFileSetting fileSetting)
        {
            var fileObjectPath = string.Empty;

            // setting path
            var uploadSettingPath = fileSetting.UploadPath;
            if (string.IsNullOrWhiteSpace(uploadSettingPath)
                && !fileSetting.DisableDefaultFolderGroup
                && !string.IsNullOrWhiteSpace(fileObjectName))
            {
                var fileObjectNames = fileObjectName.LSplit(".");
                fileObjectPath = SixnetFileManager.CombinePath(fileObjectNames);
            }
            else if (!Path.IsPathRooted(uploadSettingPath))
            {
                fileObjectPath = SixnetFileManager.CombinePath(fileObjectPath, uploadSettingPath);
            }

            // date folder
            if (fileSetting.UseDateGroupFolder)
            {
                var dataFolder = DateTimeOffset.Now.ToString("yyyyMMdd");
                fileObjectPath = SixnetFileManager.CombinePath(fileObjectPath, dataFolder);
            }

            // create random folder
            if (!fileSetting.Rename)
            {
                var randomFolder = GuidHelper.GetGuid().ToString().Replace("-", "");
                fileObjectPath = SixnetFileManager.CombinePath(fileObjectPath, randomFolder);
            }
            return fileObjectPath;
        }

        #endregion
    }
}
