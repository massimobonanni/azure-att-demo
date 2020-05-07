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
    public class ReportItemsController : ControllerBase
    {
        private readonly ILogger<ReportItemsController> _logger;
        private readonly IExpenseReportsProvider _reportItemsProvider;

        public ReportItemsController(ILogger<ReportItemsController> logger,
            IExpenseReportsProvider reportItemsProvider)
        {
            _logger = logger;
            _reportItemsProvider = reportItemsProvider;
        }

        [HttpGet()]
        public async Task<ActionResult<Employee>> GetExpenseReportItemsForEmployeeAsync(Guid employeeId,
            int year, int month)
        {
            if (year < 0)
                return BadRequest($"{nameof(year)} not valid");
            if (month < 0)
                return BadRequest($"{nameof(month)} not valid");
            if (employeeId == Guid.Empty)
                return BadRequest($"{nameof(employeeId)} not valid");

            var items = await _reportItemsProvider.GetExpenseReportByEmployeeAsync(employeeId, year, month, default);
            return Ok(items.OrderBy(i => i.ExpenseDate));
        }

        [HttpPost()]
        public async Task<ActionResult<bool>> AddExpenseReportItemAsync([FromBody] ExpenseReportItem item)
        {
            if (item == null)
                return BadRequest($"{nameof(item)} not valid");

            var result = await _reportItemsProvider.AddExpenseReportItemAsync(item, default);

            return Ok(result);
        }

        [HttpDelete()]
        public async Task<ActionResult<bool>> RemoveExpenseReportItemAsync(Guid itemId)
        {
            var result = await _reportItemsProvider.RemoveExpenseReportItemAsync(itemId, default);

            if (result)
                return Ok(true);
            else
                return StatusCode(500);
        }
    }
}
