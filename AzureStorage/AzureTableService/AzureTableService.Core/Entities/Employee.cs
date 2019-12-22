using System;
using System.Collections.Generic;
using System.Text;

namespace AzureTableService.Core.Entities
{
    public class Employee
    {
        public string DepartmentName { get; set; }
        public string EmployeeId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }

        public override string ToString()
        {
            return $"{EmployeeId} - {FirstName} {LastName} @ {DepartmentName}";
        }
    }
}
