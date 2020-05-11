using ApplicationInsight.Core.Entities;
using ApplicationInsight.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationInsight.Mocks
{
    public class MockEmployeesProvider : IEmployeesProvider
    {
        private readonly ILogger<MockEmployeesProvider> _logger;

        public MockEmployeesProvider(ILogger<MockEmployeesProvider> logger)
        {
            _logger = logger;
        }

        public Task<bool> DeleteEmployeeAsync(Guid employeeId, CancellationToken cancellationToken)
        {
            var innerEmployee = Repositories.Employees.FirstOrDefault(e => e.Id == employeeId);

            if (innerEmployee == null)
                return Task.FromResult(false);

            Repositories.Employees.Remove(innerEmployee);
            return Task.FromResult(true);

        }

        public Task<Employee> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken)
        {
            var employee = Repositories.Employees.FirstOrDefault(e => e.Id == employeeId);
            return Task.FromResult(employee);
        }

        public Task<IEnumerable<Employee>> GetEmployeesAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Repositories.Employees.AsEnumerable());
        }

        public Task<bool> InsertEmployeeAsync(Employee employee, CancellationToken cancellationToken)
        {
            Repositories.Employees.Add(employee);
            return Task.FromResult(true);
        }

        public Task<bool> UpdateEmployeeAsync(Employee employee, CancellationToken cancellationToken)
        {
            var innerEmployee = Repositories.Employees.FirstOrDefault(e => e == employee);

            if (innerEmployee == null)
                return Task.FromResult(false);

            Repositories.Employees.Remove(innerEmployee);
            Repositories.Employees.Add(employee);

            return Task.FromResult(true);
        }
    }
}
