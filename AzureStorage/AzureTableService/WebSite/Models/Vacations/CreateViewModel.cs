using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AzureTableService.Core.Entities;

namespace WebSite.Models.Vacations
{
    public class CreateViewModel
    {
        public string EmployeeId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime FromDate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime ToDate { get; set; }

        [Required(ErrorMessage = "Description is mandatory")]
        public string Description { get; set; }

        public Vacation ToCoreVacation()
        {
            return new Vacation()
            {
                EmployeeId = EmployeeId,
                FromDate=FromDate,
                ToDate=ToDate,
                Description=Description
            };
        }
    }
}
