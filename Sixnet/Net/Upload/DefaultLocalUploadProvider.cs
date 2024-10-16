using Sixnet.Code;
using Sixnet.DependencyInjection;
using Sixnet.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sixnet.Net.Upload
{
    internal class DefaultLocalUploadProvider : ISixnetUploadProvider
    {
        #region Upload
        public UploadResult Upload(UploadParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter == null, nameof(parameter));
            SixnetDirectThrower.ThrowArgErrorIf(parameter.Files.IsNullOrEmpty(), "Files is null or empty");
            var uploadOptions = SixnetContainer.GetOptions<UploadOptions>();
            return UploadResult.SuccessResult(parameter.Files.Select(f => SaveFile(f, uploadOptions, parameter.Setting)));
        }

        public async Task<UploadResult> UploadAsync(UploadParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter == null, nameof(parameter));
            SixnetDirectThrower.ThrowArgErrorIf(parameter.Files.IsNullOrEmpty(), "Files is null or empty");

            var uploadOptions = SixnetContainer.GetOptions<UploadOptions>();
            var uploadTasks = parameter.Files.Select(f => SaveFileAsync(f, uploadOptions, parameter.Setting));
            return UploadResult.SuccessResult(await Task.WhenAll(uploadTasks).ConfigureAwait(false));
        }

        #endregion

        #region Move

        /// <summary>
        /// Move file
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns></returns>
        public List<string> Move(MoveUploadFileParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter?.RelativeFilePaths.IsNullOrEmpty() ?? true, nameof(MoveUploadFileParameter.RelativeFilePaths));
            var uploadSetting = SixnetUploader.GetUploadSetting(parameter.ObjectName);

            // not move
            var notMove = parameter.IgnoreNotTempFirst && !uploadSetting.TempFirst;
            if (!notMove)
            {
                var uploadOptions = SixnetContainer.GetOptions<UploadOptions>();
                var uploadSavePath = GetUploadSavePath(uploadOptions, uploadSetting);
                var targetPath = GetTargetPath(uploadSetting);
                var targetFiles = new List<string>();
                foreach (var file in parameter.RelativeFilePaths)
                {
                    var filePath = file;
                    if (!string.IsNullOrWhiteSpace(uploadOptions.TempFolder))
                    {
                        filePath = filePath.LSplit(uploadOptions.TempFolder)[^1];
                    }
                    var orginalFile = Path.Combine(uploadSavePath, filePath);
                    var targetFile = Path.Combine(targetPath, filePath);
                    if (File.Exists(targetFile))
                    {
                        continue;
                    }
                    File.Move(orginalFile, targetFile);
                }
            }
            return parameter.RelativeFilePaths.Select(c => c).ToList();
        }

        /// <summary>
        /// Move file
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns></returns>
        public Task<List<string>> MoveAsync(MoveUploadFileParameter parameter)
        {
            return Task.FromResult(Move(parameter));
        }


        #endregion

        #region Save file

        /// <summary>
        /// Save file
        /// </summary>
        /// <param name="file">File</param>
        /// <param name="uploadSetting">Upload setting</param>
        /// <returns>upload file result</returns>
        UploadFileResult SaveFile(UploadFile file, UploadOptions uploadOptions, UploadSetting uploadSetting)
        {
            var fileResult = HandleFile(file, uploadOptions, uploadSetting);
            File.WriteAllBytes(fileResult.FullPath, file.FileContent);
            return fileResult;
        }

        /// <summary>
        /// Save file
        /// </summary>
        /// <param name="file">File</param>
        /// <param name="uploadSetting">Upload setting</param>
        /// <returns>upload file result</returns>
        async Task<UploadFileResult> SaveFileAsync(UploadFile file, UploadOptions uploadOptions, UploadSetting uploadSetting)
        {
            var fileResult = HandleFile(file, uploadOptions, uploadSetting);
            await File.WriteAllBytesAsync(fileResult.FullPath, file.FileContent).ConfigureAwait(false);
            return fileResult;
        }

        #endregion

        #region Handle file

        /// <summary>
        /// Handle file
        /// </summary>
        /// <param name="file">File</param>
        /// <param name="uploadSetting">Upload setting</param>
        /// <returns></returns>
        UploadFileResult HandleFile(UploadFile file, UploadOptions uploadOptions, UploadSetting uploadSetting)
        {
            SixnetDirectThrower.ThrowArgNullIf(uploadSetting == null, nameof(uploadSetting));
            SixnetDirectThrower.ThrowArgNullIf(file == null, nameof(file));

            #region save path

            (string relativePath, string savePath) = GetSavePath(uploadOptions, uploadSetting, file.Folder);

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
            if (uploadSetting.Rename)
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

        /// <summary>
        /// Get save path
        /// Item1: relative path
        /// Item2: save path
        /// </summary>
        /// <param name="uploadSetting"></param>
        /// <returns></returns>
        (string, string) GetSavePath(UploadOptions uploadOptions, UploadSetting uploadSetting, string folder)
        {
            // relative path
            var relativePath = string.Empty;

            // root path
            var savePath = GetRootPath(uploadSetting);

            // temp folder
            if (uploadSetting.TempFirst && !string.IsNullOrWhiteSpace(uploadOptions.TempFolder))
            {
                relativePath = uploadOptions.TempFolder;
                savePath = Path.Combine(savePath, uploadOptions.TempFolder);
            }

            // customer folder
            if (!string.IsNullOrWhiteSpace(folder))
            {
                relativePath = Path.Combine(relativePath, folder);
                savePath = Path.Combine(savePath, folder);
            }

            // date folder
            if (!uploadSetting.DateClassification)
            {
                var dataFolder = DateTimeOffset.Now.ToString("yyyyMMdd");
                relativePath = Path.Combine(relativePath, dataFolder);
                savePath = Path.Combine(savePath, dataFolder);
            }

            // create random folder
            if (!uploadSetting.Rename)
            {
                var randomFolder = GuidHelper.GetGuid().ToString().Replace("-", "");
                relativePath = Path.Combine(relativePath, randomFolder);
                savePath = Path.Combine(savePath, randomFolder);
            }

            // create folder
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            return (relativePath, savePath);
        }

        /// <summary>
        /// Get root path
        /// </summary>
        /// <param name="uploadSetting"></param>
        /// <returns></returns>
        string GetRootPath(UploadSetting uploadSetting)
        {
            return Path.IsPathRooted(uploadSetting.SavePath)
                   ? uploadSetting.SavePath
                   : Path.Combine(Directory.GetCurrentDirectory(), uploadSetting.SavePath);
        }

        /// <summary>
        /// Get upload save path
        /// </summary>
        /// <param name="uploadOptions"></param>
        /// <param name=""></param>
        /// <returns></returns>
        string GetUploadSavePath(UploadOptions uploadOptions, UploadSetting uploadSetting)
        {
            var savePath = GetRootPath(uploadSetting);

            // temp folder
            if (uploadSetting.TempFirst && !string.IsNullOrWhiteSpace(uploadOptions.TempFolder))
            {
                savePath = Path.Combine(savePath, uploadOptions.TempFolder);
            }
            return savePath;
        }

        /// <summary>
        /// Get the target path
        /// </summary>
        /// <param name="uploadSetting"></param>
        /// <returns></returns>
        string GetTargetPath(UploadSetting uploadSetting)
        {
            var targetPath = uploadSetting.TargetPath;
            if (!Path.IsPathRooted(targetPath))
            {
                targetPath = Path.Combine(GetRootPath(uploadSetting), targetPath);
            }
            return targetPath;
        }

        #endregion
    }
}
