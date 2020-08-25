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
        Task<IEnumerable<Vacation>> QueryAsync(VacationSearchFilters filter, CancellationToken cancellationToken);

        Task<bool> UpsertAsync(Vacation vacation, CancellationToken cancellationToken);

        Task<bool> DeleteAsync(string employeeId, DateTime vacationDate, CancellationToken cancellationToken);
    }
}
