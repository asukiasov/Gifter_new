using SixtyThreeBits.Core.Libraries.FileStorages.Base;
using SixtyThreeBits.Core.Libraries.FileStorages.DTO;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Libraries.FileStorages
{
    public class LocalFileStorage : FileStorageBase
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
        public override async Task DeleteFile(string filename, string folderPath = null)
        {
            var destinationFilePhysicalPath = getDestinationFilePhysicalPath(filename, folderPath);
            if (File.Exists(destinationFilePhysicalPath))
            {
                File.Delete(destinationFilePhysicalPath);
            }
            await Task.CompletedTask;
        }

        public override async Task DeleteFolderRecursive(string folderPath)
        {
            var DestinationFolderPhysicalPath = getDestinationFolderPhysicalPath(folderPath);
            if (Directory.Exists(DestinationFolderPhysicalPath))
            {
                Directory.Delete(DestinationFolderPhysicalPath, recursive: true);
            }
            await Task.CompletedTask;
        }

        public override string GetUploadedFileHttpPath(string filename, string folderPath = null)
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

        public override string GetUploadedFileHttpPathOrDefault(string filename, string folderPath = null, string noImageHttpPath = null)
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

        public override string GetUploadedFileHttpPathSigned(string filename, string folderPath = null)
        {
            return GetUploadedFileHttpPath(filename, folderPath);
        }

        public override async Task SaveUploadedFile(Stream sourceFileStream, string filename, string folderPath = null)
        {
            if (!string.IsNullOrWhiteSpace(folderPath))
            {
                var destinationFolderPhysicalPath = getDestinationFolderPhysicalPath(folderPath);
                if (!Directory.Exists(destinationFolderPhysicalPath))
                {
                    Directory.CreateDirectory(destinationFolderPhysicalPath);
                }
            }

            var destinationFilePhysicalPath = getDestinationFilePhysicalPath(filename, folderPath);

            using (var destinationFileStream = new FileStream(destinationFilePhysicalPath, FileMode.Create))
            {
                sourceFileStream.Seek(0, SeekOrigin.Begin);
                await sourceFileStream.CopyToAsync(destinationFileStream);
            }
        }

        public override async Task SaveUploadedFile(byte[] sourceFileBytes, string filename, string folderPath = null)
        {
            if (!string.IsNullOrWhiteSpace(folderPath))
            {
                var destinationFolderPhysicalPath = getDestinationFolderPhysicalPath(folderPath);
                if (!Directory.Exists(destinationFolderPhysicalPath))
                {
                    Directory.CreateDirectory(destinationFolderPhysicalPath);
                }
            }

            var destinationFilePhysicalPath = getDestinationFilePhysicalPath(filename, folderPath);
            if (!string.IsNullOrWhiteSpace(destinationFilePhysicalPath))
            {
                await File.WriteAllBytesAsync(destinationFilePhysicalPath, sourceFileBytes);
            }
        }

        public override async Task SaveUploadedFile(string sourceFilePhysicalPath, string filename, string folderPath = null)
        {
            if (!string.IsNullOrWhiteSpace(folderPath))
            {
                var destinationFolderPhysicalPath = getDestinationFolderPhysicalPath(folderPath);
                if (!Directory.Exists(destinationFolderPhysicalPath))
                {
                    Directory.CreateDirectory(destinationFolderPhysicalPath);
                }
            }

            var destinationFilePhysicalPath = getDestinationFilePhysicalPath(filename, folderPath);
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

        public override async Task<List<FileStorageItemDTO>> GetFiles(string folderPath = null)
        {
            if (!string.IsNullOrWhiteSpace(folderPath))
            {
                var destinationFolderPhysicalPath = getDestinationFolderPhysicalPath(folderPath);
                if (!Directory.Exists(destinationFolderPhysicalPath))
                {
                    Directory.CreateDirectory(destinationFolderPhysicalPath);
                }
            }

            var files = new DirectoryInfo($"{_uploadFolderPhysicalPath}{folderPath}").GetFiles().ToList();
            var fileStorageItems = files.Select(item => new FileStorageItemDTO
            (
                Filename: item.Name,
                FilesizeBytes: item.Length,
                FileDateCreated: item.CreationTime,
                FileDateCreatedUtc: item.CreationTimeUtc,
                FileDateUpdated: item.LastWriteTime,
                FileDateUpdatedUtc: item.LastWriteTimeUtc
            )).ToList();
            return await Task.FromResult(fileStorageItems) ?? new List<FileStorageItemDTO>(0);
        }
        #endregion

        #region Private Methods
        string getDestinationFilePhysicalPath(string filename, string folderPath = null)
        {
            var destinationFilePhysicalPath = default(string);
            if (!string.IsNullOrWhiteSpace(filename))
            {
                var DestinationFolderPhysicalPath = getDestinationFolderPhysicalPath(folderPath);
                destinationFilePhysicalPath = $"{DestinationFolderPhysicalPath}\\{filename}";
            }
            return destinationFilePhysicalPath;
        }

        string getDestinationFolderPhysicalPath(string folderPath)
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