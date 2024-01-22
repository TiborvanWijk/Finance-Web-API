using FinanceApi.Data.Dtos;
using FinanceApi.Models;

namespace FinanceApi.Services.Interfaces
{
    public interface IAuthorizeService
    {
        bool IsAuthorized(string ownerId, string authorizedUserId);
        bool TrySendAuthRequest(string ownerId, AuthorizeUserInviteDto authorizeUserInviteDto, out int errorCode, out string errorMessage);
        bool TryDeleteAuthInvite(string ownerId, string authorizedUserId, out int errorCode, out string errorMessage);
        bool TryAcceptAuthorizationInvite(string ownerId, string authorizedUserId, out int errorCode, out string errorMessage);
        bool TryDeleteAuthorization(string ownerId, string authorizedUserId, out int errorCode, out string errorMessage);
        bool TryGetAllAuthorizationInvites(string userId, out ICollection<AuthorizeUserInvite> invites, out int errorCode, out string errorMessage);
        bool TryEditPermission(string userId, string authorizedUserId, bool canEdit, out int errorCode, out string errorMessage);
    }
}
