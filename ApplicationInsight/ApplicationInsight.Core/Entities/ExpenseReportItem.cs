using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationInsight.Core.Entities
{
    public class ExpenseReportItem
    {
        public Guid Id { get; set; }

        public Guid EmployeeId { get; set; }

        public DateTime ExpenseDate { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }
    }
}
