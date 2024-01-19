using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;

namespace FinanceApi.Repositories
{
    public class AuthorizationRepository : IAuthorizationRepository
    {
        public void Add(AuthorizeUserRequest authorizationRequest)
        {
            throw new NotImplementedException();
        }

        public void Delete(AuthorizeUserRequest authorizationRequest)
        {
            throw new NotImplementedException();
        }

        public AuthorizeUserRequest GetById(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AuthorizeUserRequest> GetPendingRequestsForUser(int userId)
        {
            throw new NotImplementedException();
        }

        public void Update(AuthorizeUserRequest authorizationRequest)
        {
            throw new NotImplementedException();
        }
    }
}
