using FinanceApi.Data.Dtos;
using FinanceApi.Mapper;
using FinanceApi.Models;
using FinanceApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinanceApi.Controllers
{
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

        [Authorize]
        [HttpGet("incomes/current")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetUsersIncomes()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null || !userService.ExistsById(userId))
            {
                return NotFound();
            }

            ICollection<IncomeDto> incomeDtos = incomeService.GetAllByUserId(userId).Select(Map.ToIncomeDto).ToList();

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(incomeDtos);
        }


    }
}
