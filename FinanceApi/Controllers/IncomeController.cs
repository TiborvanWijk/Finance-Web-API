using FinanceApi.Data.Dtos;
using FinanceApi.Mapper;
using FinanceApi.Models;
using FinanceApi.Services;
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
    public class IncomeController : Controller
    {
        private readonly IIncomeService incomeService;
        private readonly IUserService userService;

        public IncomeController(IIncomeService incomeService, IUserService userService)
        {
            this.incomeService = incomeService;
            this.userService = userService;
        }

        [HttpGet("current/incomes")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetUsersIncomes()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null || !userService.ExistsById(userId))
            {
                return NotFound();
            }

            ICollection<IncomeDto> incomeDtos = incomeService.GetAllByUserId(userId).Select(Map.ToIncomeDto).ToList();

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(incomeDtos);
        }

        [HttpPost("Create")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult CreateIncome([FromBody] IncomeDto incomeDto)
        {
            
            if (incomeDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!Validator.IsValidCurrencyCode(incomeDto.Currency))
            {
                ModelState.AddModelError("CurrencyCodeError", "Currency ISOcode is not valid.");
                return BadRequest(ModelState);
            }

            if (incomeDto.Amount <= 0)
            {
                ModelState.AddModelError("AmountError", "Amount must be more then '0'.");
                return BadRequest(ModelState);
            }

            var income = Map.ToIncome(incomeDto);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            income.User = userService.GetUserById(userId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!incomeService.Create(income))
            {
                ModelState.AddModelError("CreatingError", "Something went wrong while creating.");
                return StatusCode(500, ModelState);
            }

            
            if (income.Status && !userService.UpdateBalance(userId, income.Amount))
            {
                ModelState.AddModelError("UpdatingError", "Something went wrong while updating userbalance.");
            }

            return Ok("Income created succesfully.");
        }



        [HttpPost("AssociateCategories/{incomeId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult AddCategoryToExpense(int incomeId, [FromBody] ICollection<int> categoryIds)
        {
            string errorMessage;
            int responseCode;
            if (!incomeService.AddCategories(User.FindFirst(ClaimTypes.NameIdentifier).Value, incomeId, categoryIds, out errorMessage, out responseCode))
            {
                switch (responseCode)
                {
                    case 400:
                        return BadRequest(errorMessage);
                    case 404:
                        return NotFound(errorMessage);
                    case 500:
                        return StatusCode(500, errorMessage);
                    default:
                        break;
                }
            }

            return Ok("Categories successfully added to income.");
        }

    }
}
