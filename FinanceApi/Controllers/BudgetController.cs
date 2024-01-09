using FinanceApi.Controllers.ApiResponseHelpers;
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

        
        [HttpGet("current/budget/{categoryId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult GetBudgetByCategoryId(int categoryId)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = userService.GetById(userId, true);

            ICollection<Budget> budgets;

            int errorCode;
            string errorMessage;

            if (!budgetService.TryGetBudgetsByCategoryId(user, categoryId, out budgets, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
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

            budgetDto.Id = 0;

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = userService.GetById(userId, true);

            int errorCode;
            string errorMessage;

            if (!budgetService.Create(user, budgetDto, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Budget created succesfully.");
        }

        [HttpPost("AssociateCategories/{budgetId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult AddCategoriesToBudget(int budgetId, [FromBody] ICollection<int> categoryIds)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            string errorMessage;
            int errorCode;
            if (!budgetService.AddCategories(User.FindFirst(ClaimTypes.NameIdentifier).Value, budgetId, categoryIds, out errorMessage, out errorCode))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Categories successfully added to expense.");
        }

        [HttpPut("put")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateBudget([FromBody] BudgetDto budgetDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = userService.GetById(userId, true);

            int errorCode;
            string errorMessage;

            if (!budgetService.Update(user, budgetDto, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Budget updated succesfully.");
        }



    }
}
