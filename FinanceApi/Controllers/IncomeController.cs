﻿using FinanceApi.Controllers.ApiResponseHelpers;
using FinanceApi.Data.Dtos;
using FinanceApi.Mapper;
using FinanceApi.Models;
using FinanceApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
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
        [OutputCache]
        public IActionResult GetIncomes(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] string? listOrderBy,
            [FromQuery] string? listDir,
            [FromQuery] string? optionalOwnerId,
            [FromQuery] int? categoryId
            )
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            int errorCode;
            string errorMessage;

            if(!authorizeService.ValidateUsers(HttpContext, currUserId, optionalOwnerId, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            var userLookupId = optionalOwnerId == null ? currUserId : optionalOwnerId;
           
            ICollection<Income> incomes;

            if(!incomeService.TryGetIncomesFilteredOrDefault(userLookupId, out incomes, from, to, listOrderBy, listDir, categoryId, out errorCode, out errorMessage)){
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            var incomeDtos = incomes.Select(Map.ToIncomeDto).ToList();

            return Ok(incomeDtos);
        }

        [HttpPost("post")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult CreateIncome([FromBody] IncomeDto incomeDto, [FromQuery] string? optionalOwnerId)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            incomeDto.Id = 0;

            var currUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;


            int errorCode;
            string errorMessage;

            if (!authorizeService.ValidateUsers(HttpContext, currUserId, optionalOwnerId, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            var userLookupId = optionalOwnerId == null ? currUserId : optionalOwnerId;


            var user = userService.GetById(userLookupId, true);


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
        public IActionResult AddCategoryToIncome(int incomeId, [FromBody] ICollection<int> categoryIds, [FromQuery] string? optionalOwnerId)
        {

            var currUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            int errorCode;
            string errorMessage;

            if (!authorizeService.ValidateUsers(HttpContext, currUserId, optionalOwnerId, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            var userLookupId = optionalOwnerId == null ? currUserId : optionalOwnerId;


            if (!incomeService.AddCategories(userLookupId, incomeId, categoryIds, out errorMessage, out errorCode))
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
        public IActionResult UpdateIncome(IncomeDto incomeDto, [FromQuery] string? optionalOwnerId)
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
        public IActionResult DeleteIncome(int incomeId, [FromQuery] string? optionalOwnerId)
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

            if (!incomeService.TryDeleteIncome(user, incomeId, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return NoContent();
        }



        [HttpDelete("remove_category/{incomeId}/{categoryId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult RemoveCategory(int incomeId, int categoryId, [FromQuery] string? optionalOwnerId)
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


            if (!incomeService.TryRemoveCategories(user, incomeId, categoryId, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return NoContent();
        }



    }
}
