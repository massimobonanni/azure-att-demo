using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzureTableService.Core.Entities;
using AzureTableService.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebSite.Models.Employees;

namespace WebSite.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IEmployeesRepository employeesRepository;
        private readonly ILogger logger;

        public EmployeesController(ILogger<EmployeesController> logger, IEmployeesRepository employeesRepository)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (employeesRepository == null)
                throw new ArgumentNullException(nameof(employeesRepository));

            this.logger = logger;
            this.employeesRepository = employeesRepository;
        }

        // GET: Employees
        public async Task<ActionResult> Index([FromQuery(Name = "filterFirstName")]string filterFirstName, [FromQuery(Name = "filterLastName")]string filterLastName)
        {
            var filters = new EmployeeSearchFilters()
            {
                FirstName = filterFirstName,
                LastName = filterLastName
            };

            var result = await this.employeesRepository.QueryAsync(filters, default);

            var model = new IndexViewModel() { Filters = filters, Employees = result };

            return View(model);
        }

        // GET: Employees/Details/5
        public async Task<ActionResult> Details(string id)
        {
            var result = await this.employeesRepository.GetByIdAsync(id, default);

            if (result == null)
            {
                return this.NotFound();
            }

            var model = EditViewModel.FromCoreEmployee(result);

            return View(model);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            var model = new CreateViewModel();
            return View(model);
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateViewModel employeeModel)
        {
            if (ModelState.IsValid)
            {
                var employee = employeeModel.ToCoreEmployee();
                var result = await this.employeesRepository.InsertAsync(employee, default);
                if (result)
                    return RedirectToAction(nameof(Index));

                ModelState.AddModelError(string.Empty, "An error occurs during insert operation");
            }
            return View(employeeModel);
        }

        // GET: Employees/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            var result = await this.employeesRepository.GetByIdAsync(id, default);

            if (result == null)
            {
                return this.NotFound();
            }

            var model = EditViewModel.FromCoreEmployee(result);

            return View(model);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditViewModel employeeModel)
        {
            if (ModelState.IsValid)
            {
                var employee = employeeModel.ToCoreEmployee();
                var result = await this.employeesRepository.UpdateAsync(employee, default);
                if (result)
                    return RedirectToAction(nameof(Index));

                ModelState.AddModelError(string.Empty, "An error occurs during update operation");
            }
            return View(employeeModel);
        }

        // GET: Employees/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            var result = await this.employeesRepository.GetByIdAsync(id, default);

            if (result == null)
            {
                return this.NotFound();
            }

            var model = DeleteViewModel.FromCoreEmployee(result);

            return View(model);
        }

        // POST: Employees/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(DeleteViewModel employeeModel)
        {
            var result = await this.employeesRepository.DeleteAsync(employeeModel.EmployeeId, default);
            if (result)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError(string.Empty, "An error occurs during delete operation");

            return View(employeeModel);
        }
    }
}