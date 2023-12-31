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

        public bool ExistsById(string userId)
        {
            return userRepository.ExistsById(userId);
        }

        public User GetUserById(string userId)
        {
            return userRepository.GetById(userId);
        }
    }
}
