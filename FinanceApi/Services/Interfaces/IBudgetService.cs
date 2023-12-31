using FinanceApi.Models;

namespace FinanceApi.Services.Interfaces
{
    public interface IBudgetService
    {
        Budget GetById(int budgetId);
        ICollection<Budget> GetAllOfUser(int userId);
        bool Exists(int budgetId);
        bool Create(Budget budget);
        bool Update(Budget budget);
        bool Delete(Budget budget);
    }
}
