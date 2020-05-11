using ApplicationInsight.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationInsight.Presentation.Models.EmployeesController
{
    public class IndexModel
    {
        public IEnumerable<Employee> Employees { get; set; }
    }
}
