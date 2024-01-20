namespace FinanceApi.Services.Interfaces
{
    public interface IAuthorizeService
    {
        bool TrySendAuthRequest(string ownerId, string authorizedUserId, string title, string description, out int errorCode, out string errorMessage);
        bool TryDeleteAuthRequest(string ownerId, string authorizedUserId, out int errorCode, out string errorMessage);
        bool TryAuthorize(string ownerId, string authorizedUserId, out int errorCode, out string errorMessage);
        bool TryDeleteAuthorization(string ownerId, string authorizedUserId, out int errorCode, out string errorMessage);
    }
}
