using FinanceApi.Controllers.ApiResponseHelpers;
using FinanceApi.Data.Dtos;
using FinanceApi.Enums;
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
    public class ExpenseController : Controller
    {
        private readonly IExpenseService expenseService;
        private readonly IUserService userService;
        private readonly ICategoryService categoryService;

        public ExpenseController(IExpenseService expenseService, IUserService userService, ICategoryService categoryService)
        {
            this.expenseService = expenseService;
            this.userService = userService;
            this.categoryService = categoryService;
        }



        [HttpGet("current")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult GetExpenses()
        {

            var expenses = expenseService.GetAllOfUser(User.FindFirst(ClaimTypes.NameIdentifier)?.Value).Select(Map.ToExpenseDto);

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(expenses);
        }

        [HttpGet("current/expenses/{categoryId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult GetExpenses(int categoryId)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = userService.GetById(userId, true);


            int errorCode;
            string errorMessage;
            ICollection<Expense> expenses;

            if (!expenseService.tryGetExpensesWithCategoryId(user, categoryId, out expenses, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            var expenseDtos = expenses.Select(Map.ToExpenseDto);

            return Ok(expenseDtos);
        }



        [HttpPost("post")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult CreateExpense([FromBody] ExpenseDto expenseDto)
        {

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            expenseDto.Id = 0;
            
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = userService.GetById(userId, true);

            int errorCode;
            string errorMessage;

            if (!expenseService.Create(user, expenseDto, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Expense created succesfully.");
        }


        [HttpPost("associate_categories/{expenseId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult AddCategoryToExpense(int expenseId, [FromBody] ICollection<int> categoryIds)
        {

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string errorMessage;
            int errorCode;
            if (!expenseService.AddCategories(User.FindFirst(ClaimTypes.NameIdentifier).Value, expenseId, categoryIds, out errorMessage, out errorCode))
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
        public IActionResult UpdateExpense([FromBody] ExpenseDto expenseDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = userService.GetById(userId, true);

            int errorCode;
            string errorMessage;
            decimal prevAmount;

            if (!expenseService.Update(user, expenseDto, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Updated income succesfully.");
        }

        [HttpDelete("delete/{expenseId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult DeleteExpense(int expenseId)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = userService.GetById(userId, true);

            int errorCode;
            string errorMessage;

            if (!expenseService.TryDeleteExpense(user, expenseId, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Expense succesfully deleted.");
        }

        [HttpDelete("removeCategories/{expenseId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult actionResult(int expenseId, [FromBody] ICollection<int> categoryIds)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = userService.GetById(userId, true);

            int errorCode;
            string errorMessage;


            if (!expenseService.TryRemoveCategories(user, expenseId, categoryIds, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Categories deleted succesfully.");
        }


    }
}
