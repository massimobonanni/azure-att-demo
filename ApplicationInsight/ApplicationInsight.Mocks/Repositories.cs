using ApplicationInsight.Core.Entities;
using FizzWare.NBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApplicationInsight.Mocks
{
    internal static class Repositories
    {

        internal static readonly ICollection<Employee> Employees =
            new List<Employee>() {
                    new Employee() { Id=Guid.NewGuid(), FirstName="Giuseppe",LastName="Rossi"},
                    new Employee() { Id=Guid.NewGuid(), FirstName="Carlo",LastName="Bianchi"},
                    new Employee() { Id=Guid.NewGuid(), FirstName="Mario",LastName="Verdi"},
                    new Employee() { Id=Guid.NewGuid(), FirstName="Laura",LastName="Bianchini"}
            };

        internal static readonly ICollection<ExpenseReportItem> ExpenseReportItems =
            new List<ExpenseReportItem>();

        static Repositories()
        {
            foreach (var employee in Employees)
            {
                CreateExportReportItemsForEmployee(employee);
            }
        }

        private static void CreateExportReportItemsForEmployee(Employee employee)
        {
            var items = Builder<ExpenseReportItem>
                .CreateListOfSize(Faker.RandomNumber.Next(10, 100))
                .All()
                    .With(i => i.Amount = Faker.RandomNumber.Next(1, 100000) / 100.0m)
                    .With(i => i.Currency = "EUR")
                    .With(i => i.EmployeeId = employee.Id)
                    .With(i => i.ExpenseDate = DateTime.Now.AddDays(-Faker.RandomNumber.Next(1, 90)).Date)
                .Build();

            ((List<ExpenseReportItem>)ExpenseReportItems).AddRange(items);
        }
    }
}
