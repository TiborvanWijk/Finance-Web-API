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


        [HttpPost("Post")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult CreateExpense([FromBody] ExpenseDto expenseDto)
        {

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = userService.GetUserById(userId);

            int errorCode;
            string errorMessage;

            if (!expenseService.Create(user, expenseDto, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            if(expenseDto.Status && !userService.UpdateBalance(user, -expenseDto.Amount))
            {
                ModelState.AddModelError("UpdatingError", "Something went wrong while updating userbalance.");
                return StatusCode(500, ModelState);
            }

            return Ok("Expense created succesfully.");
        }


        [HttpPost("AssociateCategories/{expenseId}")]
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

            var user = userService.GetUserById(userId);

            int errorCode;
            string errorMessage;
            decimal prevAmount;

            if (!expenseService.Update(user, expenseDto, out errorCode, out errorMessage, out prevAmount))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            decimal balance = expenseDto.Status ? -(expenseDto.Amount - prevAmount) : prevAmount;

            if (!userService.UpdateBalance(user, balance))
            {
                return StatusCode(500, "Something went wrong with updating users balance.");
            }

            return Ok("Updated income succesfully.");
        }
        
    }
}
