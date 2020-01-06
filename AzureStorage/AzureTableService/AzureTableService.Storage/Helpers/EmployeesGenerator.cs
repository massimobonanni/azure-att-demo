using AzureTableService.Storage.Entities;
using FizzWare.NBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureTableService.Storage.Helpers
{
    internal static class EmployeesGenerator
    {
        private static IEnumerable<string> Departments =
            new List<string>() { "IT", "R&D", "Marketing", "Finance", "Operation","Logistic","Public Relations" };

		public static IEnumerable<Employee> Generate(int numberOfEmployees)
		{
			BuilderSetup.DisablePropertyNamingFor<Employee, string>(x => x.Email);
			BuilderSetup.DisablePropertyNamingFor<Employee, string>(x => x.DepartmentName);
			BuilderSetup.DisablePropertyNamingFor<Employee, DateTime?>(x => x.BirthDate);

			return Builder<Employee>
					.CreateListOfSize(numberOfEmployees)
						.All()
							.WithFactory(() => GenerateEmployee())
						.Build();
		}

		private static Employee GenerateEmployee()
		{
			var employee = new Employee();
			employee.FirstName = Faker.Name.First();
			employee.LastName = Faker.Name.Last();
			if (Faker.RandomNumber.Next(0, 100) < 50)
				employee.Email = $"{employee.FirstName}.{employee.LastName}@{Faker.Internet.DomainName()}";
			if (Faker.RandomNumber.Next(0, 100) < 50)
				employee.BirthDate = Faker.DateTimeFaker.BirthDay(18, 65);
			if (Faker.RandomNumber.Next(0, 100) < 50)
				employee.DepartmentName = Departments.ElementAt(Faker.RandomNumber.Next(0, Departments.Count()));
			return employee;
		}
	}
}
