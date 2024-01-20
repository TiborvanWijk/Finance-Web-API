using FinanceApi.Data.Dtos;
using FinanceApi.Models;

namespace FinanceApi.Services.Interfaces
{
    public interface IAuthorizeService
    {
        bool TrySendAuthRequest(string ownerId, string authorizedUserId, AuthorizeUserInviteDto authorizeUserInviteDto, out int errorCode, out string errorMessage);
        bool TryDeleteAuthRequest(string ownerId, string authorizedUserId, out int errorCode, out string errorMessage);
        bool TryAcceptAuthorizationInvite(string ownerId, string authorizedUserId, out int errorCode, out string errorMessage);
        bool TryDeleteAuthorization(string ownerId, string authorizedUserId, out int errorCode, out string errorMessage);
        bool TryGetAllAuthorizationInvites(string userId, out ICollection<AuthorizeUserInvite> invites, out int errorCode, out string errorMessage);
    }
}
