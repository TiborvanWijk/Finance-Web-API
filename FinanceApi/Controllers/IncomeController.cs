﻿using FinanceApi.Controllers.ApiResponseHelpers;
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

        public IncomeController(IIncomeService incomeService, IUserService userService)
        {
            this.incomeService = incomeService;
            this.userService = userService;
        }

        [HttpGet("current/incomes")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetUsersIncomes()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            ICollection<IncomeDto> incomeDtos = incomeService.GetAllOfUser(userId).Select(Map.ToIncomeDto).ToList();

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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


        [HttpPost("Post")]
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


            if (incomeDto.IsPaid && !userService.UpdateBalance(user, incomeDto.Amount))
            {
                ModelState.AddModelError("UpdatingError", "Something went wrong while updating userbalance.");
            }

            return Ok("Income created succesfully.");
        }



        [HttpPost("AssociateCategories/{incomeId}")]
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
            decimal prevAmount;

            if (!incomeService.Update(user, incomeDto, out errorCode, out errorMessage, out prevAmount))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            decimal balance = incomeDto.IsPaid ? incomeDto.Amount - prevAmount : -prevAmount;

            if (!userService.UpdateBalance(user, balance))
            {
                return StatusCode(500, "Something went wrong with updating users balance.");
            }

            return Ok("Updated income succesfully.");
        }

        //[HttpDelete("RemoveCategories/{incomeId}")]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        //[ProducesResponseType(500)]
        //public IActionResult RemoveCategoriesFromIncome(int incomeId, [FromBody] ICollection<int> categoryIds)
        //{

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }


        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

        //    var user = userService.GetById(userId);

        //    int errorCode;
        //    string errorMessage;


        //    if (!incomeService.RemoveCategories(user, incomeId, categoryIds, out errorCode, out errorMessage))
        //    {

        //    }



        //}



    }
}
