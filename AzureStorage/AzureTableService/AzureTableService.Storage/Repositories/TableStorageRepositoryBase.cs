using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;

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
        }

        protected string StorageConnectionString;
        protected ILogger Logger;

        internal const string TableServicesSettingsSectionName = "TableServicesSettings";
        internal const string TableServicesConnectionStringKeyName = "StorageConnectionString";

        protected string GetStorageConnectionString(IConfiguration configuration)
        {
            var section = configuration.GetSection(TableServicesSettingsSectionName);
            if (section == null)
                throw new Exception($"The '{TableServicesSettingsSectionName}' configuration section is missing");

            var connectionString = section[TableServicesConnectionStringKeyName];

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new Exception($"The '{TableServicesConnectionStringKeyName}' configuration value is not valid");

            return connectionString;
        }

        protected CloudStorageAccount CreateStorageAccount()
        {
            if (!CloudStorageAccount.TryParse(this.StorageConnectionString, out var storageAccount))
            {
                throw new Exception($"Error during creation of StorageAccount");
            }

            return storageAccount;
        }
    }
}