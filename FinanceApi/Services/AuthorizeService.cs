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
        private readonly IRoleRepository roleRepository;

        public AuthorizeService(IAuthorizeRepository authorizeRepository, IAuthorizationInviteRepository authorizationInviteRepository,
            IUserRepository userRepository, IRoleRepository roleRepository)
        {
            this.authorizeRepository = authorizeRepository;
            this.authorizationInviteRepository = authorizationInviteRepository;
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
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

            ICollection<IdentityRole> roles = new List<IdentityRole>();
            var role = roleRepository.GetByName("View", true);
            roles.Add(role);

            var authorizedUser = new AuthorizedUserJoin()
            {
                OwnerId = ownerId,
                Owner = userRepository.GetById(ownerId, true),
                AuthorizedUserId = authorizedUserId,
                AuthorizedUser = userRepository.GetById(authorizedUserId, true),
                Roles = roles, 
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
            throw new NotImplementedException();
        }

        public bool TryDeleteAuthRequest(string ownerId, string authorizedUserId, out int errorCode, out string errorMessage)
        {
            throw new NotImplementedException();
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

        public bool TrySendAuthRequest(string ownerId, string authorizedUserId, AuthorizeUserInviteDto authorizeUserInviteDto, out int errorCode, out string errorMessage)
        {

            errorCode = 0;
            errorMessage = string.Empty;


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


            if (!userRepository.ExistsById(ownerId) || !userRepository.ExistsById(authorizedUserId))
            {
                errorCode = 404;
                errorMessage = "User not found.";
                return false;
            }

            if (authorizationInviteRepository.RequestExists(ownerId, authorizedUserId))
            {
                errorCode = 400;
                errorMessage = "Authorization invite already sent.";
                return false;
            }

            


            var authorizeInvite = new AuthorizeUserInvite()
            {
                Owner = userRepository.GetById(ownerId, true),
                OwnerId = ownerId,
                AuthorizedUser = userRepository.GetById(authorizedUserId, true),
                AuthorizedUserId = authorizedUserId,
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
    }
}
