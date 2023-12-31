using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;
using FinanceApi.Services.Interfaces;

namespace FinanceApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
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
            return userRepository.ExistsById(userId);
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
            throw new NotImplementedException();
        }

        public User GetByUsername(string username)
        {
            throw new NotImplementedException();
        }

        public User GetUserById(string userId)
        {
            return userRepository.GetById(userId);
        }

        public bool Update(User user)
        {
            throw new NotImplementedException();
        }
    }
}
