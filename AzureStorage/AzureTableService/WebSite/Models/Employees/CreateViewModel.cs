using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AzureTableService.Core.Entities;

namespace WebSite.Models.Employees
{
    public class CreateViewModel
    {
        public string DepartmentName { get; set; }
        [Required(ErrorMessage = "First name is mandatory")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is mandatory")]
        public string LastName { get; set; }
        [EmailAddress(ErrorMessage = "The Email field is not a valid e-mail address.")]
        public string Email { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? BirthDate { get; set; }

        public Employee ToCoreEmployee()
        {
            return new Employee()
            {
                BirthDate = this.BirthDate,
                DepartmentName = this.DepartmentName,
                Email = this.Email,
                FirstName = this.FirstName,
                LastName = this.LastName,
                EmployeeId = Guid.NewGuid().ToString()
            };
        }
    }
}
