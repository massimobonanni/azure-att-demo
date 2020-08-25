using AzureTableService.Storage.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureTableService.Storage.Extensions
{
    internal static class VacationExtensions
    {
        public static Core.Entities.Vacation ToCoreVacation(this Vacation vacation)
        {
            if (vacation == null)
                throw new NullReferenceException(nameof(vacation));

            return new Core.Entities.Vacation()
            {
                EmployeeId = vacation.EmployeeId,
                Description = vacation.Description,
                FromDate = vacation.Date,
                ToDate = vacation.Date
            };
        }

        public static IEnumerable<Vacation> ToTableVacations(this Core.Entities.Vacation vacation)
        {
            if (vacation == null)
                throw new NullReferenceException(nameof(vacation));
            if (string.IsNullOrWhiteSpace(vacation.EmployeeId))
                throw new ArgumentException(nameof(vacation.EmployeeId));

            var tableVacations = new List<Vacation>();
            var startDate = vacation.FromDate;

            while (startDate <= vacation.ToDate)
            {
                if (startDate.DayOfWeek != DayOfWeek.Saturday && startDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    tableVacations.Add(new Vacation(vacation.EmployeeId, startDate) { Description = vacation.Description });
                }
                startDate = startDate.AddDays(1);
            }
            return tableVacations;
        }

    }
}
