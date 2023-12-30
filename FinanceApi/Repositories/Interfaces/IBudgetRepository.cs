using FinanceApi.Models;

namespace FinanceApi.Repositories.Interfaces
{
    public interface IBudgetRepository
    {
        Budget GetById(int budgetId);
        ICollection<Budget> GetAllOfUser(int userId);
        bool Exists(int budgetId);
        bool Create(Budget budget);
        bool Update(Budget budget);
        bool Delete(Budget budget);
    }
}
