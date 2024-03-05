using FinanceApi.Data.Dtos;
using FinanceApi.Models;

namespace FinanceApi.Services.Interfaces
{
    public interface IExpenseService
    {
        bool TryGetExpensesFilteredOrDefault(string userId, out ICollection<Expense> expense, DateTime? startDate, DateTime? endDate, string? list_order_by, string? list_dir, int? categoryId, out int errorCode, out string errorMessage);
        bool AddCategories(string userId, int expenseId, ICollection<int> categoryIds, out string errorMessage, out int errorCode);
        bool ValidateExpense(ExpenseDto expenseDto, out int errorCode, out string errorMessage);
        bool Create(User user, ExpenseDto expenseDto, out int errorCode, out string errorMessage);
        bool Update(User user, ExpenseDto expenseDto, out int errorCode, out string errorMessage);
        bool TryDeleteExpense(User user, int expenseId, out int errorCode, out string errorMessage);
        bool TryRemoveCategories(User user, int expenseId, int categoryId, out int errorCode, out string errorMessage);
    }
}
