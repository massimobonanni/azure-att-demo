using System;

namespace AzureTableService.Core.Interfaces
{
    public class VacationSearchFilters : SearchFilters
    {
        public string EmployeeId { get; set; }

        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}