using FinanceApi.Models;

namespace FinanceApi.Services.Interfaces
{
    public interface IUserService
    {
        bool ExistsById(string userId);
        User GetUserById(string userId);
        User GetById(string userId);
        User GetByUsername(string username);
        bool UpdateBalance(User user, decimal amount);
        bool ExistsByUsername(string username);
        bool Create(User user);
        bool Update(User user);
        bool Delete(User user);
    }
}
