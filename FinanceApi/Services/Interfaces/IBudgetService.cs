using FinanceApi.Data.Dtos;
using FinanceApi.Models;

namespace FinanceApi.Services.Interfaces
{
    public interface IBudgetService
    {
        bool Create(User user, BudgetManageDto budgetDto, out int errorCode, out string errorMessage);
        bool Update(User user, BudgetManageDto budgetDto, out int errorCode, out string errorMessage);
        bool AddCategories(string userId, int budgetId, ICollection<int> categoryIds, out string errorMessage, out int errorCode);
        bool TryDeleteBudget(User user, int budgetId, out int errorCode, out string errorMessage);
        decimal GetBudgetSpending(string userId, int budgetId);
        bool TryRemoveCategory(User user, int budgetId, int categoryId, out int errorCode, out string errorMessage);
        bool TryGetAllOrderedOrDefault(string userId, out ICollection<Budget> budgets, out int errorCode, out string errorMessage, DateTime? startDate, DateTime? endDate, string? listOrderBy, string? listDir, int? categoryId);
    }
}
