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

            var categoryValidationCode = categoryService.ValidateCategoryUpdate(User.FindFirst(ClaimTypes.NameIdentifier).Value, categoryDto);

            switch (categoryValidationCode)
            {
                case 404:
                    return NotFound();
                case 400:
                    ModelState.AddModelError("NotUnique", "Category with this title already exists.");
                    return BadRequest(ModelState);
                default:
                    break;
            }
            var category = Map.ToCategory(categoryDto);

            if (!categoryService.Update(category))
            {
                ModelState.AddModelError("UpdatingError", "Something went wrong while updating.");
                return StatusCode(500, ModelState);
            }

            return Ok("Category updated succesfully.");
        }

    }
}
