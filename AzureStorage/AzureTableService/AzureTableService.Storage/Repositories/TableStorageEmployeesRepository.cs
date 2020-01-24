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
using AzureTableService.Core.Logger;
using AzureTableService.Storage.Extensions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using AzureTableService.Storage.Helpers;

namespace AzureTableService.Storage.Repositories
{
    public class TableStorageEmployeesRepository : TableStorageRepositoryBase, IEmployeesRepository
    {
        internal const string EmployeesTableName = "Employees";

        public TableStorageEmployeesRepository(ILoggerFactory loggerFactory, IConfiguration configuration)
            : base(loggerFactory, configuration)
        {
            this.Logger = loggerFactory.CreateLogger<TableStorageEmployeesRepository>();
        }

        #region [ IEmployeesRepository interface ]
        public async Task<Employee> GetByIdAsync(string employeeId, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await this.GetByIdInternalAsync(employeeId, cancellationToken);

                return entity?.ToCoreEmployee();
            }
            catch (StorageException e)
            {
                this.Logger.LogError(e, "Error during GetByIdAsync operation");
                throw;
            }
        }

        public async Task<IEnumerable<Employee>> QueryAsync(EmployeeSearchFilters filters, CancellationToken cancellationToken)
        {
            if (filters == null)
                throw new ArgumentNullException(nameof(filters));
            try
            {
                var tableReference = await CreateTableReference();

                var query = new TableQuery<Entities.Employee>().Where(filters.GenerateFilterCondition());

                var continuationToken = default(TableContinuationToken);
                var results = new List<Employee>();

                do
                {
                    TableQuerySegment<Entities.Employee> queryResult;
                    using (var sw = new StopWatcher(Logger, Storage.Logger.LogConstants.QueryDurationMetricName))
                    {
                        queryResult = await tableReference.ExecuteQuerySegmentedAsync(query, continuationToken,
                            default, default, cancellationToken);
                        sw.AddProperty(Storage.Logger.LogConstants.QueryCountItemMetricName, queryResult.Count());
                    }
                    results.AddRange(queryResult.Results.Select(e => e.ToCoreEmployee()));

                    continuationToken = queryResult.ContinuationToken;

                } while (continuationToken != null && !cancellationToken.IsCancellationRequested);

                return results;
            }
            catch (StorageException e)
            {
                this.Logger.LogError(e, "Error during QueryAsync operation");
                throw;
            }
        }

        public async Task<bool> InsertAsync(Employee employee, CancellationToken cancellationToken)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            try
            {
                var tableReference = await CreateTableReference();
                var entity = employee.ToTableEmployee();
                TableOperation operation = TableOperation.Insert(entity);

                TableResult result = await tableReference.ExecuteAsync(operation, default, default, cancellationToken);

                return result.HttpStatusCode >= 200 && result.HttpStatusCode <= 299;
            }
            catch (StorageException e)
            {
                this.Logger.LogError(e, "Error during InsertAsync operation");
                throw;
            }

        }
        public async Task<bool> UpdateAsync(Employee employee, CancellationToken cancellationToken)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            try
            {
                var entity = await this.GetByIdInternalAsync(employee.EmployeeId, cancellationToken);
                if (entity != null)
                {
                    entity.FillTableEmployee(employee);
                    var tableReference = await CreateTableReference();

                    TableOperation operation = TableOperation.Replace(entity);

                    TableResult result = await tableReference.ExecuteAsync(operation, default, default, cancellationToken);

                    return result.HttpStatusCode >= 200 && result.HttpStatusCode <= 299;
                }
                return false;
            }
            catch (StorageException e)
            {
                this.Logger.LogError(e, "Error during UpdateAsync operation");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(string employeeId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(employeeId))
                throw new ArgumentException(nameof(employeeId));

            try
            {
                var entity = await this.GetByIdInternalAsync(employeeId, cancellationToken);
                if (entity != null)
                {
                    var tableReference = await CreateTableReference();

                    TableOperation operation = TableOperation.Delete(entity);

                    TableResult result = await tableReference.ExecuteAsync(operation,default,default,cancellationToken);

                    return result.HttpStatusCode >= 200 && result.HttpStatusCode <= 299;
                }
                return false;
            }
            catch (StorageException e)
            {
                this.Logger.LogError(e, "Error during DeleteAsync operation");
                throw;
            }
        }

        public async Task<bool> SeedAsync(int numberOfEmployees, CancellationToken cancellationToken)
        {
            if (numberOfEmployees <= 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfEmployees));

            var result = true;

            var tableReference = await CreateTableReference();

            var counter = 0;
            var maxBatchSize = 100;

            while (counter < numberOfEmployees)
            {
                var batchSize = Math.Min(numberOfEmployees - counter, maxBatchSize);
                var employees = EmployeesGenerator.Generate(batchSize);

                // here put the bacth insert
                TableBatchOperation batchOperation = new TableBatchOperation();
                foreach (var employee in employees)
                {
                    batchOperation.Insert(employee);
                }

                try
                {
                    await tableReference.ExecuteBatchAsync(batchOperation, default, default, cancellationToken);
                }
                catch (Exception e)
                {
                    this.Logger.LogError(e, "Error during SeedAsync operation");
                    throw;
                }

                counter += batchSize;
            }

            return result;
        }
        #endregion [ IEmployeesRepository interface ]

        #region [ Private methods ]
        private async Task<CloudTable> CreateTableReference()
        {
            CloudTableClient cloudTableClient = storageAccount.CreateCloudTableClient();
            var tableReference = cloudTableClient.GetTableReference(EmployeesTableName);

            await tableReference.CreateIfNotExistsAsync();
            return tableReference;
        }

        private async Task<Entities.Employee> GetByIdInternalAsync(string employeeId, CancellationToken cancellationToken)
        {
            try
            {
                var tableReference = await CreateTableReference();

                var query = new TableQuery<Entities.Employee>().Where((new EmployeeSearchFilters() { EmployeeId = employeeId }).GenerateFilterCondition());

                var continuationToken = default(TableContinuationToken);
                var queryResult = await tableReference.ExecuteQuerySegmentedAsync(query, continuationToken);

                return queryResult.Results.FirstOrDefault();
            }
            catch (StorageException e)
            {
                this.Logger.LogError(e, "Error during GetByIdAsync operation");
                throw;
            }
        }
        #endregion [ Private methods ]

    }
}
