using ApplicationInsight.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationInsight.Presentation.Models.EmployeesController
{
    public class EditModel
    {
        public Guid EmployeeId { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required] 
        public string FirstName { get; set; }
    }
}
