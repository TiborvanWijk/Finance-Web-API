namespace FinanceApi.Services.Interfaces
{
    public interface IAuthorizeService
    {
        bool TrySendAuthRequest(string ownerId, string authorizedUserId, string title, string message, out int errorCode, out string errorMessage);
        bool TryDeleteAuthRequest(string ownerId, string authorizedUserId, out int errorCode, out string errorMessage);
        bool TryAcceptAuthorizationInvite(string ownerId, string authorizedUserId, out int errorCode, out string errorMessage);
        bool TryDeleteAuthorization(string ownerId, string authorizedUserId, out int errorCode, out string errorMessage);
    }
}
