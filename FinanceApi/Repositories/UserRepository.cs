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
            dataContext.Users.Add(user);
            return Save();
        }

        public bool Delete(User user)
        {
            dataContext.Users.Remove(user);
            return Save();
        }

        public bool ExistsById(string userId)
        {
            return dataContext.Users.Any(u => u.Id.Equals(userId));
        }

        public bool ExistsByUsername(string username)
        {
            return dataContext.Users.Any(u => u.UserName.Equals(username));
        }

        public User GetById(string userId)
        {
            return dataContext.Users.Where(u => u.Id.Equals(userId)).FirstOrDefault();
        }

        public User GetByUsername(string username)
        {
            return dataContext.Users.FirstOrDefault(u => u.UserName.Equals(username));
        }

        public bool Save()
        {
            var saved = dataContext.SaveChanges();
            return saved > 0;
        }

        public bool Update(User user)
        {
            dataContext.Users.Update(user);
            return Save();
        }
    }
}
