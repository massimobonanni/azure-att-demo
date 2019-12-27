using System;
using System.Collections.Generic;
using System.Text;
using AzureTableService.Storage.Entities;
using AzureTableService.Storage.Repositories;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureTableService.Core.Interfaces
{
    internal static class EmployeeSearchFiltersExtensions
    {

        public static string GenerateFilterCondition(this EmployeeSearchFilters filters)
        {
            if (filters == null)
                throw new NullReferenceException(nameof(filters));

            var filter = TableQuery.GenerateFilterCondition("PartitionKey", 
                QueryComparisons.Equal, Employee.EmployeesPartitionKey);

            if (!string.IsNullOrEmpty(filters.EmployeeId))
            {
                filter = TableQuery.CombineFilters(filter,
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, filters.EmployeeId));
            }

            if (!string.IsNullOrEmpty(filters.FirstName))
            {
                filter = TableQuery.CombineFilters(filter,
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("FirstName", QueryComparisons.Equal, filters.FirstName));
            }

            if (!string.IsNullOrEmpty(filters.LastName))
            {
                filter = TableQuery.CombineFilters(filter,
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("LastName", QueryComparisons.Equal, filters.LastName));
            }

            return filter;
        }
    }
}
