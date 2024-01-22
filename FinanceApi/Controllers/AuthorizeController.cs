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
    [Route("/api/[controller]")]
    [ApiController]
    public class AuthorizeController : Controller
    {
        private readonly IAuthorizeService authorizeService;

        public AuthorizeController(IAuthorizeService authorizeService)
        {
            this.authorizeService = authorizeService;
        }


        [HttpGet("get_all_authorization_invites")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public IActionResult GetAllAuthorizationInvites()
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            int errorCode;
            string errorMessage;

            ICollection<AuthorizeUserInvite> invites;

            if (!authorizeService.TryGetAllAuthorizationInvites(userId, out invites, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            var inviteDtos = invites.Select(Map.ToAuthorizeUserInviteDto).ToList();

            return Ok(inviteDtos);
        }


        [HttpPost("create_authorize_invite")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult AuthorizeUser([FromBody] AuthorizeUserInviteDto authorizeUserInviteDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            int errorCode;
            string errorMessage;

            if (!authorizeService.TrySendAuthRequest(userId, authorizeUserInviteDto, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Succesfully sent a authorization invite.");
        }


        [HttpPost("accept_authorize_invite/{ownerId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult AcceptAuthorization(string ownerId)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            int errorCode;
            string errorMessage;

            if (!authorizeService.TryAcceptAuthorizationInvite(ownerId, userId, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Succesfully authorized.");
        }


        [HttpPatch("edit_permission/{authorizedUserId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult GiveEditPermission(string authorizedUserId, [FromBody] bool canEdit)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            int errorCode;
            string errorMessage;

            if (!authorizeService.TryEditPermission(userId, authorizedUserId, canEdit, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Succesfully updated permission.");
        }



        [HttpDelete("delete_authorization/{authorizedUserId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult DeleteAuthorization(string authorizedUserId)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            int errorCode;
            string errorMessage;

            if(!authorizeService.TryDeleteAuthorization(currUser, authorizedUserId, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Authorization deleted succesfully.");
        }

        [HttpDelete("delete_authorization_invite/{userId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult DeleteAuthorizationInvite(string userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            int errorCode;
            string errorMessage;

            if (!authorizeService.TryDeleteAuthInvite(currUser, userId, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Authorization invite deleted succesfully.");
        }



        [HttpDelete("decline_authorization_invite/{userId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult DeclineAuthorizationInvite(string userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            int errorCode;
            string errorMessage;

            if (!authorizeService.TryDeleteAuthInvite(userId, currUser, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Authorization invite declined succesfully.");
        }
    }
}
