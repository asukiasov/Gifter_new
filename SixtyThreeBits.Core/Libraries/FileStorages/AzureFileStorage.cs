using SixtyThreeBits.Core.Abstractions;
using SixtyThreeBits.Core.Infrastructure.Services;
using SixtyThreeBits.Core.Libraries.FileStorages.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Libraries.FileStorages;

public class AzureFileStorage : IFileStorage
{
    private readonly AzureBlobStorageService _azureBlobStorageService;
    readonly string _noImageHttpPath;

    public AzureFileStorage(string azureBlobStorageConnectionString, string azureBlobStorageContainerName, string noImageHttpPath)
    {
        _azureBlobStorageService = new AzureBlobStorageService(azureBlobStorageConnectionString, azureBlobStorageContainerName);
        _noImageHttpPath = noImageHttpPath;
    }

    public async Task<List<FileStorageItemDTO>> GetFiles(string folderPath = null)
    {
        var blobs = await _azureBlobStorageService.GetFiles(folderPath);
        var result = blobs.Select(item => new FileStorageItemDTO
        (
            Filename: Path.GetFileName(item.Name),
            FilesizeBytes: item.Properties.ContentLength ?? 0,
            FileDateCreated: item.Properties.CreatedOn?.DateTime ?? DateTime.UnixEpoch,
            FileDateCreatedUtc: item.Properties.CreatedOn?.UtcDateTime ?? DateTime.UnixEpoch,
            FileDateUpdated: item.Properties.LastModified?.DateTime ?? DateTime.UnixEpoch,
            FileDateUpdatedUtc: item.Properties.LastModified?.UtcDateTime ?? DateTime.UnixEpoch
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
            var fileHttpPath = _azureBlobStorageService.GetBlobDownloadUrl(filePath);
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
        return _azureBlobStorageService.GetBlobDownloadUrlPrivate(filePath);
    }

    public async Task SaveUploadedFile(Stream sourceFileStream, string filename, string folderPath = null)
    {
        var filePath = string.IsNullOrWhiteSpace(folderPath) ? filename : $"{folderPath}/{filename}";
        await _azureBlobStorageService.UploadFile(sourceFileStream, filePath);
    }

    public async Task SaveUploadedFile(byte[] sourceFileBytes, string filename, string folderPath = null)
    {
        var filePath = string.IsNullOrWhiteSpace(folderPath) ? filename : $"{folderPath}/{filename}";
        await _azureBlobStorageService.UploadFile(sourceFileBytes, filePath);
    }

    public async Task SaveUploadedFile(string sourceFilePhysicalPath, string filename, string folderPath = null)
    {
        var filePath = string.IsNullOrWhiteSpace(folderPath) ? filename : $"{folderPath}/{filename}";
        await _azureBlobStorageService.UploadFile(sourceFilePhysicalPath, filePath);
    }

    public async Task DeleteFile(string filename, string folderPath = null)
    {
        var filePath = string.IsNullOrWhiteSpace(folderPath) ? filename : $"{folderPath}/{filename}";
        await _azureBlobStorageService.DeleteFile(filePath);
    }

    public async Task DeleteFolderRecursive(string folderPath)
    {
        await _azureBlobStorageService.DeleteFolder(folderPath);
    }
}