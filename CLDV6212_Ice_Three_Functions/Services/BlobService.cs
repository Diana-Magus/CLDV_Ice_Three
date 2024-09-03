using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDV6212_Ice_Three_Functions.Models;
using Azure.Data.Tables;

namespace CLDV6212_Ice_Three_Functions.Services
{
    public class BlobService
    {

        private readonly string connectionString = "defaultEndpointsProtocol=https;AccountName=clvd6212ice3;AccountKey=973k4FLw7sEDEEQX/DMQe4lRp46dJRUwIjyquB1DYPDAJzKDDPJh85ms+epGWvu8RTFQpap1mM44+AStLe8V5Q==;EndpointSuffix=core.windows.net";

        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerTreasure = "treasure";
        private readonly string _containerHint = "hint";

        public BlobService(string connectionString)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
        }


        public async Task<string> UploadsTreasureAsync(Stream fileSteam, string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerTreasure);
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileSteam);
            return blobClient.Uri.ToString();


        }
        public async Task DeleteTreasureBlobAsync(String blobUri)
        {
            Uri uri = new Uri(blobUri);
            string blobName = uri.Segments[^1]; // end of the array element
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerTreasure);
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        }

       
        public async Task<string> UploadsHintAsync(Stream fileSteam, string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerHint);
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileSteam);
            return blobClient.Uri.ToString();


        }
        public async Task DeleteHintBlobAsync(String blobUri)
        {
            Uri uri = new Uri(blobUri);
            string blobName = uri.Segments[^1]; // end of the array element
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerHint);
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        }

    }
}
