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

            if(!authorizeService.TryGetAllAuthorizationInvites(userId, out invites, out errorCode, out errorMessage))
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

            if(!ModelState.IsValid)
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
            
            if(!authorizeService.TryAcceptAuthorizationInvite(ownerId, userId, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Succesfully authorized.");        
        }


        [HttpPatch("give_edit_permission/{authorizedUserId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult GiveEditPermission(string authorizedUserId)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            int errorCode;
            string errorMessage;

            if (!authorizeService.TryGiveEditPermission(userId, authorizedUserId, out errorCode, out errorMessage))
            {
                return ApiResponseHelper.HandleErrorResponse(errorCode, errorMessage);
            }

            return Ok("Succesfully given edit permission.");
        }


    }
}
