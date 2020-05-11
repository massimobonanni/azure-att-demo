using ApplicationInsight.Core.Entities;
using ApplicationInsight.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationInsight.Mocks
{
    public class MockExpenseReportsProvider : IExpenseReportsProvider
    {
        private readonly ILogger<MockExpenseReportsProvider> _logger;

        public MockExpenseReportsProvider(ILogger<MockExpenseReportsProvider> logger)
        {
            _logger = logger;
        }

        public Task<IEnumerable<ExpenseReportItem>> GetExpenseReportByEmployeeAsync(Guid emoloyeeId, int year, int month, CancellationToken cancellationToken)
        {
            var items = Repositories.ExpenseReportItems
                .Where(i => i.ExpenseDate.Year == year)
                .Where(i => i.ExpenseDate.Month == month)
                .ToList();

            return Task.FromResult<IEnumerable<ExpenseReportItem>>(items);
        }

        public Task<bool> AddExpenseReportItemAsync(ExpenseReportItem item, CancellationToken cancellationToken)
        {
            var innerItem = Repositories.ExpenseReportItems.FirstOrDefault(i => i == item);
            if (innerItem != null)
                Repositories.ExpenseReportItems.Remove(innerItem);

            Repositories.ExpenseReportItems.Add(item);
            return Task.FromResult(true);
        }

        public Task<bool> RemoveExpenseReportItemAsync(Guid itemId, CancellationToken cancellationToken)
        {
            var innerItem = Repositories.ExpenseReportItems.FirstOrDefault(i => i.Id == itemId);
            if (innerItem == null)
                return Task.FromResult(false);

            Repositories.ExpenseReportItems.Remove(innerItem);
            return Task.FromResult(true);
        }

        public Task<bool> RemoveExpenseReportItemsForEmployee(Guid employeeId, int? year, int? month, CancellationToken cancellationToken)
        {
            var items = Repositories.ExpenseReportItems.Where(i => i.EmployeeId == employeeId);
            if (year.HasValue)
                items = items.Where(i => i.ExpenseDate.Year == year.Value);
            if (month.HasValue)
                items = items.Where(i => i.ExpenseDate.Month == month.Value);

            items = items.ToList();

            if (items.Any())
            {
                foreach (var item in items)
                {
                    Repositories.ExpenseReportItems.Remove(item);
                }
            }
            return Task.FromResult(true);
        }
    }
}
