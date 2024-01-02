using FinanceApi.Models;

namespace FinanceApi.Services.Interfaces
{
    public interface IBudgetService
    {
        Budget GetById(int budgetId);
        ICollection<Budget> GetAllOfUser(string userId);
        bool ExistsById(string userId, int budgetId);
        bool Create(Budget budget);
        bool Update(Budget budget);
        bool Delete(Budget budget);
        bool ExistsByTitle(string userId, string title);
    }
}
