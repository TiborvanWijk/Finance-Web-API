using FinanceApi.Controllers.ApiResponseHelpers;
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



        [HttpGet("netincome")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult GetNetIncome([FromQuery] DateTime? startdate, [FromQuery] DateTime? endDate)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            decimal netIncome;
            int errorCode;
            string errorMessage;

            if (!financialService.TryGetNetIncomeInTimePeriod(userId, startdate, endDate, out netIncome, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok(netIncome);
        }

        [HttpGet("get_savings_rate")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult GetSavingsRate([FromQuery] DateTime? startdate, [FromQuery] DateTime? endDate)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            decimal savingsRate;
            int errorCode;
            string errorMessage;

            if (!financialService.tryGetSavingsRate(userId, out savingsRate, startdate, endDate, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok(savingsRate);            
        }
    }
}
