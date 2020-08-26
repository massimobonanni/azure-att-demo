using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AzureTableService.Storage.Entities
{
    internal class Vacation : TableEntity
    {
        public Vacation() : base()
        {

        }

        public Vacation(string employeeId, DateTime date) : base()
        {
            this.RowKey = date.ToString("yyyyMMdd");
            this.PartitionKey = employeeId;
        }

        public string EmployeeId
        {
            get => this.PartitionKey;
            set => this.PartitionKey = value;
        }

        [IgnoreProperty]
        public DateTime Date
        {
            get => DateTime.ParseExact(this.RowKey, "yyyyMMdd", CultureInfo.InvariantCulture);
            set => this.RowKey = value.ToString("yyyyMMdd");
        }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"{EmployeeId} - {Date} , {Description}";
        }
    }
}
