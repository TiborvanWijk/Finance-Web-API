using FinanceApi.Data.Dtos;
using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;
using FinanceApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FinanceApi.Services
{
    public class AuthorizeService : IAuthorizeService
    {
        private readonly IAuthorizeRepository authorizeRepository;
        private readonly IAuthorizationInviteRepository authorizationInviteRepository;
        private readonly IUserRepository userRepository;

        public AuthorizeService(IAuthorizeRepository authorizeRepository, IAuthorizationInviteRepository authorizationInviteRepository,
            IUserRepository userRepository)
        {
            this.authorizeRepository = authorizeRepository;
            this.authorizationInviteRepository = authorizationInviteRepository;
            this.userRepository = userRepository;
        }

        public bool TryAcceptAuthorizationInvite(string ownerId, string authorizedUserId, out int errorCode, out string errorMessage)
        {

            errorCode = 0;
            errorMessage = string.Empty;

            if(!userRepository.ExistsById(ownerId) || !userRepository.ExistsById(authorizedUserId))
            {
                errorCode = 404;
                errorMessage = "User not found.";
                return false;            
            }

            if(!authorizationInviteRepository.RequestExists(ownerId, authorizedUserId))
            {
                errorCode = 404;
                errorMessage = "User has not sent you a authorize invite.";
                return false;
            }


            var authorizedUser = new AuthorizedUserJoin()
            {
                OwnerId = ownerId,
                Owner = userRepository.GetById(ownerId, true),
                AuthorizedUserId = authorizedUserId,
                AuthorizedUser = userRepository.GetById(authorizedUserId, true),
                CanEdit = false,
            };

            if (!authorizeRepository.Authorize(authorizedUser))
            {
                errorCode = 500;
                errorMessage = "Something went wrong while saving.";
                return false;
            }

            if(!authorizationInviteRepository.Delete(ownerId, authorizedUserId))
            {
                errorCode = 500;
                errorMessage = "Something went wrong while deleting authorization request.";
                return false;
            }

            return true;
        }

        public bool TryDeleteAuthorization(string ownerId, string authorizedUserId, out int errorCode, out string errorMessage)
        {

            errorCode = 0;
            errorMessage = string.Empty;

            if (!userRepository.ExistsById(ownerId) || !userRepository.ExistsById(authorizedUserId))
            {
                errorCode = 404;
                errorMessage = "User not found.";
                return false;
            }

            if (!authorizeRepository.IsAuthorized(ownerId, authorizedUserId))
            {
                errorCode = 400;
                errorMessage = "User is not authorized";
                return false;
            }

            if(!authorizeRepository.Remove(ownerId, authorizedUserId))
            {
                errorCode = 500;
                errorMessage = "Something went wrong while deleting authorization.";
                return false;
            }

            return true;
        }

        public bool TryDeleteAuthInvite(string ownerId, string authorizedUserId, out int errorCode, out string errorMessage)
        {

            errorCode = 0;
            errorMessage = string.Empty;

            if (!userRepository.ExistsById(ownerId) || !userRepository.ExistsById(authorizedUserId))
            {
                errorCode = 404;
                errorMessage = "User not found.";
                return false;
            }

            if (!authorizationInviteRepository.RequestExists(ownerId, authorizedUserId))
            {
                errorCode = 404;
                errorMessage = "Invite not found.";
                return false;
            }


            if(!authorizationInviteRepository.Delete(ownerId, authorizedUserId))
            {
                errorCode = 500;
                errorMessage = "Something went wrong while deleting invite.";
                return false;
            }

            return true;
        }

        public bool TryGetAllAuthorizationInvites(string userId, out ICollection<AuthorizeUserInvite> invites, out int errorCode, out string errorMessage)
        {

            errorCode = 0;
            errorMessage = string.Empty;
            invites = new List<AuthorizeUserInvite>();

            if(!userRepository.ExistsById(userId))
            {
                errorCode = 401;
                errorMessage = "Unauthorized.";
                return false;
            }

            invites = authorizationInviteRepository.GetPendingRequestsForUser(userId);

            return true;
        }

        public bool TryEditPermission(string userId, string authorizedUserId, bool canEdit, out int errorCode, out string errorMessage)
        {

            errorCode = 0;
            errorMessage = string.Empty;


            if(!userRepository.ExistsById(authorizedUserId))
            {
                errorCode = 404;
                errorMessage = "User not found.";
                return false;
            }

            if(!authorizeRepository.IsAuthorized(userId, authorizedUserId))
            {
                errorCode = 400;
                errorMessage = "User is not authorized.";
                return false;
            }

            var authorizedUserJoin = authorizeRepository.GetAuthorizedUsers(userId).First(au => au.AuthorizedUserId.Equals(authorizedUserId));

            authorizedUserJoin.CanEdit = canEdit;

            if (!authorizeRepository.update(authorizedUserJoin))
            {
                errorCode = 500;
                errorMessage = "Something went wrong while updating users privilege";
                return false;
            }

            return true;
        }

        public bool TrySendAuthRequest(string ownerId, AuthorizeUserInviteDto authorizeUserInviteDto, out int errorCode, out string errorMessage)
        {

            errorCode = 0;
            errorMessage = string.Empty;

            if (ownerId.Equals(authorizeUserInviteDto.UserId))
            {
                errorCode = 400;
                errorMessage = "User id is the same as current user's id.";
                return false;
            }

            if(authorizeUserInviteDto.Title.Length > 40)
            {
                errorCode = 400;
                errorMessage = "Title too long must be in range of 0 and 40";
                return false;
            }

            if (authorizeUserInviteDto.Message.Length > 250)
            {
                errorCode = 400;
                errorMessage = "Message too long must be in range of 0 and 250";
                return false;
            }


            if (!userRepository.ExistsById(ownerId) || !userRepository.ExistsById(authorizeUserInviteDto.UserId))
            {
                errorCode = 404;
                errorMessage = "User not found.";
                return false;
            }

            if(authorizeRepository.IsAuthorized(ownerId, authorizeUserInviteDto.UserId))
            {
                errorCode = 400;
                errorMessage = "User is already authorized.";
                return false;
            }

            if (authorizationInviteRepository.RequestExists(ownerId, authorizeUserInviteDto.UserId))
            {
                errorCode = 400;
                errorMessage = "Authorization invite already sent.";
                return false;
            }

            var authorizeInvite = new AuthorizeUserInvite()
            {
                Owner = userRepository.GetById(ownerId, true),
                OwnerId = ownerId,
                AuthorizedUser = userRepository.GetById(authorizeUserInviteDto.UserId, true),
                AuthorizedUserId = authorizeUserInviteDto.UserId,
                Title = authorizeUserInviteDto.Title,
                Message = authorizeUserInviteDto.Message,
            };

            if (!authorizationInviteRepository.Create(authorizeInvite))
            {
                errorCode = 500;
                errorMessage = "Something went wrong while creating authorization invite.";
                return false;
            }
            
            return true;
        }

        public bool ValidateUsers(HttpContext httpContext, string currUserId, string? optionalOwnerId, out int errorCode, out string errorMessage)
        {
            errorCode = 0;
            errorMessage = string.Empty;

            if (!userRepository.ExistsById(currUserId))
            {
                errorCode = 401;
                errorMessage = "Unauthorized.";
                return false;
            }
            else if(optionalOwnerId == null)
            {
                return true;
            }

            if (!userRepository.ExistsById(optionalOwnerId))
            {
                errorCode = 404;
                errorMessage = "User not found.";
                return false;
            }

            if (!authorizeRepository.IsAuthorized(optionalOwnerId, currUserId))
            {
                errorCode = 403;
                errorMessage = "Forbidden.";
                return false;
            }

            if(httpContext.Request.Method != HttpMethods.Get)
            {
                var authConnection = authorizeRepository.GetAuthorizedUsers(optionalOwnerId).First(au => au.AuthorizedUserId.Equals(currUserId));
                if (!authConnection.CanEdit)
                {
                    errorCode = 403;
                    errorMessage = "Forbidden.";
                    return false;
                }
            }

            return true;
        }

        public bool TryGetAllAuthorizedUsers(string userId, out ICollection<User> authorizedUsers, out int errorCode, out string errorMessage)
        {

            errorCode = 0;
            errorMessage = string.Empty;
            authorizedUsers = new List<User>();

            if(!userRepository.ExistsById(userId))
            {
                errorCode = 401;
                errorMessage = "Unauthorized";
                return false;
            }

            try
            {
                authorizedUsers = authorizeRepository.GetAuthorizedUsers(userId).Select(au => au.AuthorizedUser).ToList();
                return true;
            }
            catch (Exception ex)
            {
                errorCode = 500;
                errorMessage = $"Something went wrong while fetching authorized users: {ex}";
                return false;
            }

        }

        public bool tryGetAllOutGoingInvites(string userId, out ICollection<AuthorizeUserInvite> invites, out int errorCode, out string errorMessage)
        {

            errorCode = 0;
            errorMessage = string.Empty;
            invites = new List<AuthorizeUserInvite>();

            if (!userRepository.ExistsById(userId))
            {
                errorCode = 401;
                errorMessage = "Unauthorized.";
                return false;
            }

            invites = authorizationInviteRepository.GetAllSentRequest(userId);
            return true;
        }
    }
}
