using FinanceApi.Models;

namespace FinanceApi.Repositories.Interfaces
{
    public interface IAuthorizeRepository
    {
        ICollection<AuthorizedUserJoin> GetAuthorizedUsers(string userId);
        bool IsAuthorized(string ownerId, string authorizedUserId);
        bool Authorize(AuthorizedUserJoin authorizedUser);
        bool Remove(string ownerId, string authorizedUserId);
        bool Save();
    }
}
