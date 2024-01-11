using FinanceApi.Models;

namespace FinanceApi.Repositories.Interfaces
{
    public interface IBudgetRepository
    {
        Budget GetById(int budgetId, bool tracking);
        ICollection<Budget> GetAllOfUser(string userId);
        ICollection<Budget> GetAllOfUserByCategoryId(string userId, int categoryId);
        bool ExistsById(string userId, int budgetId);
        bool Create(Budget budget);
        bool Update(Budget budget);
        bool Delete(Budget budget);
        bool save();
        bool ExistsByTitle(string userId, string title);
        bool AddCategory(BudgetCategory budgetCategory);
        bool DeleteBudgetCategoryWithId(string userId, int categoryId, int budgetId);
    }
}
