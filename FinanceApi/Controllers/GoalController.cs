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
    public class GoalController : Controller
    {
        private readonly IGoalService goalService;
        private readonly IUserService userService;
        private readonly IAuthorizeService authorizeService;

        public GoalController(IGoalService goalService, IUserService userService, IAuthorizeService authorizeService)
        {
            this.goalService = goalService;
            this.userService = userService;
            this.authorizeService = authorizeService;
        }


        [HttpGet("current")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult GetGoals(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? listOrderBy,
            [FromQuery] string? listDir,
            [FromQuery] string? optionalOwnerId)
        {

            if(!ModelState.IsValid)
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
            
            ICollection<Goal> goals;
            if(!goalService.TryGetAllOrderedOrDefault(userLookupId, out goals, out errorCode, out errorMessage, startDate, endDate, listOrderBy, listDir))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            var goalDtos = goals.Select(Map.ToGoalDto).ToList();

            for(int i = 0; i < goalDtos.Count(); ++i)
            {
                goalDtos[i].Progress = goalService.GetProgressAmountOfGoal(userLookupId, goalDtos[i].Id); 
            }

            return Ok(goalDtos);
        }

        [HttpGet("current/goals/{categoryId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult GetGoalsByCategoryId(int categoryId, [FromQuery] string? optionalOwnerId)
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

            ICollection<Goal> goals;

            if(!goalService.TryGetGoalsByCategoryId(user, categoryId, out goals, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }


            var goalDtos = goals.Select(Map.ToGoalDto);

            return Ok(goalDtos);
        }
        


        [HttpPost("post")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult CreateGoal([FromBody] GoalManageDto goalDto, [FromQuery] string? optionalOwnerId)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            goalDto.Id = 0;

            var currUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            int errorCode;
            string errorMessage;

            if (!authorizeService.ValidateUsers(HttpContext, currUserId, optionalOwnerId, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            var userLookupId = optionalOwnerId == null ? currUserId : optionalOwnerId;
            var user = userService.GetById(userLookupId, true);

            if (!goalService.Create(user, goalDto, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Goal created succesfully.");
        }

        [HttpPost("associate_categories/{goalId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult AddCategories(int goalId, [FromBody] ICollection<int> categoryIds, [FromQuery] string? optionalOwnerId)
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

            if (!goalService.AddCategories(userLookupId, goalId, categoryIds, out errorMessage, out errorCode))
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
        public IActionResult UpdateGoal([FromBody] GoalManageDto goalManageDto, [FromQuery] string? optionalOwnerId)
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

            if (!goalService.Update(user, goalManageDto, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Goal updated succesfully.");
        }



        [HttpDelete("delete/{goalId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult DeleteGoal(int goalId, [FromQuery] string? optionalOwnerId)
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

            if (!goalService.TryDeleteGoal(user, goalId, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Goal deleted succesfully.");
        }


        [HttpDelete("remove_category/{goalId}/{categoryId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult RemoveCategories(int goalId, int categoryId, [FromQuery] string? optionalOwnerId)
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

            if (!goalService.TryRemoveCategory(user, goalId, categoryId, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Categories removed succesfully.");
        }

    }
}
