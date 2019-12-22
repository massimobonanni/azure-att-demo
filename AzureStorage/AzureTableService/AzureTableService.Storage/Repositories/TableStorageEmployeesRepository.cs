using AzureTableService.Core.Entities;
using AzureTableService.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzureTableService.Storage.Extensions;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureTableService.Storage.Repositories
{
    public class TableStorageEmployeesRepository : TableStorageRepositoryBase, IEmployeesRepository
    {
        internal const string EmployeesTableName = "Employees";

        public TableStorageEmployeesRepository(ILoggerFactory loggerFactory, IConfiguration configuration)
            :base(loggerFactory,configuration)
        {
            this.Logger = loggerFactory.CreateLogger<TableStorageEmployeesRepository>();
        }
        
        #region [ IEmployeesRepository interface ]
        public Task<Employee> GetByIdAsync(string employeeId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Employee>> QueryAsync(EmployeeSearchFilters filter, CancellationToken cancellationToken)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            var storageAccount = CreateStorageAccount();
            CloudTableClient cloudTableClient = storageAccount.CreateCloudTableClient();
            var tableReference = cloudTableClient.GetTableReference(EmployeesTableName);

            await tableReference.CreateIfNotExistsAsync();

            //var query = new TableQuery<Entities.Employee>()
            //    .Where(
            //        TableQuery.CombineFilters(
            //            TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, EmployeesTableName),
            //            TableOperators.And,
            //            TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, "Ben@contoso.com")
            //        ));

            var query = new TableQuery<Entities.Employee>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, EmployeesTableName));

            var continuationToken = default(TableContinuationToken);
            var results = new List<Employee>();

            do
            {
                var queryResult = await tableReference.ExecuteQuerySegmentedAsync(query,continuationToken);

                results.AddRange(queryResult.Results.Select(e=> e.ToCoreEmployee()));

                continuationToken = queryResult.ContinuationToken;

            } while (continuationToken != null && !cancellationToken.IsCancellationRequested);

            return results;
        }

        #endregion [ IEmployeesRepository interface ]
    }
}
