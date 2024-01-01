﻿using FinanceApi.Data.Dtos;
using FinanceApi.Enums;
using FinanceApi.Mapper;
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

        public ExpenseController(IExpenseService expenseService, IUserService userService)
        {
            this.expenseService = expenseService;
            this.userService = userService;
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


    }
}
