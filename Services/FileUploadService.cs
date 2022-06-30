using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using CameraAPI.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace CameraAPI.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IConfiguration _configuration;

        public FileUploadService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> UploadFileToStorage(IFormFile file)
        {
            var azureConnectionString = _configuration.GetConnectionString("AzureBlob");
            var container = new BlobContainerClient(azureConnectionString, "receipt-images");
            var createResponse = await container.CreateIfNotExistsAsync();
            if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                await container.SetAccessPolicyAsync(PublicAccessType.Blob);
            var blob = container.GetBlobClient(file.FileName);
            await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
            using (var fileStream = file.OpenReadStream())
            {
                await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = file.ContentType });
            }
            return blob.GenerateSasUri(BlobSasPermissions.Read, DateTime.Now.AddHours(24)).ToString();
        }
    }
}
