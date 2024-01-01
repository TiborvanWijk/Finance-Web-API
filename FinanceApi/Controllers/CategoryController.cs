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

            if (categoryDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (categoryService.ExistsBytitle(categoryDto.Title))
            {
                ModelState.AddModelError("Duplicate", "Category with this title already exists.");
                return BadRequest(ModelState);
            }

            var category = Map.ToCategory(categoryDto);
            category.User = userService.GetById(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!categoryService.Create(category))
            {
                ModelState.AddModelError("CreationError", "Something went wrong while creating category.");
                return StatusCode(500, ModelState);
            }

            return Ok("Category created succesfully.");
        }


    }
}
