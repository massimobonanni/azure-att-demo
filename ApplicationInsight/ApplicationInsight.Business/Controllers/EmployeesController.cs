using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationInsight.Core.Entities;
using ApplicationInsight.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApplicationInsight.Business.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly ILogger<EmployeesController> _logger;
        private readonly IEmployeesProvider _employeesProvider;

        public EmployeesController(ILogger<EmployeesController> logger,
            IEmployeesProvider employeesProvider)
        {
            _logger = logger;
            _employeesProvider = employeesProvider;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesAsync()
        {
            var employees = await _employeesProvider.GetEmployeesAsync(default);
            return Ok(employees);
        }

        [HttpGet("{employeeId}")]
        public async Task<ActionResult<Employee>> GetEmployeeAsync(Guid employeeId)
        {
            var employee = await _employeesProvider.GetEmployeeAsync(employeeId, default);
            return Ok(employee);
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> InsertEmployeeAsync([FromBody] Employee employee)
        {
            if (employee == null)
                return BadRequest($"{nameof(employee)} not valid");

            employee.Id = Guid.NewGuid();
            var result = await _employeesProvider.InsertEmployeeAsync(employee, default);

            return Ok(result);
        }

        [HttpPut]
        public async Task<ActionResult<Employee>> UpdateEmployeeAsync([FromBody] Employee employee)
        {
            if (employee == null)
                return BadRequest($"{nameof(employee)} not valid");

            var result = await _employeesProvider.UpdateEmployeeAsync(employee, default);

            return Ok(result);
        }

        [HttpDelete("{employeeId}")]
        public async Task<ActionResult<bool>> DeleteEmployeeAsync(Guid employeeId)
        {
            var result = await _employeesProvider.DeleteEmployeeAsync(employeeId, default);

            return Ok(result);
        }
    }
}
