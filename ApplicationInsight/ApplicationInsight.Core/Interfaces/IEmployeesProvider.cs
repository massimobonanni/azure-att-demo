using ApplicationInsight.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationInsight.Core.Interfaces
{
    public interface IEmployeesProvider
    {
        Task<IEnumerable<Employee>> GetEmployeesAsync(CancellationToken cancellationToken);

        Task<Employee> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken);

        Task<bool> InsertEmployeeAsync(Employee employee, CancellationToken cancellationToken);

        Task<bool> UpdateEmployeeAsync(Employee employee, CancellationToken cancellationToken);

        Task<bool> DeleteEmployeeAsync(Guid employeeId, CancellationToken cancellationToken);
    }
}
