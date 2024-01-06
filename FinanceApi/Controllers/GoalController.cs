using FinanceApi.Controllers.ApiResponseHelpers;
using FinanceApi.Data.Dtos;
using FinanceApi.Mapper;
using FinanceApi.Models;
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
    public class GoalController : Controller
    {
        private readonly IGoalService goalService;
        private readonly IUserService userService;

        public GoalController(IGoalService goalService, IUserService userService)
        {
            this.goalService = goalService;
            this.userService = userService;
        }


        [HttpGet("current")]
        [ProducesResponseType(200)]
        public IActionResult GetGoals()
        {

            var goals = goalService.GetAllOfUser(User.FindFirst(ClaimTypes.NameIdentifier).Value).Select(Map.ToGoalDto);

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(goals);
        }

        [HttpPost("Post")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult CreateGoal([FromBody] GoalManageDto goalDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            goalDto.Id = 0;

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = userService.GetUserById(userId);
            int errorCode;
            string errorMessage;

            if (!goalService.Create(user, goalDto, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Goal created succesfully.");
        }

        [HttpPost("AssociateCategories/{goalId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult AddCategories(int goalId, [FromBody] ICollection<int> categoryIds)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string errorMessage;
            int errorCode;
            if (!goalService.AddCategories(User.FindFirst(ClaimTypes.NameIdentifier).Value, goalId, categoryIds, out errorMessage, out errorCode))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Categories successfully added to goal.");
        }


        [HttpPut("put")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateGoal([FromBody] GoalManageDto goalManageDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = userService.GetById(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            int errorCode = 0;
            string errorMessage = string.Empty;


            if(!goalService.Update(user, goalManageDto, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Goal updated succesfully.");
        }
    }
}
