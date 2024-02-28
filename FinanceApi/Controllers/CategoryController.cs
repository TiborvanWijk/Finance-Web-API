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
        private readonly IAuthorizeService authorizeService;

        public CategoryController(ICategoryService categoryService, IUserService userService, IAuthorizeService authorizeService)
        {
            this.categoryService = categoryService;
            this.userService = userService;
            this.authorizeService = authorizeService;
        }


        [HttpGet("current")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult GetAllCategories([FromQuery] string? listOrderBy, [FromQuery] string? listDir, [FromQuery] string? optionalOwnerId)
        {

            var currUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            int errorCode;
            string errorMessage;

            if (!authorizeService.ValidateUsers(HttpContext, currUserId, optionalOwnerId, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            var userLookupId = optionalOwnerId == null ? currUserId : optionalOwnerId; 
            
            ICollection<Category> categories;

            if (!categoryService.TryGetCategoriesFilteredOrDefault(userLookupId, out categories, out errorCode, out errorMessage, listOrderBy, listDir))
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
        public IActionResult GetCategoryExpenseAmount(int categoryId, [FromQuery] string? optionalOwnerId)
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
        public IActionResult CreateCategory([FromBody] CategoryDto categoryDto, [FromQuery] string? optionalOwnerId)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            categoryDto.Id = 0;
            var currUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;


            int errorCode;
            string errorMessage;

            if (!authorizeService.ValidateUsers(HttpContext, currUserId, optionalOwnerId, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            var userLookupId = optionalOwnerId == null ? currUserId : optionalOwnerId;

            var user = userService.GetById(userLookupId, true);

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
        public IActionResult UpdateCategory([FromBody] CategoryDto categoryDto, [FromQuery] string? optionalOwnerId)
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
        public IActionResult DeleteCategory(int categoryId, [FromQuery] string? optionalOwnerId)
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

            if (!categoryService.TryDelete(user, categoryId, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return NoContent();
        }


    }
}
