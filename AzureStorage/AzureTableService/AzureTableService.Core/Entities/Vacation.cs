
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureTableService.Core.Entities
{
    public class Vacation
    {
        public string EmployeeId { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"{EmployeeId} - [{FromDate.Date} , {ToDate.Date}]";
        }
    }
}
