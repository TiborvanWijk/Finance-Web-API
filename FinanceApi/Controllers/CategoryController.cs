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
    public class CategoryController : Controller
    {
        private readonly ICategoryService categoryService;
        private readonly IUserService userService;

        public CategoryController(ICategoryService categoryService, IUserService userService)
        {
            this.categoryService = categoryService;
            this.userService = userService;
        }


        [HttpGet("current")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult GetAllCategories()
        {

            var categoryDtos = categoryService.GetAllOfUser(User.FindFirst(ClaimTypes.NameIdentifier).Value).Select(Map.ToCategoryDto);

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(categoryDtos);
        }

        [HttpGet("current_by_expenseAmount")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult GetCategoriesSortedByExpenseAmount()
        {


            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = userService.GetById(userId, true);

            int errorCode;
            string errorMessage;
            ICollection<Category> categories;

            if(!categoryService.TryGetCategoriesSortedByExpenseAmount(user, out categories, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            var categoryDtos = categories.Select(Map.ToCategoryDto);
            return Ok(categoryDtos);
        }

        [HttpGet("category_expense_amount/{categoryId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult GetCategoryExpenseAmount(int categoryId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = userService.GetById(userId, true);

            int errorCode;
            string errorMessage;
            decimal expenseAmount;

            if (!categoryService.TryGetCategoryExpenseAmount(user, categoryId, out expenseAmount, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok(expenseAmount);
        }



        [HttpPost("post")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult CreateCategory([FromBody] CategoryDto categoryDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            categoryDto.Id = 0;

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = userService.GetById(userId, true);

            int errorCode;
            string errorMessage;

            if (!categoryService.Create(user, categoryDto, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Category created succesfully.");
        }

        [HttpPut("put")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateCategory([FromBody] CategoryDto categoryDto)
        { 
        
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = userService.GetById(userId, true);

            int errorCode;
            string errorMessage;

            if (!categoryService.Update(user, categoryDto, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Category updated succesfully.");
        }



        [HttpDelete("delete/{categoryId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult DeleteCategory(int categoryId)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = userService.GetById(userId, true);

            int errorCode;
            string errorMessage;

            if(!categoryService.TryDelete(user, categoryId, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Category deleted succesfully.");
        }


    }
}
