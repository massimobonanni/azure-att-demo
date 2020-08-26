using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AzureTableService.Core.Entities;

namespace WebSite.Models.Vacations
{
    public class DetailViewModel
    {
        public string EmployeeId { get; set; }
        public string DepartmentName { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
       
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }

        public List<Vacation> Vacations { get; set; }

        public static DetailViewModel FromCoreEmployee(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            return new DetailViewModel()
            {
                BirthDate = employee.BirthDate,
                DepartmentName = employee.DepartmentName,
                Email = employee.Email,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                EmployeeId = employee.EmployeeId
            };
        }
    }
}
