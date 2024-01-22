using FinanceApi.Controllers.ApiResponseHelpers;
using FinanceApi.Data.Dtos;
using FinanceApi.Mapper;
using FinanceApi.Models;
using FinanceApi.Services;
using FinanceApi.Services.Interfaces;
using FinanceApi.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Claims;

namespace FinanceApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class IncomeController : Controller
    {
        private readonly IIncomeService incomeService;
        private readonly IUserService userService;
        private readonly IAuthorizeService authorizeService;

        public IncomeController(IIncomeService incomeService, IUserService userService, IAuthorizeService authorizeService)
        {
            this.incomeService = incomeService;
            this.userService = userService;
            this.authorizeService = authorizeService;
        }

        [HttpGet("current")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetUsersIncomes(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] string? list_order_by,
            [FromQuery] string? list_dir,
            [FromQuery] string? optionalOwnerId)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            int errorCode;
            string errorMessage;
            string userLookupId;

            if(!authorizeService.TryGetUserLookupIdWithValidation(HttpContext, currUserId, optionalOwnerId, out userLookupId, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }      

           
            ICollection<Income> incomes;

            if(!incomeService.TryGetIncomesFilteredOrDefault(userLookupId, out incomes, from, to, list_order_by, list_dir, out errorCode, out errorMessage)){
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            var incomeDtos = incomes.Select(Map.ToIncomeDto).ToList();

            return Ok(incomeDtos);
        }

        [HttpGet("current/incomes/{categoryId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetUsersIncomesByCategoryId(int categoryId)
        {

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = userService.GetById(userId, true);


            int errorCode;
            string errorMessage;
            ICollection<Income> incomes;

            if(!incomeService.tryGetIncomesWithCategoryId(user, categoryId, out incomes, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            var incomeDtos = incomes.Select(Map.ToIncomeDto).ToList();

            return Ok(incomeDtos);
        }


        [HttpPost("post")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult CreateIncome([FromBody] IncomeDto incomeDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            incomeDto.Id = 0;

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = userService.GetById(userId, true);

            int errorCode;
            string errorMessage;

            if (!incomeService.Create(user, incomeDto, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Income created succesfully.");
        }



        [HttpPost("associate_categories/{incomeId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult AddCategoryToIncome(int incomeId, [FromBody] ICollection<int> categoryIds)
        {
            string errorMessage;
            int errorCode;
            if (!incomeService.AddCategories(User.FindFirst(ClaimTypes.NameIdentifier).Value, incomeId, categoryIds, out errorMessage, out errorCode))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Categories successfully added to income.");
        }

        [HttpPut("put")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateIncome(IncomeDto incomeDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = userService.GetById(userId, true);

            int errorCode;
            string errorMessage;

            if (!incomeService.Update(user, incomeDto, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Updated income succesfully.");
        }

        [HttpDelete("delete/{incomeId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult DeleteIncome(int incomeId)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = userService.GetById(userId, true);

            int errorCode;
            string errorMessage;

            if(!incomeService.TryDeleteIncome(user, incomeId, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Income deleted succesfully.");
        }



        [HttpDelete("remove_categories/{incomeId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult RemoveCategoriesFromIncome(int incomeId, [FromBody] ICollection<int> categoryIds)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = userService.GetById(userId, true);

            int errorCode;
            string errorMessage;

            
            if (!incomeService.TryRemoveCategories(user, incomeId, categoryIds, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Categories deleted succesfully.");
        }



    }
}
