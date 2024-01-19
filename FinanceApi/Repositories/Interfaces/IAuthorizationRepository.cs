using FinanceApi.Models;

namespace FinanceApi.Repositories.Interfaces
{
    public interface IAuthorizationRepository
    {
        AuthorizeUserRequest GetById(int id);
        IEnumerable<AuthorizeUserRequest> GetPendingRequestsForUser(int userId);
        void Add(AuthorizeUserRequest authorizationRequest);
        void Update(AuthorizeUserRequest authorizationRequest);
        void Delete(AuthorizeUserRequest authorizationRequest);
    }
}
