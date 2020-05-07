using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationInsight.Core.Entities
{
    public class Employee
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public static bool operator ==(Employee employee1, Employee employee2)
        {
            if (employee1 == null || employee2 == null)
                return false;
            return employee1.Equals(employee2);
        }

        public static bool operator !=(Employee employee1, Employee employee2)
        {
            if (employee1 == null || employee2 == null)
                return false;
            return !employee1.Equals(employee2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return this.Id.Equals(((Employee)obj).Id);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
