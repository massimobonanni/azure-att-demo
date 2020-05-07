using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationInsight.Core.Entities
{
    public class ExpenseReportItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid EmployeeId { get; set; }

        public DateTime ExpenseDate { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public static bool operator ==(ExpenseReportItem item1, ExpenseReportItem item2)
        {
            if (item1 == null || item2 == null)
                return false;
            return item1.Equals(item2);
        }

        public static bool operator !=(ExpenseReportItem item1, ExpenseReportItem item2)
        {
            if (item1 == null || item2 == null)
                return false;
            return !item1.Equals(item2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return this.Id.Equals(((ExpenseReportItem)obj).Id);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
