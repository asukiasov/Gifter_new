using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace SixtyThreeBits.Core.Infrastructure.Services
{
    public class AzureBlobStorageService
    {
        #region Properties
        private readonly BlobContainerClient _containerClient;
        #endregion

        #region Constructors
        public AzureBlobStorageService(string connectionString, string blobContainerName)
        {
            if (!string.IsNullOrWhiteSpace(connectionString) && !string.IsNullOrWhiteSpace(blobContainerName))
            {
                _containerClient = new BlobContainerClient(connectionString, blobContainerName);
            }            
        }
        #endregion

        #region Methods
        public async Task<List<BlobItem>> GetFiles(string folderPath)
        {
            var blobsList = new List<BlobItem>();

            var blobItems = _containerClient.GetBlobsAsync(traits: BlobTraits.None, states: BlobStates.None, prefix: folderPath, cancellationToken: default);

            await foreach (var blobItem in blobItems)
            {
                var IsCurrentFolderFile = blobItem.Name.Replace($"{folderPath}/", "").Count(item => item == '/') == 0;
                if (IsCurrentFolderFile)
                {
                    blobsList.Add(blobItem);
                }
            }

            return blobsList;
        }

        public string GetBlobDownloadUrl(string filePath)
        {
            return $"https://{_containerClient.AccountName}.blob.core.windows.net/{_containerClient.Name}/{filePath}";
        }

        public string GetBlobDownloadUrlPrivate(string filePath, int expireInSeconds = 3600)
        {
            var blobClient = _containerClient.GetBlobClient(filePath);
            var blobUri = blobClient.GenerateSasUri(new BlobSasBuilder(BlobSasPermissions.Read, expiresOn: DateTimeOffset.Now.AddSeconds(expireInSeconds)));
            return blobUri.AbsoluteUri;
        }

        public async Task UploadFile(string filePhysicalPath, string filePath)
        {
            var blobClient = _containerClient.GetBlobClient(filePath);
            await blobClient.UploadAsync(path: filePhysicalPath, overwrite: true);
        }

        public async Task UploadFile(Stream inputStream, string filePath)
        {
            var blobClient = _containerClient.GetBlobClient(filePath);
            await blobClient.UploadAsync(inputStream, overwrite: true);
        }

        public async Task UploadFile(byte[] fileBytes, string filePath)
        {
            var blobClient = _containerClient.GetBlobClient(filePath);
            var binaryData = BinaryData.FromBytes(fileBytes);
            await blobClient.UploadAsync(binaryData, overwrite: true);
        }

        public async Task DeleteFile(string filePath)
        {
            var blobClient = _containerClient.GetBlobClient(filePath);
            await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        }

        public async Task DeleteFolder(string folderPath)
        {
            var folderBlobs = _containerClient.GetBlobsAsync(traits: BlobTraits.None, states: BlobStates.None, prefix: folderPath, cancellationToken: default);
            await foreach (var blob in folderBlobs)
            {
                var blobClient = _containerClient.GetBlobClient(blob.Name);
                await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
            }
        }
        #endregion
    }
}