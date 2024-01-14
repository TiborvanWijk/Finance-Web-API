using FinanceApi.Models;

namespace FinanceApi.Services.Interfaces
{
    public interface IUserService
    {
        bool ExistsById(string userId);
        User GetById(string userId, bool tracking);
        User GetByUsername(string username);
        bool ExistsByUsername(string username);
        bool Create(User user);
        bool Update(User user);
        bool Delete(User user);
        bool TryGetUserBalance(string userId, out decimal balance, out int errorCode, out string errorMessage);
        bool TryUpdateUserCurrency(string userId, string currency, out int errorCode, out string errorMessage);
    }
}
