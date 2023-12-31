using FinanceApi.Models;

namespace FinanceApi.Services.Interfaces
{
    public interface IUserService
    {
        bool ExistsById(string userId);
        User GetUserById(string userId);
    }
}
