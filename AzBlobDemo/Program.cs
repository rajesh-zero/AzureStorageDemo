using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AzBlobDemo
{
    class Program
    {
        static string ConnectionString = "ConnectionString";
        public static async Task Main()
        {

            await ProcessAsync();

            Console.WriteLine("Program Finished....");
            Console.ReadLine();
        }

        private static async Task ProcessAsync()
        {
            BlobServiceClient blobClient = new BlobServiceClient(ConnectionString);

            var container = await CreateSampleContainerAsync(blobClient);

            string blobName = UploadSampleBlob(blobClient, container.Name);

            await DownloadSampleBlobAsync(blobClient, container.Name, blobName);

            DeleteSampleBlob(blobClient, container.Name, blobName);

            await DeleteSampleContainerAsync(blobClient, container.Name);
        }
        private static async Task<BlobContainerClient> CreateSampleContainerAsync(BlobServiceClient blobServiceClient)
        {
            // Name the sample container based on new GUID to ensure uniqueness.
            // The container name must be lowercase.
            string containerName = "new-container";

            try
            {
                // Create the container
                BlobContainerClient container = await blobServiceClient.CreateBlobContainerAsync(containerName);

                if (await container.ExistsAsync())
                {
                    Console.WriteLine("Created container {0}", container.Name);
                    return container;
                }
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine("HTTP error code {0}: {1}",
                                    e.Status, e.ErrorCode);
                Console.WriteLine(e.Message);
            }

            return null;
        }

      
        private static string UploadSampleBlob(BlobServiceClient blobServiceClient,string containerName)
        {

            BlobContainerClient container = blobServiceClient.GetBlobContainerClient(containerName);


            string localPath = "./";
            string fileName = "HelloWorld.txt";
            string localFilePath = Path.Combine(localPath, fileName);
            BlobClient blob = container.GetBlobClient(fileName);
            blob.Upload(localFilePath);
            return fileName;
        }
        
        private static async Task DownloadSampleBlobAsync(BlobServiceClient blobServiceClient, string containerName, string blobName)
        {
            string downloadFilePath = "./downloaded.txt";

            Console.WriteLine("\nDownloading blob to\n\t{0}\n", downloadFilePath);
            BlobContainerClient container = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = container.GetBlobClient(blobName);

            // Download the blob's contents and save it to a file
            BlobDownloadInfo download = await blobClient.DownloadAsync();

            using (FileStream downloadFileStream = File.OpenWrite(downloadFilePath))
            {
                await download.Content.CopyToAsync(downloadFileStream);
                downloadFileStream.Close();
            }
        }

        private static void DeleteSampleBlob(BlobServiceClient blobServiceClient, string containerName, string blobName)
        {

            BlobContainerClient container = blobServiceClient.GetBlobContainerClient(containerName);

            BlobClient blob = container.GetBlobClient(blobName);
            blob.DeleteIfExists();
        }

        private static async Task DeleteSampleContainerAsync(BlobServiceClient blobServiceClient, string containerName)
        {
            BlobContainerClient container = blobServiceClient.GetBlobContainerClient(containerName);

            try
            {
                // Delete the specified container and handle the exception.
                await container.DeleteAsync();
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine("HTTP error code {0}: {1}",
                                    e.Status, e.ErrorCode);
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

    }
}
