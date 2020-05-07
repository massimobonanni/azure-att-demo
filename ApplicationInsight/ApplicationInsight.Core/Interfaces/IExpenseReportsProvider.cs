using ApplicationInsight.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationInsight.Core.Interfaces
{
    public interface IExpenseReportsProvider
    {
        Task<IEnumerable<ExpenseReportItem>> GetExpenseReportByEmployeeAsync(Guid emoloyeeId, int year, int month, CancellationToken cancellationToken);

        Task<bool> AddExpenseReportItemAsync(ExpenseReportItem item, CancellationToken cancellationToken);

        Task<bool> RemoveExpenseReportItemAsync(Guid itemId, CancellationToken cancellationToken);
    }
}
