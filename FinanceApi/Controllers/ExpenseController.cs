using FinanceApi.Controllers.ApiResponseHelpers;
using FinanceApi.Data.Dtos;
using FinanceApi.Mapper;
using FinanceApi.Models;
using FinanceApi.Services.Interfaces;
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
        private readonly IAuthorizeService authorizeService;

        public ExpenseController(IExpenseService expenseService, IUserService userService, ICategoryService categoryService, IAuthorizeService authorizeService)
        {
            this.expenseService = expenseService;
            this.userService = userService;
            this.categoryService = categoryService;
            this.authorizeService = authorizeService;
        }



        [HttpGet("current")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult GetExpenses([FromQuery] DateTime? from, [FromQuery] DateTime? to,
            [FromQuery] string? list_order_by, [FromQuery] string? list_dir, [FromQuery] string? optionalOwnerId)
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
            
            ICollection<Expense> expenses;

            if (!expenseService.TryGetExpensesFilteredOrDefault(userLookupId, out expenses, from, to, list_order_by, list_dir, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            var expenseDtos = expenses.Select(Map.ToExpenseDto);

            return Ok(expenseDtos);
        }

        [HttpGet("current/expenses/{categoryId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult GetExpenses(int categoryId, [FromQuery] string? optionalOwnerId)
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
            var user = userService.GetById(userLookupId, true);

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
        public IActionResult CreateExpense([FromBody] ExpenseDto expenseDto, [FromQuery] string? optionalOwnerId)
        {

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            expenseDto.Id = 0;
            var currUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;


            int errorCode;
            string errorMessage;

            if (!authorizeService.ValidateUsers(HttpContext, currUserId, optionalOwnerId, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            var userLookupId = optionalOwnerId == null ? currUserId : optionalOwnerId;
            var user = userService.GetById(userLookupId, true);

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
        public IActionResult AddCategoryToExpense(int expenseId, [FromBody] ICollection<int> categoryIds, [FromQuery] string? optionalOwnerId)
        {

            if(!ModelState.IsValid)
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
            
            if (!expenseService.AddCategories(userLookupId, expenseId, categoryIds, out errorMessage, out errorCode))
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
        public IActionResult UpdateExpense([FromBody] ExpenseDto expenseDto, [FromQuery] string? optionalOwnerId)
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
            var user = userService.GetById(userLookupId, true);

            if (!expenseService.Update(user, expenseDto, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Updated expense succesfully.");
        }

        [HttpDelete("delete/{expenseId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult DeleteExpense(int expenseId, [FromQuery] string? optionalOwnerId)
        {

            var currUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;


            int errorCode;
            string errorMessage;

            if (!authorizeService.ValidateUsers(HttpContext, currUserId, optionalOwnerId, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            var userLookupId = optionalOwnerId == null ? currUserId : optionalOwnerId;
            var user = userService.GetById(userLookupId, true);

            if (!expenseService.TryDeleteExpense(user, expenseId, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Expense succesfully deleted.");
        }

        [HttpDelete("remove_categories/{expenseId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult actionResult(int expenseId, [FromBody] ICollection<int> categoryIds, [FromQuery] string? optionalOwnerId)
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
            var user = userService.GetById(userLookupId, true);

            if (!expenseService.TryRemoveCategories(user, expenseId, categoryIds, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Categories deleted succesfully.");
        }


    }
}
