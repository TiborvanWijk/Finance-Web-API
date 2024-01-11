using FinanceApi.Data.Dtos;
using FinanceApi.Models;

namespace FinanceApi.Services.Interfaces
{
    public interface IBudgetService
    {
        Budget GetById(int budgetId, bool tracking);
        ICollection<Budget> GetAllOfUser(string userId);
        bool ExistsById(string userId, int budgetId);
        bool Create(User user, BudgetDto budgetDto, out int errorCode, out string errorMessage);
        bool Update(User user, BudgetDto budgetDto, out int errorCode, out string errorMessage);
        bool ExistsByTitle(string userId, string title);
        bool AddCategories(string userId, int budgetId, ICollection<int> categoryIds, out string errorMessage, out int errorCode);
        bool TryGetBudgetsByCategoryId(User user, int categoryId, out ICollection<Budget> budgets, out int errorCode, out string errorMessage);
        bool TryDeleteBudget(User user, int budgetId, out int errorCode, out string errorMessage);
        bool TryGetBudgetSpending(User user, int budgetId, out decimal spending, out int errorCode, out string errorMessage);
        bool TryRemoveCategories(User user, int budgetId, ICollection<int> categoryIds, out int errorCode, out string errorMessage);
    }
}
