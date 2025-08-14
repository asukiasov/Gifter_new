using SixtyThreeBits.Core.Abstractions;
using SixtyThreeBits.Core.Libraries.FileStorages.DTO;
using SixtyThreeBits.Core.Libraries.FileStorages.Enums;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Libraries.FileStorages.Base
{
    public abstract class FileStorageBase : IFileStorage
    {
        #region Abstract Methods
        public abstract Task SaveUploadedFile(Stream sourceFileStream, string filename, string folderPath = null);
        public abstract Task SaveUploadedFile(byte[] sourceFileBytes, string filename, string folderPath = null);
        public abstract Task SaveUploadedFile(string sourceFilePhysicalPath, string filename, string folderPath = null);
        public abstract Task DeleteFile(string filename, string folderPath = null);
        public abstract Task DeleteFolderRecursive(string folderPath);
        public abstract string GetUploadedFileHttpPath(string filename, string folderPath = null);
        public abstract string GetUploadedFileHttpPathOrDefault(string filename, string folderPath = null, string noImageHttpPath = null);
        public abstract string GetUploadedFileHttpPathSigned(string filename, string folderPath = null);
        public abstract Task<List<FileStorageItemDTO>> GetFiles(string folderPath = null);
        #endregion

        #region Methods
        public string GetFolderPathByModule(FileManagerModules fileManagerModules)
        {
            return fileManagerModules switch
            {
                FileManagerModules.Blog => "blog",
                FileManagerModules.News => "news",
                FileManagerModules.Pages => "pages",
                FileManagerModules.Products => "products",
                _ => null
            };
        }

        public string GetThumbnailFolderPathByModule(FileManagerModules fileManagerModules)
        {
            return fileManagerModules switch
            {
                FileManagerModules.Blog => "blog/thumbnails",
                FileManagerModules.News => "news/thumbnails",
                FileManagerModules.Pages => "pages/thumbnails",
                FileManagerModules.Products => "products/thumbnails",
                _ => null
            };
        }
        #endregion
    }
}
