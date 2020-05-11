using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationInsight.Core.Entities;
using ApplicationInsight.Core.Interfaces;
using ApplicationInsight.Presentation.Models.EmployeesController;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApplicationInsight.Presentation.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ILogger<EmployeesController> _logger;
        private readonly IEmployeesProvider _employeesProvider;

        public EmployeesController(IEmployeesProvider employeesProvider,
            ILogger<EmployeesController> logger)
        {
            if (employeesProvider == null)
                throw new ArgumentNullException(nameof(employeesProvider));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            this._employeesProvider = employeesProvider;
            this._logger = logger;
        }

        // GET: Employees
        public async Task<ActionResult> Index()
        {
            var model = new IndexModel();
            var employees = await this._employeesProvider.GetEmployeesAsync(default);

            model.Employees = employees.OrderBy(e => e.LastName).ThenBy(e => e.FirstName);
            return View(model);
        }

        // GET: Employees/Details/5
        public async Task<ActionResult> Details(string id, [FromQuery] int? year = null, [FromQuery] int? month = null)
        {
            Guid employeeId;
            if (!Guid.TryParse(id, out employeeId))
            {
                this._logger.LogWarning("Employee Details invoked with non valid parameter id='{0}'", id);
                return RedirectToAction(nameof(Index));
            }

            var model = new DetailsModel();
            model.Employee = await this._employeesProvider.GetEmployeeAsync(employeeId, default);

            if (model.Employee == null)
            {
                this._logger.LogWarning("Employee Details invoked with not existing employee id='{0}'", id);
                return RedirectToAction(nameof(Index));
            }

            model.Month = month.HasValue ? month.Value : DateTime.Now.Month;
            model.Year = year.HasValue ? year.Value : DateTime.Now.Year;

            return View(model);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            var model = new CreateModel();
            return View(model);
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var employee = new Employee()
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName
                    };
                    var result = await this._employeesProvider.InsertEmployeeAsync(employee, default);
                    if (result)
                        return RedirectToAction(nameof(Index));

                    ModelState.AddModelError(string.Empty, "Error during inserting employee");
                }
                catch
                {
                    ModelState.AddModelError(string.Empty, "Error during inserting employee");
                }
            }
            return View();
        }

        // GET: Employees/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            Guid employeeId;
            if (!Guid.TryParse(id, out employeeId))
            {
                this._logger.LogWarning("Employee Edit invoked with non valid parameter id='{0}'", id);
                return RedirectToAction(nameof(Index));
            }

            var model = new EditModel();
            var employee = await this._employeesProvider.GetEmployeeAsync(employeeId, default);

            if (employee == null)
            {
                this._logger.LogWarning("Employee Edit invoked with not existing employee id='{0}'", id);
                return RedirectToAction(nameof(Index));
            }

            model.FirstName = employee.FirstName;
            model.LastName = employee.LastName;
            model.EmployeeId = employee.Id;

            return View(model);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var employee = new Employee()
                    {
                        Id = model.EmployeeId,
                        FirstName = model.FirstName,
                        LastName = model.LastName
                    };
                    var result = await this._employeesProvider.UpdateEmployeeAsync(employee, default);
                    if (result)
                        return RedirectToAction(nameof(Index));

                    ModelState.AddModelError(string.Empty, "Error during Updating employee");
                }
                catch
                {
                    ModelState.AddModelError(string.Empty, "Error during Updating employee");
                }
            }
            return View();
        }


        // GET: Employees/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            Guid employeeId;
            if (!Guid.TryParse(id, out employeeId))
            {
                this._logger.LogWarning("Employee Delete invoked with non valid parameter id='{0}'", id);
                return RedirectToAction(nameof(Index));
            }

            var model = new DeleteModel();
            var employee = await this._employeesProvider.GetEmployeeAsync(employeeId, default);

            if (employee == null)
            {
                this._logger.LogWarning("Employee Delete invoked with not existing employee id='{0}'", id);
                return RedirectToAction(nameof(Index));
            }

            model.FirstName = employee.FirstName;
            model.LastName = employee.LastName;
            model.EmployeeId = employee.Id;

            return View(model);
        }

        // POST: Employees/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(DeleteModel model)
        {
            try
            {
                var result = await this._employeesProvider.DeleteEmployeeAsync(model.EmployeeId, default);
                if (result)
                    return RedirectToAction(nameof(Index));

                ModelState.AddModelError(string.Empty, "Error during Deleting employee");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Error during Deleting employee");
            }
            return View();
        }
    }
}