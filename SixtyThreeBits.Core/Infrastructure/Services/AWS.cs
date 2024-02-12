using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Services
{
    public class AwsService 
    {
        #region Properties
        readonly string _awsAccessKeyID;
        readonly string _awsSecretAccessKey;
        readonly string _awsS3BucketName;
        readonly Amazon.RegionEndpoint _awsS3Region;
        #endregion

        #region Constructor
        public AwsService(string awsAccessKeyID, string awsSecretAccessKey, string awsS3RegionSystemName, string awsS3BucketName)
        {
            _awsAccessKeyID = awsAccessKeyID;
            _awsSecretAccessKey = awsSecretAccessKey;
            _awsS3BucketName = awsS3BucketName;
            _awsS3Region = Amazon.RegionEndpoint.GetBySystemName(awsS3RegionSystemName);
        }
        #endregion

        #region Methods
        public async Task CopyFolderAsyncTask(string sourceFolderPath, string destinationFolderPath, string sourceAwsS3BucketName, string destinationAwsS3BucketName)
        {
            if (!string.IsNullOrWhiteSpace(sourceFolderPath) && !string.IsNullOrWhiteSpace(destinationFolderPath))
            {
                using (var s3Client = new AmazonS3Client(_awsAccessKeyID, _awsSecretAccessKey, _awsS3Region))
                {
                    sourceFolderPath = $"{sourceFolderPath.Trim('/')}/";
                    destinationFolderPath = $"{destinationFolderPath.Trim('/')}/";

                    var request = new ListObjectsV2Request()
                    {
                        BucketName = sourceAwsS3BucketName,
                        Prefix = sourceFolderPath,
                        Delimiter = "/"
                    };

                    var s3ObjectsList = await s3Client.ListObjectsV2Async(request);

                    foreach (var s3Object in s3ObjectsList.S3Objects)
                    {
                        var CopyObjectRequest = new CopyObjectRequest
                        {
                            SourceBucket = sourceAwsS3BucketName,
                            SourceKey = s3Object.Key,
                            DestinationBucket = destinationAwsS3BucketName,
                            DestinationKey = $"{destinationFolderPath}{s3Object.Key.Substring(sourceFolderPath.Length)}"
                        };
                        await s3Client.CopyObjectAsync(CopyObjectRequest);
                    }

                    foreach (var folder in s3ObjectsList.CommonPrefixes)
                    {
                        var actualFolder = folder.Substring(sourceFolderPath.Length);
                        actualFolder = actualFolder.Substring(0, actualFolder.Length - 1);
                        await CopyFolderAsyncTask(folder, $"{destinationFolderPath}{folder.Substring(sourceFolderPath.Length)}", sourceAwsS3BucketName, destinationAwsS3BucketName);
                    }

                }
            }
        }

        public async Task CopyFileAsyncTask(string sourceFilePath, string destinationFilePath, string sourceAwsS3BucketName, string destinationAwsS3BucketName)
        {
            if (!string.IsNullOrWhiteSpace(sourceFilePath) && !string.IsNullOrWhiteSpace(destinationFilePath))
            {
                using (var s3Client = new AmazonS3Client(_awsAccessKeyID, _awsSecretAccessKey, _awsS3Region))
                {
                    sourceFilePath = $"{sourceFilePath.Trim('/')}";
                    destinationFilePath = $"{destinationFilePath.Trim('/')}";

                    var request = new GetObjectRequest
                    {
                        BucketName = sourceAwsS3BucketName,
                        Key = sourceFilePath
                    };

                    var file = await s3Client.GetObjectAsync(request);
                    if (file != null)
                    {
                        var CopyObjectRequest = new CopyObjectRequest
                        {
                            SourceBucket = sourceAwsS3BucketName,
                            SourceKey = file.Key,
                            DestinationBucket = destinationAwsS3BucketName,
                            DestinationKey = destinationFilePath
                        };
                        await s3Client.CopyObjectAsync(CopyObjectRequest);
                    }
                }
            }
        }

        public async Task DeleteFolderAsyncTask(string folderPath)
        {
            if (!string.IsNullOrWhiteSpace(folderPath))
            {
                using (var s3Client = new AmazonS3Client(_awsAccessKeyID, _awsSecretAccessKey, _awsS3Region))
                {
                    folderPath = $"{folderPath.Trim('/')}/";
                    var s3Objects = s3Client.ListObjectsAsync(_awsS3BucketName, folderPath).Result;
                    var s3ObjectsVersions = s3Objects.S3Objects.Select(item => new KeyVersion
                    {
                        Key = item.Key
                    }).ToList();

                    s3ObjectsVersions.Add(new KeyVersion
                    {
                        Key = folderPath
                    });

                    var MultiObjectDeleteRequest = new DeleteObjectsRequest
                    {
                        BucketName = _awsS3BucketName,
                        Objects = s3ObjectsVersions
                    };
                    var result = await s3Client.DeleteObjectsAsync(MultiObjectDeleteRequest);
                }
            }
        }

        public async Task DeleteFileAsyncTask(string filename)
        {
            if (!string.IsNullOrWhiteSpace(filename))
            {
                using (var s3Client = new AmazonS3Client(_awsAccessKeyID, _awsSecretAccessKey, _awsS3Region))
                {
                    var deleteObjectRequest = new DeleteObjectRequest
                    {
                        BucketName = _awsS3BucketName,
                        Key = filename
                    };

                    await s3Client.DeleteObjectAsync(deleteObjectRequest);
                }
            }
        }

        public async Task DownloadFileFromS3AsyncTask(string fileToDownload, string filePathToSave)
        {
            var s3Config = new AmazonS3Config
            {
                Timeout = TimeSpan.FromHours(1),
                RegionEndpoint = _awsS3Region
            };
            var FileTransferUtility = new TransferUtility(new AmazonS3Client(_awsAccessKeyID, _awsSecretAccessKey, s3Config));
            await FileTransferUtility.DownloadAsync(filePathToSave, _awsS3BucketName, fileToDownload);
        }

        public string GetFileDownloadUrlSigned(string filePath, int expireInSeconds = 3600)
        {
            var downloadUrl = default(string);
            using (var s3Client = new AmazonS3Client(_awsAccessKeyID, _awsSecretAccessKey, _awsS3Region))
            {
                var GetPreSignedUrlRequest = new GetPreSignedUrlRequest
                {
                    BucketName = _awsS3BucketName,
                    Key = filePath,
                    Expires = DateTime.Now.AddSeconds(expireInSeconds)
                };
                downloadUrl = s3Client.GetPreSignedURL(GetPreSignedUrlRequest);
            }

            return downloadUrl;
        }

        public string GetFileDownloadUrlPublic(string filePath)
        {
            return $"https://{_awsS3BucketName}.s3.{_awsS3Region.PartitionDnsSuffix}/{filePath}";
        }

        public async Task<byte[]> GetFileBytesFromS3AsyncTask(string filepath)
        {
            using (var s3Client = new AmazonS3Client(_awsAccessKeyID, _awsSecretAccessKey, _awsS3Region))
            {
                var request = new GetObjectRequest
                {
                    BucketName = _awsS3BucketName,
                    Key = filepath
                };

                using (var response = await s3Client.GetObjectAsync(request))
                {
                    int totalBytesToRead = (int)response.ContentLength;
                    int bytesRead = 0;
                    byte[] byteArray = new byte[totalBytesToRead];
                    while (totalBytesToRead > 0)
                    {
                        int index = response.ResponseStream.Read(byteArray, bytesRead, totalBytesToRead);
                        if (index == 0)
                        {
                            break;
                        }
                        bytesRead += index;
                        totalBytesToRead -= index;
                    }
                    return byteArray;
                }
            }
        }

        public async Task<GetFileInfoResult> GetFileInfoAsyncTask(string filePath)
        {
            var result = new GetFileInfoResult();
            using (var s3Client = new AmazonS3Client(_awsAccessKeyID, _awsSecretAccessKey, _awsS3Region))
            {
                var request = new GetObjectRequest
                {
                    BucketName = _awsS3BucketName,
                    Key = filePath
                };

                using (var response = await s3Client.GetObjectAsync(request))
                {
                    result.FilePath = response.Key;
                    result.Filename = Path.GetFileName(result.FilePath);
                    result.FileSizeBytes = response.ContentLength;
                }
            }
            return result;
        }

        public async Task<List<GetFilesResultItem>> GetFilesAsyncTask()
        {
            var result = new List<GetFilesResultItem>();
            using (var s3Client = new AmazonS3Client(_awsAccessKeyID, _awsSecretAccessKey, _awsS3Region))
            {
                var request = new ListObjectsV2Request
                {
                    BucketName = _awsS3BucketName
                };

                var paginatorResponse = s3Client.Paginators.ListObjectsV2(request);


                await foreach (var response in paginatorResponse.Responses)
                {
                    foreach (var File in response.S3Objects)
                    {
                        var IsCurrentFolderFile = File.Key.Count(item => item == '/') == 0;
                        if (IsCurrentFolderFile)
                        {
                            result.Add(new GetFilesResultItem
                            {
                                Name = File.Key.Split('/').LastOrDefault(),
                                Key = File.Key,
                                Size = File.Size,
                                DateModified = File.LastModified,
                            });
                        }
                    }
                }
            }
            return result;
        }

        public async Task<List<GetFilesResultItem>> GetFilesAsyncTask(string folderPath)
        {
            var result = new List<GetFilesResultItem>();
            using (var s3Client = new AmazonS3Client(_awsAccessKeyID, _awsSecretAccessKey, _awsS3Region))
            {
                var request = new ListObjectsV2Request
                {
                    BucketName = _awsS3BucketName,
                    Prefix = $"{folderPath}/"
                };

                var paginatorResponse = s3Client.Paginators.ListObjectsV2(request);

                await foreach (var response in paginatorResponse.Responses)
                {
                    foreach (var file in response.S3Objects)
                    {
                        var IsCurrentFolderFile = file.Key.Replace($"{folderPath}/", "").Count(item => item == '/') == 0;
                        if (IsCurrentFolderFile)
                        {
                            result.Add(new GetFilesResultItem
                            {
                                Name = file.Key.Split('/').LastOrDefault(),
                                Key = file.Key,
                                Size = file.Size,
                                DateModified = file.LastModified,
                            });
                        }
                    }
                }
            }
            return result;
        }

        public async Task<bool> Ping()
        {
            using (var s3Client = new AmazonS3Client(_awsAccessKeyID, _awsSecretAccessKey, _awsS3Region))
            {
                var result = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(s3Client, _awsS3BucketName);
                return result;
            }
        }

        public async Task UploadFileToS3AsyncTask(Stream inputStream, string filePath)
        {
            using (var s3Client = new AmazonS3Client(_awsAccessKeyID, _awsSecretAccessKey, _awsS3Region))
            {
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = inputStream,
                    Key = filePath,
                    BucketName = _awsS3BucketName
                };

                var fileTransferUtility = new TransferUtility(s3Client);
                await fileTransferUtility.UploadAsync(uploadRequest);
            }
        }

        public async Task UploadFileToS3AsyncTask(byte[] inputBytes, string filePath)
        {
            using (var s3Client = new AmazonS3Client(_awsAccessKeyID, _awsSecretAccessKey, _awsS3Region))
            {
                using (var inputStream = new MemoryStream(inputBytes))
                {
                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = inputStream,
                        Key = filePath,
                        BucketName = _awsS3BucketName
                    };

                    var fileTransferUtility = new TransferUtility(s3Client);
                    await fileTransferUtility.UploadAsync(uploadRequest);
                }
            }
        }

        public async Task UploadFileToS3AsyncTask(string filePhysicalPath, string filePathAws)
        {
            var s3Config = new AmazonS3Config
            {
                Timeout = TimeSpan.FromHours(1),
                RegionEndpoint = _awsS3Region
            };
            using (var fileTransferUtility = new TransferUtility(new AmazonS3Client(_awsAccessKeyID, _awsSecretAccessKey, s3Config)))
            {
                await fileTransferUtility.UploadAsync(filePhysicalPath, _awsS3BucketName, filePathAws);
            }
        }
        #endregion

        #region Nested Classes
        public class GetFileInfoResult
        {
            #region Properties
            public string Filename { get; set; }
            public string FilePath { get; set; }
            public long FileSizeBytes { get; set; }
            #endregion
        }

        public class GetFilesResultItem
        {
            #region Properties
            public string Name { get; set; }
            public string Key { get; set; }
            public long Size { get; set; }
            public DateTime DateModified { get; set; }
            #endregion
        }
        #endregion
    }
}
