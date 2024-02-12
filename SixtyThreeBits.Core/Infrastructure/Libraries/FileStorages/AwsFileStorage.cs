using SixtyThreeBits.Core.Abstractions;
using SixtyThreeBits.Core.Infrastructure.Libraries.FileStorages.Core;
using SixtyThreeBits.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Libraries.FileStorages
{
    public class AwsFileStorage : IFileStorage
    {
        #region Properties
        readonly string _noImageHttpPath;
        readonly AwsService _awsService;
        #endregion

        #region Constructors
        public AwsFileStorage(string awsAccessKeyID, string awsSecretAccessKey, string awsS3RegionSystemName, string awsS3BucketNamePublic, string noImageHttpPath)
        {
            _awsService = new AwsService(awsAccessKeyID, awsSecretAccessKey, awsS3RegionSystemName, awsS3BucketNamePublic);
            _noImageHttpPath = noImageHttpPath;
        }
        #endregion

        #region Methods
        public async Task DeleteFile(string filename, string folderPath = null)
        {
            var filePath = string.IsNullOrWhiteSpace(folderPath) ? filename : $"{folderPath.TrimEnd('/')}/{filename}";
            await _awsService.DeleteFileAsyncTask(filePath);
        }

        public async Task DeleteFolderRecursive(string folderPath)
        {
            await _awsService.DeleteFolderAsyncTask(folderPath);
        }

        public async Task<List<FileStorageItem>> GetFiles(string folderPath = null)
        {
            var files = string.IsNullOrWhiteSpace(folderPath) ? await _awsService.GetFilesAsyncTask() : await _awsService.GetFilesAsyncTask(folderPath);
            var result = files.Select(item => new FileStorageItem(
                Filename: item.Name,
                FilesizeBytes: item.Size,
                FileDateCreated: item.DateModified,
                FileDateCreatedUtc: item.DateModified.ToUniversalTime(),
                FileDateUpdated: item.DateModified,
                FileDateUpdatedUtc: item.DateModified.ToUniversalTime()
            )).ToList();
            return result;
        }

        public string GetUploadedFileHttpPath(string filename, string folderPath = null)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                return null;
            }
            else
            {
                var filePath = string.IsNullOrWhiteSpace(folderPath) ? filename : $"{folderPath.TrimEnd('/')}/{filename}";
                var fileHttpPath = _awsService.GetFileDownloadUrlPublic(filePath);
                return fileHttpPath;
            }
        }

        public string GetUploadedFileHttpPathOrDefault(string filename, string folderPath = null, string noImageHttpPath = null)
        {
            var fileHttpPath = GetUploadedFileHttpPath(filename, folderPath);
            if (string.IsNullOrWhiteSpace(fileHttpPath))
            {
                return string.IsNullOrWhiteSpace(noImageHttpPath) ? _noImageHttpPath : noImageHttpPath;
            }
            else
            {
                return fileHttpPath;
            }
        }

        public string GetUploadedFileHttpPathSigned(string filename, string folderPath = null)
        {
            var filePath = string.IsNullOrWhiteSpace(folderPath) ? filename : $"{folderPath}/{filename}";
            var fileDownloadUrlSigned = _awsService.GetFileDownloadUrlSigned(filePath);
            return fileDownloadUrlSigned;
        }

        public async Task SaveUploadedFile(Stream sourceFileStream, string filename, string folderPath = null)
        {
            var filePath = string.IsNullOrWhiteSpace(folderPath) ? filename : $"{folderPath}/{filename}";
            await _awsService.UploadFileToS3AsyncTask(sourceFileStream, filePath);
        }

        public async Task SaveUploadedFile(byte[] sourceFileBytes, string filename, string folderPath = null)
        {
            var filePath = string.IsNullOrWhiteSpace(folderPath) ? filename : $"{folderPath}/{filename}";
            await _awsService.UploadFileToS3AsyncTask(sourceFileBytes, filePath);
        }

        public async Task SaveUploadedFile(string sourceFilePhysicalPath, string filename, string folderPath = null)
        {
            var filePath = string.IsNullOrWhiteSpace(folderPath) ? filename : $"{folderPath}/{filename}";
            await _awsService.UploadFileToS3AsyncTask(sourceFilePhysicalPath, filePath);
        }
        #endregion
    }
}
