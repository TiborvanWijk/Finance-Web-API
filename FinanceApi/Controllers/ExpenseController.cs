using FinanceApi.Data.Dtos;
using FinanceApi.Enums;
using FinanceApi.Mapper;
using FinanceApi.Models;
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
            if (expenseDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!Validator.IsValidCurrencyCode(expenseDto.Currency))
            {
                ModelState.AddModelError("CurrencyCodeError", "Currency ISOcode is not valid.");
                return BadRequest(ModelState);
            }

            if(expenseDto.Amount <= 0)
            {
                ModelState.AddModelError("AmountError", "Amount must be more then '0'.");
                return BadRequest(ModelState);
            }

            var expense = Map.ToExpense(expenseDto);

            expense.User = userService.GetUserById(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!expenseService.Create(expense))
            {
                ModelState.AddModelError("CreatingError", "Something went wrong while creating.");
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

            if (!expenseService.Exists(expenseId))
            {
                return NotFound("Expense not found.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (categoryIds == null || categoryIds.Count() <= 0)
            {
                ModelState.AddModelError("MissingCategoryId", "No category id's found.");
                return BadRequest(ModelState);
            }

            foreach (var categoryId in categoryIds)
            {
                if (!categoryService.ExistsById(categoryId))
                {
                    return NotFound("Category not found.");
                }
            }

            var expense = expenseService.GetById(expenseId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            foreach (var categoryId in categoryIds)
            {
                var expenseCategory = new ExpenseCategory()
                {
                    CategoryId = categoryId,
                    Category = categoryService.GetById(categoryId),
                    ExpenseId = expenseId,
                    Expense = expense,
                };

                if (!expenseService.AddCategory(expenseCategory))
                {
                    ModelState.AddModelError("AddingError", "Something went wrong while adding category to expense.");
                    return StatusCode(500, ModelState);
                }
            }

            return Ok("Categories succesfully added to expense.");
        }

    }
}
