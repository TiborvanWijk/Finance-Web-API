using FinanceApi.Controllers.ApiResponseHelpers;
using FinanceApi.Data.Dtos;
using FinanceApi.Mapper;
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
    public class CategoryController : Controller
    {
        private readonly ICategoryService categoryService;
        private readonly IUserService userService;

        public CategoryController(ICategoryService categoryService, IUserService userService)
        {
            this.categoryService = categoryService;
            this.userService = userService;
        }


        [HttpGet("Current")]
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


        [HttpPost("Post")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult CreateCategory([FromBody] CategoryDto categoryDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = userService.GetUserById(userId);

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

            var user = userService.GetUserById(userId);

            int errorCode;
            string errorMessage;

            if (!categoryService.Update(user, categoryDto, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Category updated succesfully.");
        }

    }
}
