using FinanceApi.Data;
using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;

namespace FinanceApi.Repositories
{
    public class AuthorizeRepository : IAuthorizeRepository
    {
        private readonly DataContext dataContext;

        public AuthorizeRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public bool Authorize(AuthorizedUserJoin authorizedUser)
        {
            dataContext.AuthorizedUsers.Add(authorizedUser);
            return Save();
        }

        public ICollection<AuthorizedUserJoin> GetAuthorizedUsers(string userId)
        {
            return dataContext.AuthorizedUsers.Where(au => au.OwnerId.Equals(userId)).ToList();
        }

        public bool IsAuthorized(string ownerId, string authorizedUserId)
        {
            return dataContext.AuthorizedUsers.Any(au => au.OwnerId.Equals(ownerId) && au.AuthorizedUserId.Equals(authorizedUserId));
        }

        public bool Remove(string ownerId, string authorizedUserId)
        {
            dataContext.AuthorizedUsers.Remove(GetAuthorizedUsers(ownerId).First(au => au.AuthorizedUserId.Equals(authorizedUserId)));
            return Save();
        }

        public bool Save()
        {
            var saved = dataContext.SaveChanges();
            return saved > 0;
        }

        public bool update(AuthorizedUserJoin authorizedUserJoin)
        {
            dataContext.AuthorizedUsers.Update(authorizedUserJoin);
            return Save();
        }
    }
}
