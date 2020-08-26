using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AzureTableService.Core.Entities;

namespace WebSite.Models.Vacations
{
    public class DeleteViewModel
    {
        public string EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}", NullDisplayText = "")]
        public DateTime Date { get; set; }
        public string DateFormatted { get; set; }
        public string Description { get; set; }

        public Vacation ToCoreVacation()
        {
            return new Vacation()
            {
                FromDate = Date,
                ToDate = Date,
                Description = Description,
                EmployeeId = this.EmployeeId
            };
        }

    }
}
