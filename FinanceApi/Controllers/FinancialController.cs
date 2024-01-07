using FinanceApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinanceApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FinancialController : Controller
    {
        private readonly IIncomeService incomeService;
        private readonly IExpenseService expenseService;
        private readonly IFinancialService financialService;

        public FinancialController(IIncomeService incomeService, IExpenseService expenseService, IFinancialService financialService)
        {
            this.incomeService = incomeService;
            this.expenseService = expenseService;
            this.financialService = financialService;
        }




        [HttpGet("GetSavingsRate")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult GetSavingsRate()
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var rate = financialService.GetSavingsRate(userId);

            return Ok(rate);            
        }
        [HttpGet("GetSavingsRateInTimePeriod")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult GetSavingsRateInTimePeriod([FromQuery] DateTime? startdate, [FromQuery] DateTime? endDate)
        {

            if (!ModelState.IsValid || startdate == null || endDate == null)
            {
                ModelState.AddModelError("InvalidDate", "Start or end date is invalid.");
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var rate = financialService.GetSavingsRateInTimePeriod(userId, startdate, endDate);

            return Ok(rate);
        }

    }
}
