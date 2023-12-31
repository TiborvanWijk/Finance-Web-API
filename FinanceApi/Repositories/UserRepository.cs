using FinanceApi.Data;
using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;

namespace FinanceApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext dataContext;

        public UserRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public bool Create(User user)
        {
            throw new NotImplementedException();
        }

        public bool Delete(User user)
        {
            throw new NotImplementedException();
        }

        public bool ExistsById(string userId)
        {
            return dataContext.Users.Any(u => u.Id.Equals(userId));
        }

        public bool ExistsByUsername(string username)
        {
            throw new NotImplementedException();
        }

        public ICollection<User> GetAll()
        {
            throw new NotImplementedException();
        }

        public User GetById(string userId)
        {
            return dataContext.Users.Where(u => u.Id.Equals(userId)).FirstOrDefault();
        }

        public User GetByUsername(string username)
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            throw new NotImplementedException();
        }

        public bool Update(User user)
        {
            throw new NotImplementedException();
        }
    }
}
