using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AzureTableService.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebSite.Models.Vacations;

namespace WebSite.Controllers
{
    public class VacationsController : Controller
    {
        private readonly IVacationsRepository vacationsRepository;
        private readonly IEmployeesRepository employeesRepository;
        private readonly ILogger logger;

        public VacationsController(ILogger<VacationsController> logger, IEmployeesRepository employeesRepository,
            IVacationsRepository vacationsRepository)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (employeesRepository == null)
                throw new ArgumentNullException(nameof(employeesRepository));
            if (vacationsRepository == null)
                throw new ArgumentNullException(nameof(vacationsRepository));

            this.logger = logger;
            this.employeesRepository = employeesRepository;
            this.vacationsRepository = vacationsRepository;
        }

        public async Task<ActionResult> Details(string employeeId)
        {
            var result = await this.employeesRepository.GetByIdAsync(employeeId, default);

            if (result == null)
            {
                return this.NotFound();
            }

            var model = DetailViewModel.FromCoreEmployee(result);

            var vacations = await this.vacationsRepository.QueryAsync(new VacationSearchFilters()
            {
                EmployeeId = employeeId,
                From = DateTime.Now.Date
            }, default);

            if (vacations != null)
            {
                model.Vacations = vacations.OrderBy(v => v.FromDate).ToList();
            }

            return View(model);
        }

        // GET: Employees/Create
        public async Task<ActionResult> Create(string employeeId)
        {
            var employee = await this.employeesRepository.GetByIdAsync(employeeId, default);

            if (employee == null)
            {
                return this.NotFound();
            }

            var model = new CreateViewModel();
            model.EmployeeId = employeeId;
            model.FirstName = employee.FirstName;
            model.LastName = employee.LastName;

            return View(model);
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateViewModel vacationModel)
        {
            if (ModelState.IsValid)
            {
                var vacation = vacationModel.ToCoreVacation();
                var result = await this.vacationsRepository.UpsertAsync(vacation, default);
                if (result)
                    return RedirectToAction(nameof(Details), new { employeeId = vacation.EmployeeId });

                ModelState.AddModelError(string.Empty, "An error occurs during vacation request");
            }
            return View(vacationModel);
        }

        // GET: Employees/Delete/5
        public async Task<ActionResult> Delete(string employeeId, DateTime date)
        {
            var employee = await this.employeesRepository.GetByIdAsync(employeeId, default);

            if (employee == null)
            {
                return this.NotFound();
            }

            var vacations = await this.vacationsRepository.QueryAsync(new VacationSearchFilters()
            {
                EmployeeId = employeeId,
                From = date,
                To = date
            }, default);

            if (vacations == null || !vacations.Any())
            {
                return this.NotFound();
            }

            var model = new DeleteViewModel();
            model.EmployeeId = employeeId;
            model.FirstName = employee.FirstName;
            model.LastName = employee.LastName;
            model.Date = vacations.First().FromDate;
            model.DateFormatted = model.Date.ToString("yyyyMMdd");
            model.Description = vacations.First().Description;

            return View(model);
        }

        // POST: Employees/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(DeleteViewModel vacationModel)
        {
            var date = DateTime.ParseExact(vacationModel.DateFormatted, "yyyyMMdd", CultureInfo.InvariantCulture);
            var result = await this.vacationsRepository.DeleteAsync(vacationModel.EmployeeId, date, default);
            if (result)
                return RedirectToAction(nameof(Details), new { employeeId = vacationModel.EmployeeId });

            ModelState.AddModelError(string.Empty, "An error occurs during cancelling vacation");

            return View(vacationModel);
        }
    }
}
