using ApplicationInsight.Core.Entities;
using ApplicationInsight.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationInsight.Business.Services
{
    public class MockEmployeesProvider : IEmployeesProvider
    {
        private readonly ILogger<MockEmployeesProvider> _logger;

        public MockEmployeesProvider(ILogger<MockEmployeesProvider> logger)
        {
            _logger = logger;
        }

        private static readonly ICollection<Employee> _innerEmployees =
            new List<Employee>() {
                    new Employee() { Id=Guid.NewGuid(), FirstName="Giuseppe",LastName="Rossi"},
                    new Employee() { Id=Guid.NewGuid(), FirstName="Carlo",LastName="Bianchi"},
                    new Employee() { Id=Guid.NewGuid(), FirstName="Mario",LastName="Verdi"},
                    new Employee() { Id=Guid.NewGuid(), FirstName="Laura",LastName="Bianchini"}
            };

        public Task<bool> DeleteEmployeeAsync(Guid employeeId, CancellationToken cancellationToken)
        {
            var innerEmployee = _innerEmployees.FirstOrDefault(e => e.Id == employeeId);

            if (innerEmployee == null)
                return Task.FromResult(false);

            _innerEmployees.Remove(innerEmployee);
            return Task.FromResult(true);

        }

        public Task<Employee> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken)
        {
            var employee = _innerEmployees.FirstOrDefault(e => e.Id == employeeId);
            return Task.FromResult(employee);
        }

        public Task<IEnumerable<Employee>> GetEmployeesAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_innerEmployees.AsEnumerable());
        }

        public Task<bool> InsertEmployeeAsync(Employee employee, CancellationToken cancellationToken)
        {
            _innerEmployees.Add(employee);
            return Task.FromResult(true);
        }

        public Task<bool> UpdateEmployeeAsync(Employee employee, CancellationToken cancellationToken)
        {
            var innerEmployee = _innerEmployees.FirstOrDefault(e => e.Id == employee.Id);

            if (innerEmployee == null)
                return Task.FromResult(false);

            _innerEmployees.Remove(innerEmployee);
            _innerEmployees.Add(employee);

            return Task.FromResult(true);
        }
    }
}
