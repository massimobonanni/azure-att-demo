using AzureTableService.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureTableService.Core.Interfaces
{
    public interface IEmployeesRepository
    {
        Task<IEnumerable<Employee>> QueryAsync(EmployeeSearchFilters filter,CancellationToken cancellationToken);

        Task<Employee> GetByIdAsync(string employeeId, CancellationToken cancellationToken);
    }
}
