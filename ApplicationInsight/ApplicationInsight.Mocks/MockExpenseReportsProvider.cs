using ApplicationInsight.Core.Entities;
using ApplicationInsight.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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

        private static readonly ICollection<ExpenseReportItem> _innerItems =
            new List<ExpenseReportItem>();

        public Task<IEnumerable<ExpenseReportItem>> GetExpenseReportByEmployeeAsync(Guid emoloyeeId, int year, int month, CancellationToken cancellationToken)
        {
            var items = _innerItems
                .Where(i => i.ExpenseDate.Year == year)
                .Where(i => i.ExpenseDate.Month == month)
                .ToList();

            return Task.FromResult<IEnumerable<ExpenseReportItem>>(items);
        }

        public Task<bool> AddExpenseReportItemAsync(ExpenseReportItem item, CancellationToken cancellationToken)
        {
            var innerItem = _innerItems.FirstOrDefault(i => i == item);
            if (innerItem != null)
                _innerItems.Remove(innerItem);

            _innerItems.Add(item);
            return Task.FromResult(true);
        }

        public Task<bool> RemoveExpenseReportItemAsync(Guid itemId, CancellationToken cancellationToken)
        {
            var innerItem = _innerItems.FirstOrDefault(i => i.Id == itemId);
            if (innerItem == null)
                return Task.FromResult(false);

            _innerItems.Remove(innerItem);
            return Task.FromResult(true);
        }
    }
}
