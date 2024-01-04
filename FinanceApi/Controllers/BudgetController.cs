using FinanceApi.Data.Dtos;
using FinanceApi.Mapper;
using FinanceApi.Models;
using FinanceApi.Services;
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

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = userService.GetUserById(userId);

            int errorCode;
            string errorMessage;

            if (!budgetService.Create(user, budgetDto, out errorCode, out errorMessage))
            {
                switch (errorCode)
                {
                    case 400:
                        return BadRequest(errorMessage);
                    case 500:
                        return StatusCode(errorCode, errorMessage);
                    default:
                        throw new InvalidOperationException("Unexpected error code encountered.");
                }
            }

            return Ok("Budget created succesfully.");
        }



    }
}
