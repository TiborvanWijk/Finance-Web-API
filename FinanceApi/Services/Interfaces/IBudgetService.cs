using FinanceApi.Data.Dtos;
using FinanceApi.Models;

namespace FinanceApi.Services.Interfaces
{
    public interface IBudgetService
    {
        Budget GetById(int budgetId);
        ICollection<Budget> GetAllOfUser(string userId);
        bool ExistsById(string userId, int budgetId);
        bool Create(User user, BudgetDto budgetDto, out int errorCode, out string errorMessage);
        bool Update(Budget budget);
        bool Delete(Budget budget);
        bool ExistsByTitle(string userId, string title);
    }
}
