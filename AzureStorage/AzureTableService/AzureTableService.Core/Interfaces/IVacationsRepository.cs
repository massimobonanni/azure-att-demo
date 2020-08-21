using AzureTableService.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureTableService.Core.Interfaces
{
    public interface IVacationsRepository
    {
        Task<IEnumerable<Employee>> QueryAsync(VacationsSearchFilters filter, CancellationToken cancellationToken);

        Task<bool> InsertAsync(Vacation vacation, CancellationToken cancellationToken);

        Task<bool> UpdateAsync(Vacation vacation, CancellationToken cancellationToken);

        Task<bool> DeleteAsync(string employeeId, DateTime vacationDate, CancellationToken cancellationToken);
    }
}
