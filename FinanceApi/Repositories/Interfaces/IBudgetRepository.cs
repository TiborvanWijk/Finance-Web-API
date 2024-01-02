using FinanceApi.Models;

namespace FinanceApi.Repositories.Interfaces
{
    public interface IBudgetRepository
    {
        Budget GetById(int budgetId);
        ICollection<Budget> GetAllOfUser(string userId);
        bool ExistsById(string userId, int budgetId);
        bool Create(Budget budget);
        bool Update(Budget budget);
        bool Delete(Budget budget);
        bool save();
        bool ExistsByTitle(string userId, string title);
    }
}
