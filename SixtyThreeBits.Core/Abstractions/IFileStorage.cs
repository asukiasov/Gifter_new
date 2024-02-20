using SixtyThreeBits.Core.Libraries.FileStorages.Core;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Abstractions
{
    public interface IFileStorage
    {
        #region Abstract Methods
        Task SaveUploadedFile(Stream sourceFileStream, string filename, string folderPath = null);
        Task SaveUploadedFile(byte[] sourceFileBytes, string filename, string folderPath = null);
        Task SaveUploadedFile(string sourceFilePhysicalPath, string filename, string folderPath = null);
        Task DeleteFile(string filename, string folderPath = null);
        Task DeleteFolderRecursive(string folderPath);
        string GetUploadedFileHttpPath(string filename, string folderPath = null);
        string GetUploadedFileHttpPathOrDefault(string filename, string folderPath = null, string noImageHttpPath = null);
        string GetUploadedFileHttpPathSigned(string filename, string folderPath = null);
        Task<List<FileStorageItem>> GetFiles(string folderPath = null);
        #endregion
    }    
}