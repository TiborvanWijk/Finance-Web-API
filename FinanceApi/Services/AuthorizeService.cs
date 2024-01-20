using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;
using FinanceApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FinanceApi.Services
{
    public class AuthorizeService : IAuthorizeService
    {
        private readonly IAuthorizeRepository authorizeRepository;
        private readonly IAuthorizationRequestRepository authorizationRequestRepository;
        private readonly IUserRepository userRepository;
        private readonly IRoleRepository roleRepository;

        public AuthorizeService(IAuthorizeRepository authorizeRepository, IAuthorizationRequestRepository authorizationRequestRepository,
            IUserRepository userRepository, IRoleRepository roleRepository)
        {
            this.authorizeRepository = authorizeRepository;
            this.authorizationRequestRepository = authorizationRequestRepository;
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
        }

        public IUserRepository UserRepository { get; }

        public bool TryAuthorize(string ownerId, string authorizedUserId, out int errorCode, out string errorMessage)
        {

            errorCode = 0;
            errorMessage = string.Empty;

            if(!userRepository.ExistsById(ownerId) || !userRepository.ExistsById(authorizedUserId))
            {
                errorCode = 404;
                errorMessage = "User not found.";
                return false;            
            }

            if(!authorizationRequestRepository.RequestExists(ownerId, authorizedUserId))
            {
                errorCode = 404;
                errorMessage = "User has not sent you a authorize request.";
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

            if(!authorizationRequestRepository.Delete(ownerId, authorizedUserId))
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

        public bool TrySendAuthRequest(string ownerId, string authorizedUserId, string title, string description, out int errorCode, out string errorMessage)
        {
            throw new NotImplementedException();
        }
    }
}
