using ApplicationInsight.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationInsight.Presentation.Models.EmployeesController
{
    public class DetailsModel
    {
        public Employee Employee { get; set; }

        public int Year { get; set; }
        public int Month { get; set; }
    }
}
