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
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IEmployeesProvider _employeesProvider;

        public EmployeesController(ILogger<WeatherForecastController> logger,
            IEmployeesProvider employeesProvider)
        {
            _logger = logger;
            _employeesProvider = employeesProvider;
        }

        [HttpGet]
        public async Task<IEnumerable<Employee>> Get()
        {
            var employees = await _employeesProvider.GetEmployeesAsync(default);
            return employees;
        }
    }
}
