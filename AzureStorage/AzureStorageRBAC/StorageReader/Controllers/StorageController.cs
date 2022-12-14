using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using StorageReader.Models.Storage;
using System;
using System.IO;
using System.Threading.Tasks;

namespace StorageReader.Controllers
{
    public class StorageController : Controller
    {
        private readonly IConfiguration _configuration;
        public StorageController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public async Task<ActionResult> ReadUsingConnString([FromQuery] string containerName, [FromQuery] string blobPath)
        {
            var model = new BlobModel()
            {
                Feature= "Read blob storage using Connection String",
                ContainerName = containerName,
                BlobPath = blobPath
            };

            try
            {
				#region "Credential"
				var connectionString = this._configuration.GetConnectionString("StorageConnectionString");
				#endregion "Credential"

				var blobServiceClient = new BlobServiceClient(connectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(blobPath);
                var response = await blobClient.DownloadAsync();

                using (var streamReader = new StreamReader(response.Value.Content))
                {
                    model.Content = await streamReader.ReadToEndAsync();
                }
            }
            catch (System.Exception ex)
            {
                model.Exception = ex;
            }

            return View("BlobView",model);
        }

        public async Task<ActionResult> ReadUsingAppRegistration([FromQuery] string containerName, [FromQuery] string blobPath)
        {
            var model = new BlobModel()
            {
                Feature = "Read blob storage using App Registration",
                ContainerName = containerName,
                BlobPath = blobPath
            };

            try
            {
                #region "Credential" 
				var applicationId = this._configuration["ApplicationID"];
                var tenantId = this._configuration["TenantId"];
                var applicationSecret = this._configuration["AppSecret"];
                var storageServiceUri = this._configuration["StorageServiceUri"];
				#endregion "Credential"

				var credential = new ClientSecretCredential(tenantId, applicationId, applicationSecret);
                
                var blobServiceClient = new BlobServiceClient(new Uri(storageServiceUri), credential);
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(blobPath);
                var response = await blobClient.DownloadAsync();

                using (var streamReader = new StreamReader(response.Value.Content))
                {
                    model.Content = await streamReader.ReadToEndAsync();
                }
            }
            catch (System.Exception ex)
            {
                model.Exception = ex;
            }

            return View("BlobView", model);
        }

        public async Task<ActionResult> ReadUsingManagedIdentity([FromQuery] string containerName, [FromQuery] string blobPath)
        {
            var model = new BlobModel()
            {
                Feature = "Read blob storage using Managed Identity",
                ContainerName = containerName,
                BlobPath = blobPath
            };

            try
            {
				#region "Credential"
                var storageServiceUri = this._configuration["StorageServiceUri"];
				var credential = new ManagedIdentityCredential();
				#endregion "Credential"

				var blobServiceClient = new BlobServiceClient(new Uri(storageServiceUri), credential);
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(blobPath);
                var response = await blobClient.DownloadAsync();

                using (var streamReader = new StreamReader(response.Value.Content))
                {
                    model.Content = await streamReader.ReadToEndAsync();
                }
            }
            catch (System.Exception ex)
            {
                model.Exception = ex;
            }

            return View("BlobView", model);
        }
    }
}
