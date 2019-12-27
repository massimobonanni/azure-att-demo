using System.Collections.Generic;
using AzureTableService.Core.Entities;
using AzureTableService.Core.Interfaces;

namespace WebSite.Models.Employees
{
    public class IndexViewModel
    {
        public EmployeeSearchFilters Filters { get; set; }
        public IEnumerable<Employee> Employees { get; set; }
    }
}
