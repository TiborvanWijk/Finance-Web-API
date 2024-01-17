using FinanceApi.Data.Dtos;
using FinanceApi.Models;

namespace FinanceApi.Services.Interfaces
{
    public interface IBudgetService
    {
        Budget GetById(int budgetId, bool tracking);
        bool ExistsById(string userId, int budgetId);
        bool Create(User user, BudgetManageDto budgetDto, out int errorCode, out string errorMessage);
        bool Update(User user, BudgetManageDto budgetDto, out int errorCode, out string errorMessage);
        bool ExistsByTitle(string userId, string title);
        bool AddCategories(string userId, int budgetId, ICollection<int> categoryIds, out string errorMessage, out int errorCode);
        bool TryGetBudgetsByCategoryId(User user, int categoryId, out ICollection<Budget> budgets, out int errorCode, out string errorMessage);
        bool TryDeleteBudget(User user, int budgetId, out int errorCode, out string errorMessage);
        decimal GetBudgetSpending(string userId, int budgetId);
        bool TryRemoveCategories(User user, int budgetId, ICollection<int> categoryIds, out int errorCode, out string errorMessage);
        bool TryGetAllOrderedOrDefault(string userId, out ICollection<Budget> budgets, out int errorCode, out string errorMessage,
            DateTime? startDate, DateTime? endDate, string? listOrderBy, string? listDir);
    }
}
