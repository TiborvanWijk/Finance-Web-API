using FinanceApi.Data.Dtos;
using FinanceApi.Mapper;
using FinanceApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinanceApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BudgetController : Controller
    {
        private readonly IBudgetService budgetService;
        private readonly IUserService userService;

        public BudgetController(IBudgetService budgetService, IUserService userService)
        {
            this.budgetService = budgetService;
            this.userService = userService;
        }



        [HttpGet("current")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult GetAllBudgets()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var budgets = budgetService.GetAllOfUser(userId).Select(Map.ToBudgetDto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            return Ok(budgets);
        }

        [HttpPost("post")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult CreateBudget([FromBody] BudgetDto budgetDto)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (budgetDto.StartDate > budgetDto.EndDate)
            {
                ModelState.AddModelError("TimePeriodError", "Starting time is later then ending time.");
                return BadRequest(ModelState);
            }
            if (budgetDto.LimitAmount <= 0)
            {
                ModelState.AddModelError("AmountError", "Limit amount must me greater then '0'.");
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (budgetService.ExistsByTitle(userId, budgetDto.Title))
            {
                ModelState.AddModelError("Duplicate", "Budget with this title already exists.");
                return BadRequest(ModelState);
            }

            var budget = Map.ToBudget(budgetDto);
            budget.User = userService.GetUserById(userId);

            if (!budgetService.Create(budget))
            {
                ModelState.AddModelError("SavingError", "Something went wrong while saving budget.");
                return StatusCode(500, ModelState);
            }

            return Ok("Budget created succesfully.");
        }



    }
}
