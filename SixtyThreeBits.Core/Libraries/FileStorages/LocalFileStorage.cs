using SixtyThreeBits.Core.Abstractions;
using SixtyThreeBits.Core.Libraries.FileStorages.Common;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Libraries.FileStorages
{
    public class LocalFileStorage : IFileStorage
    {
        #region Properties
        readonly string _uploadFolderPhysicalPath;
        readonly string _uploadFolderHttpPath;
        readonly string _noImageHttpPath;
        #endregion

        #region Constructor
        public LocalFileStorage(string uploadFolderPhysicalPath, string uploadFolderHttpPath, string noImageHttpPath, string websiteDomain)
        {
            _uploadFolderPhysicalPath = $"{uploadFolderPhysicalPath.Trim('\\')}\\";
            _uploadFolderHttpPath = $"{websiteDomain.TrimEnd('/')}/{uploadFolderHttpPath.Trim('/')}/";
            _noImageHttpPath = noImageHttpPath;
        }
        #endregion

        #region Methods
        public async Task DeleteFile(string filename, string folderPath = null)
        {
            var destinationFilePhysicalPath = GetDestinationFilePhysicalPath(filename, folderPath);
            if (File.Exists(destinationFilePhysicalPath))
            {
                File.Delete(destinationFilePhysicalPath);
            }
            await Task.FromResult(0);
        }

        public async Task DeleteFolderRecursive(string folderPath)
        {
            var DestinationFolderPhysicalPath = GetDestinationFolderPhysicalPath(folderPath);
            if (Directory.Exists(DestinationFolderPhysicalPath))
            {
                Directory.Delete(DestinationFolderPhysicalPath, recursive: true);
            }
            await Task.FromResult(0);
        }

        public string GetUploadedFileHttpPath(string filename, string folderPath = null)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                return null;
            }
            else
            {
                var SB = new StringBuilder();
                SB.Append(_uploadFolderHttpPath);
                if (!string.IsNullOrEmpty(folderPath))
                {
                    SB.Append(folderPath.Trim('/')).Append('/');
                }
                SB.Append(filename);
                var FileHttpPath = SB.ToString();
                return FileHttpPath;
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
            return GetUploadedFileHttpPath(filename, folderPath);
        }

        public async Task SaveUploadedFile(Stream sourceFileStream, string filename, string folderPath = null)
        {
            if (!string.IsNullOrWhiteSpace(folderPath))
            {
                var destinationFolderPhysicalPath = GetDestinationFolderPhysicalPath(folderPath);
                if (!Directory.Exists(destinationFolderPhysicalPath))
                {
                    Directory.CreateDirectory(destinationFolderPhysicalPath);
                }
            }

            var destinationFilePhysicalPath = GetDestinationFilePhysicalPath(filename, folderPath);

            using (var destinationFileStream = new FileStream(destinationFilePhysicalPath, FileMode.Create))
            {
                sourceFileStream.Seek(0, SeekOrigin.Begin);
                await sourceFileStream.CopyToAsync(destinationFileStream);
            }
        }

        public async Task SaveUploadedFile(byte[] sourceFileBytes, string filename, string folderPath = null)
        {
            if (!string.IsNullOrWhiteSpace(folderPath))
            {
                var destinationFolderPhysicalPath = GetDestinationFolderPhysicalPath(folderPath);
                if (!Directory.Exists(destinationFolderPhysicalPath))
                {
                    Directory.CreateDirectory(destinationFolderPhysicalPath);
                }
            }

            var destinationFilePhysicalPath = GetDestinationFilePhysicalPath(filename, folderPath);
            if (!string.IsNullOrWhiteSpace(destinationFilePhysicalPath))
            {
                await File.WriteAllBytesAsync(destinationFilePhysicalPath, sourceFileBytes);
            }
        }

        public async Task SaveUploadedFile(string sourceFilePhysicalPath, string filename, string folderPath = null)
        {
            if (!string.IsNullOrWhiteSpace(folderPath))
            {
                var destinationFolderPhysicalPath = GetDestinationFolderPhysicalPath(folderPath);
                if (!Directory.Exists(destinationFolderPhysicalPath))
                {
                    Directory.CreateDirectory(destinationFolderPhysicalPath);
                }
            }

            var destinationFilePhysicalPath = GetDestinationFilePhysicalPath(filename, folderPath);
            if (!string.IsNullOrWhiteSpace(destinationFilePhysicalPath))
            {
                using (var SourceStream = File.Open(sourceFilePhysicalPath, FileMode.Open))
                {
                    using (var destinationStream = File.Create(destinationFilePhysicalPath))
                    {
                        await SourceStream.CopyToAsync(destinationStream);
                    }
                }
            }
        }

        public async Task<List<FileStorageItem>> GetFiles(string folderPath = null)
        {
            if (!string.IsNullOrWhiteSpace(folderPath))
            {
                var destinationFolderPhysicalPath = GetDestinationFolderPhysicalPath(folderPath);
                if (!Directory.Exists(destinationFolderPhysicalPath))
                {
                    Directory.CreateDirectory(destinationFolderPhysicalPath);
                }
            }

            var files = new DirectoryInfo($"{_uploadFolderPhysicalPath}{folderPath}").GetFiles().ToList();
            var fileStorageItems = files.Select(item => new FileStorageItem
            (
                Filename: item.Name,
                FilesizeBytes: item.Length,
                FileDateCreated: item.CreationTime,
                FileDateCreatedUtc: item.CreationTimeUtc,
                FileDateUpdated: item.LastWriteTime,
                FileDateUpdatedUtc: item.LastWriteTimeUtc
            )).ToList();
            return await Task.FromResult(fileStorageItems) ?? new List<FileStorageItem>(0);
        }
        #endregion

        #region Private Methods
        string GetDestinationFilePhysicalPath(string filename, string folderPath = null)
        {
            var destinationFilePhysicalPath = default(string);
            if (!string.IsNullOrWhiteSpace(filename))
            {
                var DestinationFolderPhysicalPath = GetDestinationFolderPhysicalPath(folderPath);
                destinationFilePhysicalPath = $"{DestinationFolderPhysicalPath}\\{filename}";
            }
            return destinationFilePhysicalPath;
        }

        string GetDestinationFolderPhysicalPath(string folderPath)
        {
            var sb = new StringBuilder();
            sb.Append(_uploadFolderPhysicalPath);
            if (!string.IsNullOrEmpty(folderPath))
            {
                sb.Append(folderPath.Trim('\\').Trim('/'));
            }
            return sb.ToString();
        }
        #endregion
    }
}