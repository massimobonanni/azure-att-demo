using ApplicationInsight.Core.Entities;
using ApplicationInsight.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationInsight.Business.Services
{
    public class MockEmployeesProvider : IEmployeesProvider
    {
        private static readonly ICollection<Employee> _innerEmployees =
            new List<Employee>() {
                    new Employee() { Id=Guid.NewGuid(), FirstName="Giuseppe",LastName="Rossi"},
                    new Employee() { Id=Guid.NewGuid(), FirstName="Carlo",LastName="Bianchi"},
                    new Employee() { Id=Guid.NewGuid(), FirstName="Mario",LastName="Verdi"},
                    new Employee() { Id=Guid.NewGuid(), FirstName="Laura",LastName="Bianchini"}
            };

        public Task<Employee> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken)
        {
            var employee = _innerEmployees.FirstOrDefault(e => e.Id == employeeId);
            return Task.FromResult(employee);
        }

        public Task<IEnumerable<Employee>> GetEmployeesAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_innerEmployees.AsEnumerable());
        }
    }
}
