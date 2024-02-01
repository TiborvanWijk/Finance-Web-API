using FinanceApi.Controllers.ApiResponseHelpers;
using FinanceApi.Services.Interfaces;
using FinanceApi.Validators;
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
        private readonly IAuthorizeService authorizeService;

        public FinancialController(IIncomeService incomeService, IExpenseService expenseService, IFinancialService financialService, IAuthorizeService authorizeService)
        {
            this.incomeService = incomeService;
            this.expenseService = expenseService;
            this.financialService = financialService;
            this.authorizeService = authorizeService;
        }



        [HttpGet("netincome")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult GetNetIncome([FromQuery] DateTime? startdate, [FromQuery] DateTime? endDate, [FromQuery] string? optionalOwnerId)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            int errorCode;
            string errorMessage;

            if (!authorizeService.ValidateUsers(HttpContext, currUserId, optionalOwnerId, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            var userLookupId = optionalOwnerId == null ? currUserId : optionalOwnerId;

            decimal netIncome;
            if (!financialService.TryGetNetIncomeInTimePeriod(userLookupId, startdate, endDate, out netIncome, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok(netIncome);
        }

        [HttpGet("get_savings_rate")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetSavingsRate([FromQuery] DateTime? startdate, [FromQuery] DateTime? endDate, [FromQuery] string? optionalOwnerId)
        {
            var currUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;


            int errorCode;
            string errorMessage;

            if (!authorizeService.ValidateUsers(HttpContext, currUserId, optionalOwnerId, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            var userLookupId = optionalOwnerId == null ? currUserId : optionalOwnerId;

            bool validDate = false;
            if(startdate != null || endDate != null)
            {
                if(!Validator.ValidateTimePeriod(startdate, endDate, out errorCode, out errorMessage))
                {
                    return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
                }
                validDate = true;
            }

            decimal? savingsRate = await financialService.tryGetSavingsRateAsync(userLookupId, validDate, startdate, endDate);

            if(savingsRate == null)
            {
                return ApiResponseHelper.HandleErrorResponse(500, "CalculationError");
            }

            return Ok(savingsRate);            
        }
    }
}
