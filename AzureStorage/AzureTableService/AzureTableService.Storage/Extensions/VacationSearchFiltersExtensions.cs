using System;
using System.Collections.Generic;
using System.Text;
using AzureTableService.Storage.Entities;
using AzureTableService.Storage.Repositories;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureTableService.Core.Interfaces
{
    internal static class VacationSearchFiltersExtensions
    {

        public static string GenerateFilterCondition(this VacationSearchFilters filters)
        {
            if (filters == null)
                throw new NullReferenceException(nameof(filters));

            var filter = TableQuery.GenerateFilterCondition("PartitionKey",
                QueryComparisons.Equal, filters.EmployeeId);

            if (filters.From.HasValue)
            {
                filter = TableQuery.CombineFilters(filter,
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, filters.From.Value.ToString("yyyyMMdd")));
            }

            if (filters.To.HasValue)
            {
                filter = TableQuery.CombineFilters(filter,
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThanOrEqual, filters.To.Value.ToString("yyyyMMdd")));
            }

            return filter;
        }
    }
}
