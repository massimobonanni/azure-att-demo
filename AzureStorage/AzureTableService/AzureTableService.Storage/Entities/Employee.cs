﻿using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureTableService.Storage.Entities
{
    internal class Employee : TableEntity
    {
        internal const string EmployeesLogicalTableName = "EMPLOYEES";

        public Employee():base()
        {
            
        }

        public Employee( string employeeId) : base()
        {
            this.RowKey = employeeId;
            this.PartitionKey = EmployeesLogicalTableName;
        }

        public string DepartmentName { get; set; }
        public string EmployeeId { get => this.RowKey; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }

        public override string ToString()
        {
            return $"{EmployeeId} - {FirstName} {LastName} @ {DepartmentName}";
        }
    }
}
