using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Pipelights.Website.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Pipelights.Website.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobStorageClient;


        public BlobService(BlobServiceClient blobStorageClient)
        {
            _blobStorageClient = blobStorageClient;
        }

        public async Task<bool> SaveFileToBlobStorage(Stream stream, string fileName, string container, ILogger logger)
        {
            try
            {
                var blobStorageClient = _blobStorageClient.GetBlobContainerClient(container);

                //You do not need to create sub directory. Just create blob container and use file name like the variable filename

                BlobClient blobClient = blobStorageClient.GetBlobClient(fileName);
                var blob = await blobClient.UploadAsync(stream, true);
                if (blob != null)
                {
                    logger.LogInformation($"Successfully saved file {fileName} to container {container}");

                    return true;
                }

                logger.LogError($"Successfully saved file {fileName} to container {container}");
                return false;
            }
            catch (Exception e)
            {
                logger.LogError($"Exception occurred when saving file to blob storage.File name: {fileName}, container: {container}. " +
                                 $" Exception details: {e.Message}",
                    e.StackTrace);
                return false;
            }
        }

        public async Task<bool> RemoveFile(string url, string container, ILogger logger)
        {
            try
            {
                var fileName = new BlobUriBuilder(new Uri(url))?.BlobName;
                if (!string.IsNullOrEmpty(fileName))
                {
                    var blobStorageClient = _blobStorageClient.GetBlobContainerClient(container);
                    var blobs = blobStorageClient.GetBlobs();
                    foreach (var blobItem in blobs)
                    {
                        if (blobItem.Name.Equals(fileName))
                        {
                            BlobClient blobClient2 = blobStorageClient.GetBlobClient(blobItem.Name);
                            var res = blobClient2.DeleteIfExists();
                        }
                    }
                    //You do not need to create sub directory. Just create blob container and use file name like the variable filename
                    BlobClient blobClient = blobStorageClient.GetBlobClient(fileName);
                    var x = blobStorageClient.GetBlobClient(fileName).DeleteIfExists();
                    var blob = await blobClient.DeleteAsync();
                    if (blob != null)
                    {
                        logger.LogInformation($"Successfully saved file {fileName} to container {container}");

                        return true;
                    }
                }

                logger.LogError($"Successfully saved file {fileName} to container {container}");
                return false;
            }
            catch (Exception e)
            {
                logger.LogError($"Exception occurred when saving file to blob storage.File name: {url}, container: {container}. " +
                                $" Exception details: {e.Message}",
                    e.StackTrace);
                return false;
            }
        }


        public async Task<Stream> OpenFile(string container, string fileName, ILogger logger)
        {
            try
            {
                BlobContainerClient blobStorageClient = _blobStorageClient.GetBlobContainerClient(container);
                BlobClient blobClient = blobStorageClient.GetBlobClient(fileName);

                Stream downloadStream = await blobClient.OpenReadAsync();


                if (downloadStream != null)
                {
                    logger.LogInformation($"Successfully read file {fileName} to container {container}");

                    return downloadStream;
                }

                logger.LogError($"Error opening file {fileName} from container {container}");
                return null;
            }
            catch (Exception e)
            {
                logger.LogError($"Exception occurred when reading file: {fileName}, from container: {container} " +
                                $" Exception details: {e.Message}",
                    e.StackTrace);
                return null;
            }
        }

        public List<string> GetAllResourcesFromFolder(string folder)
        {
            var result = new List<string>();

            // Get the container client object
            BlobContainerClient containerClient = _blobStorageClient.GetBlobContainerClient("lamps");

            // List all blobs in the container
            foreach (var blobItem in containerClient.GetBlobs(BlobTraits.All, BlobStates.None, folder))
            {
                if (blobItem == null || !blobItem.Name.StartsWith(folder + "/"))
                {
                    continue;
                }

                result.Add($"{containerClient.Uri.AbsoluteUri}/{blobItem.Name}");
            }

            return result;
        }

        public async Task<bool> MoveFileToBlobStorage(string sourceContainerName, string sourceFileName, string destinationContainerName,
            string destName, ILogger logger)
        {
            try
            {
                BlobContainerClient containerClient = _blobStorageClient.GetBlobContainerClient(sourceContainerName);

                BlobContainerClient destContainer = _blobStorageClient.GetBlobContainerClient(destinationContainerName);

                BlobClient blobClient = containerClient.GetBlobClient(sourceFileName);
                BlobClient destBlobClient = destContainer.GetBlobClient(destName);
                BlobClient sourceBlobClient = destContainer.GetBlobClient(sourceFileName);
                var result = await destBlobClient.StartCopyFromUriAsync(blobClient.Uri, new BlobCopyFromUriOptions(), CancellationToken.None);
                var deleteResult = await sourceBlobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);

                if (result != null && !string.IsNullOrEmpty(result.Id) &&
                    deleteResult != null)
                {
                    logger.LogInformation($"Successfully moved file {destName} to container {destinationContainerName}, from container {sourceContainerName}");
                    return true;
                }

                logger.LogError(
                        $"Failed to move file {destName} to container {sourceContainerName}, into container {destinationContainerName}");
                return false;
            }
            catch (Exception e)
            {
                logger.LogError($"Exception occurred when moving file to blob storage. File name: {destName}, source container: {destinationContainerName}" +
                                 $" destination container {sourceContainerName}. Exception details: {e.Message}, stackTrace: {e.StackTrace}");
                return false;
            }
        }

        public async Task<MemoryStream> SaveFileFromBlobStorage(string fileName, string container, ILogger logger)
        {
            try
            {
                var memoryStream = new MemoryStream();
                var blobClient = _blobStorageClient.GetBlobContainerClient(container)
                                                   .GetBlobClient(fileName);

                await blobClient.DownloadToAsync(memoryStream);
                logger.LogDebug($"Successfully saved file {fileName} from container {container}");

                return memoryStream;
            }
            catch (Exception e)
            {
                logger.LogError($"Exception occurred when saving file from blob storage.File name: {fileName}, container: {container}. " +
                                $" Exception details: {e.Message}",
                    e.StackTrace);

                return new MemoryStream();
            }
        }
    }
}
