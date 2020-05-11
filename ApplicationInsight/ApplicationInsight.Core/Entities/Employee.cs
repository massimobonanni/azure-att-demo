using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ApplicationInsight.Core.Entities
{
    public class Employee : IEquatable<Employee>
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public static bool operator ==(Employee employee1, Employee employee2)
        {
            return !(employee1 == employee2);
        }

        public static bool operator !=(Employee employee1, Employee employee2)
        {
            if (Object.ReferenceEquals(employee1, null))
            {
                if (Object.ReferenceEquals(employee2, null))
                {
                    return true;
                }
                return false;
            }
            return employee1.Equals(employee2);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Employee);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public bool Equals([AllowNull] Employee other)
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
