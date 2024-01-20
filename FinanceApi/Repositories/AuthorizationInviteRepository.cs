using FinanceApi.Data;
using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;

namespace FinanceApi.Repositories
{
    public class AuthorizationInviteRepository : IAuthorizationInviteRepository
    {
        private readonly DataContext dataContext;

        public AuthorizationInviteRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public bool Create(AuthorizeUserInvite authorizationRequest)
        {
            dataContext.AuthorizeUserInvite.Add(authorizationRequest);
            return Save();
        }

        public bool Delete(string ownerId, string authorizedUserId)
        {
            dataContext.AuthorizeUserInvite.Remove(GetAllSentRequest(ownerId).First(ar => ar.AuthorizedUserId.Equals(authorizedUserId)));
            return Save();
        }

        public ICollection<AuthorizeUserInvite> GetAllSentRequest(string ownerId)
        {
            return dataContext.AuthorizeUserInvite.Where(ar => ar.OwnerId.Equals(ownerId)).ToList();
        }

        public ICollection<AuthorizeUserInvite> GetPendingRequestsForUser(string userId)
        {
            return dataContext.AuthorizeUserInvite.Where(ar => ar.AuthorizedUserId.Equals(userId)).ToList();
        }

        public bool RequestExists(string ownerId, string authorizedUserId)
        {
            return dataContext.AuthorizeUserInvite.Any(ar => ar.OwnerId.Equals(ownerId) && ar.AuthorizedUserId.Equals(authorizedUserId));
        }

        public bool Save()
        {
            var saved = dataContext.SaveChanges();
            return saved > 0;
        }
    }
}
