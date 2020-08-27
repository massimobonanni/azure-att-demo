using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureTableService.Storage.Repositories
{
    public abstract class TableStorageRepositoryBase
    {
        protected TableStorageRepositoryBase(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            this.StorageConnectionString = GetStorageConnectionString(configuration);
            CreateStorageAccount();
        }

        protected readonly string StorageConnectionString;
        protected ILogger Logger;
        protected CloudStorageAccount storageAccount;

        internal const string TableServicesSettingsSectionName = "TableServicesSettings";
        internal const string TableServicesConnectionStringKeyName = "StorageConnectionString";

        protected string GetStorageConnectionString(IConfiguration configuration)
        {
            var fullAppSettingsKey = $"{TableServicesSettingsSectionName}:{TableServicesConnectionStringKeyName}";

            var connectionString = configuration[fullAppSettingsKey];

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                connectionString = configuration.GetConnectionString(TableServicesConnectionStringKeyName);
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception($"The '{fullAppSettingsKey}' configuration value and {TableServicesConnectionStringKeyName} ConnectionString value are not valid or not configured.");
            }

            return connectionString;
        }

        protected void CreateStorageAccount()
        {
            if (!CloudStorageAccount.TryParse(this.StorageConnectionString, out var storageAccount))
            {
                throw new Exception($"Error during creation of StorageAccount");
            }

            this.storageAccount = storageAccount;
        }

        protected async Task<CloudTable> CreateTableReference(string tableName)
        {
            CloudTableClient cloudTableClient = storageAccount.CreateCloudTableClient();
            var tableReference = cloudTableClient.GetTableReference(tableName);

            await tableReference.CreateIfNotExistsAsync();
            return tableReference;
        }
    }
}