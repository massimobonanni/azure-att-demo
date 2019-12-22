using AzureTableService.Storage.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureTableService.Storage.Extensions
{
    internal static class EmployeeExtensions
    {
        public static Core.Entities.Employee ToCoreEmployee(this Employee employee)
        {
            if (employee == null)
                throw new NullReferenceException(nameof(employee));

            return new Core.Entities.Employee()
            {
                BirthDate = employee.BirthDate,
                DepartmentName = employee.DepartmentName,
                Email = employee.Email,
                EmployeeId = employee.EmployeeId,
                FirstName = employee.FirstName,
                LastName = employee.LastName
            };
        }
    }
}
