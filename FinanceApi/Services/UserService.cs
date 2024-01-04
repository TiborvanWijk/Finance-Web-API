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

        public bool UpdateBalance(User user, decimal amount)
        {
            user.Balance += amount;
            return userRepository.Update(user);
        }

        public bool Create(User user)
        {
            return userRepository.Create(user);
        }

        public bool Delete(User user)
        {
            return userRepository.Delete(user);
        }

        public bool ExistsById(string userId)
        {
            return userRepository.ExistsById(userId);
        }

        public bool ExistsByUsername(string username)
        {
            return userRepository.ExistsByUsername(username);
        }

        public User GetById(string userId)
        {
            return userRepository.GetById(userId);
        }

        public User GetByUsername(string username)
        {
            return userRepository.GetByUsername(username);
        }

        public User GetUserById(string userId)
        {
            return userRepository.GetById(userId);
        }

        public bool Update(User user)
        {
            return userRepository.Update(user);
        }
    }
}
