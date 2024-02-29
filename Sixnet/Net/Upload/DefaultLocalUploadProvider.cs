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

            return UploadResult.SuccessResult(parameter.Files.Select(f => SaveFile(f, parameter.Setting)));
        }

        public async Task<UploadResult> UploadAsync(UploadParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter == null, nameof(parameter));
            SixnetDirectThrower.ThrowArgErrorIf(parameter.Files.IsNullOrEmpty(), "Files is null or empty");

            var uploadTasks = parameter.Files.Select(f => SaveFileAsync(f, parameter.Setting));
            return UploadResult.SuccessResult(await Task.WhenAll(uploadTasks).ConfigureAwait(false));
        }

        #endregion

        #region Save file

        /// <summary>
        /// Save file
        /// </summary>
        /// <param name="file">File</param>
        /// <param name="uploadSetting">Upload setting</param>
        /// <returns>upload file result</returns>
        UploadFileResult SaveFile(UploadFile file, UploadSetting uploadSetting)
        {
            var fileResult = HandleFile(file, uploadSetting);
            File.WriteAllBytes(fileResult.FullPath, file.FileContent);
            return fileResult;
        }

        /// <summary>
        /// Save file
        /// </summary>
        /// <param name="file">File</param>
        /// <param name="uploadSetting">Upload setting</param>
        /// <returns>upload file result</returns>
        async Task<UploadFileResult> SaveFileAsync(UploadFile file, UploadSetting uploadSetting)
        {
            var fileResult = HandleFile(file, uploadSetting);
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
        UploadFileResult HandleFile(UploadFile file, UploadSetting uploadSetting)
        {
            SixnetDirectThrower.ThrowArgNullIf(uploadSetting == null, nameof(uploadSetting));
            SixnetDirectThrower.ThrowArgNullIf(file == null, nameof(file));

            #region save path

            string relativePath = string.Empty;
            if (uploadSetting.SaveToContentRoot)
            {
                relativePath = string.IsNullOrWhiteSpace(uploadSetting.ContentRootPath) ? SixnetUploader.DefaultContentFolder : uploadSetting.ContentRootPath;
            }
            string savePath = uploadSetting.SavePath ?? string.Empty;
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
    }
}
