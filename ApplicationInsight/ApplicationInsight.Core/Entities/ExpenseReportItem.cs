using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ApplicationInsight.Core.Entities
{
    public class ExpenseReportItem:IEquatable<ExpenseReportItem>
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid EmployeeId { get; set; }

        public DateTime ExpenseDate { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public static bool operator ==(ExpenseReportItem item1, ExpenseReportItem item2)
        {
            if (Object.ReferenceEquals(item1, null))
            {
                if (Object.ReferenceEquals(item2, null))
                {
                    return true;
                }
                return false;
            }
            return item1.Equals(item2);
        }

        public static bool operator !=(ExpenseReportItem item1, ExpenseReportItem item2)
        {
            return !(item1 == item2);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ExpenseReportItem);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public bool Equals([AllowNull] ExpenseReportItem other)
        {
            if (Object.ReferenceEquals(other, null))
            {
                return false;
            }

            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            if (this.GetType() != other.GetType())
            {
                return false;
            }

            return this.Id.Equals(other.Id);
        }
    }
}
