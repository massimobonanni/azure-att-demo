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

        Task<bool> AddExpenseReportAsync(ExpenseReportItem item, CancellationToken cancellationToken);
    }
}
