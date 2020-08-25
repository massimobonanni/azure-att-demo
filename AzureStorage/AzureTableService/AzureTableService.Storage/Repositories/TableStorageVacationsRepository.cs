using AzureTableService.Core.Entities;
using AzureTableService.Core.Interfaces;
using AzureTableService.Core.Logger;
using AzureTableService.Storage.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureTableService.Storage.Repositories
{
    public class TableStorageVacationsRepository : TableStorageRepositoryBase, IVacationsRepository
    {
        internal const string VacationsTableName = "Employees";

        public TableStorageVacationsRepository(ILoggerFactory loggerFactory, IConfiguration configuration)
            : base(loggerFactory, configuration)
        {
            this.Logger = loggerFactory.CreateLogger<TableStorageEmployeesRepository>();
        }

        #region [ IVacationsRepository interface ]
        public async Task<bool> DeleteAsync(string employeeId, DateTime vacationDate, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(employeeId))
                throw new ArgumentException(nameof(employeeId));

            try
            {
                var tableReference = await CreateTableReference(VacationsTableName);
                var entity = new AzureTableService.Storage.Entities.Vacation(employeeId, vacationDate);

                TableOperation operation = TableOperation.Delete(entity);

                var operationResult = await tableReference.ExecuteAsync(operation, default, default, cancellationToken);
                return operationResult.HttpStatusCode >= 200 && operationResult.HttpStatusCode <= 299;
            }
            catch (StorageException e)
            {
                this.Logger.LogError(e, "Error during InsertAsync operation");
                throw;
            }

        }

        public async Task<bool> UpsertAsync(Vacation vacation, CancellationToken cancellationToken)
        {
            if (vacation == null)
                throw new ArgumentNullException(nameof(vacation));
            if (string.IsNullOrWhiteSpace(vacation.EmployeeId))
                throw new ArgumentException(nameof(vacation.EmployeeId));

            bool result = true;
            try
            {
                var tableReference = await CreateTableReference(VacationsTableName);
                var entities = vacation.ToTableVacations();
                if (entities.Any())
                {
                    TableBatchOperation batchOperation = new TableBatchOperation();
                    foreach (var entity in entities)
                    {
                        TableOperation operation = TableOperation.InsertOrReplace(entity);
                        batchOperation.Add(operation);
                    }

                    var operationResult = await tableReference.ExecuteBatchAsync(batchOperation, null, null, cancellationToken);
                    result = operationResult.All(r => r.HttpStatusCode >= 200 && r.HttpStatusCode <= 299);
                }
            }
            catch (StorageException e)
            {
                this.Logger.LogError(e, "Error during InsertAsync operation");
                throw;
            }
            return result;
        }

        public async Task<IEnumerable<Vacation>> QueryAsync(VacationSearchFilters filters, CancellationToken cancellationToken)
        {
            if (filters == null)
                throw new ArgumentNullException(nameof(filters));
            if (string.IsNullOrWhiteSpace(filters.EmployeeId))
                throw new ArgumentException(nameof(filters.EmployeeId));

            try
            {
                var tableReference = await CreateTableReference(VacationsTableName);
                var strFilter = filters.GenerateFilterCondition();
                var query = new TableQuery<Entities.Vacation>().Where(strFilter);

                var continuationToken = default(TableContinuationToken);
                var results = new List<Vacation>();

                do
                {
                    TableQuerySegment<Entities.Vacation> queryResult;
                    using (var sw = new StopWatcher(Logger, Storage.Logger.LogConstants.QueryDurationMetricName))
                    {
                        queryResult = await tableReference.ExecuteQuerySegmentedAsync(query, continuationToken,
                            default, default, cancellationToken);
                        sw.AddProperty(Storage.Logger.LogConstants.QueryCountItemMetricName, queryResult.Count());
                        sw.AddProperty(Storage.Logger.LogConstants.QueryFilterName, strFilter);
                        sw.AddProperty(Storage.Logger.LogConstants.QueryTableName, tableReference.Name);
                    }
                    results.AddRange(queryResult.Results.Select(e => e.ToCoreVacation()));

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

        #endregion [ IVacationsRepository interface ]
    }
}
