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

        public static Employee ToTableEmployee(this Core.Entities.Employee employee)
        {
            if (employee == null)
                throw new NullReferenceException(nameof(employee));

            return new Employee(employee.EmployeeId)
            {
                BirthDate = employee.BirthDate,
                DepartmentName = employee.DepartmentName,
                Email = employee.Email,
                FirstName = employee.FirstName,
                LastName = employee.LastName
            };
        }

        public static void FillTableEmployee(this Employee tableEmployee, Core.Entities.Employee coreEmployee)
        {
            if (tableEmployee == null)
                throw new NullReferenceException(nameof(tableEmployee));
            if (coreEmployee == null)
                throw new ArgumentNullException(nameof(coreEmployee));

            tableEmployee.BirthDate = coreEmployee.BirthDate;
            tableEmployee.DepartmentName = coreEmployee.DepartmentName;
            tableEmployee.Email = coreEmployee.Email;
            tableEmployee.FirstName = coreEmployee.FirstName;
            tableEmployee.LastName = coreEmployee.LastName;
        }
    }
}
