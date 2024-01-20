using FinanceApi.Controllers.ApiResponseHelpers;
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


        [HttpPost("create_authorize_invite/{authorizedUserId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult AuthorizeUser(string authorizedUserId, [FromQuery] string title, [FromQuery] string message)
        {

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            int errorCode;
            string errorMessage;

            if (!authorizeService.TrySendAuthRequest(userId, authorizedUserId, title, message, out errorCode, out errorMessage))
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


    }
}
