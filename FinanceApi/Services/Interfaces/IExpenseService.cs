using FinanceApi.Data.Dtos;
using FinanceApi.Models;

namespace FinanceApi.Services.Interfaces
{
    public interface IExpenseService
    {
        Expense GetById(int expenseId, bool tracking);
        bool TryGetExpensesFilteredOrDefault(string userId, out ICollection<Expense> expense, DateTime? startDate, DateTime? endDate, string? list_order_by, string? list_dir, out int errorCode, out string errorMessage);
        bool AddCategory(ExpenseCategory expenseCategory);
        bool AddCategories(string userId, int expenseId, ICollection<int> categoryIds, out string errorMessage, out int errorCode);
        bool ValidateExpense(ExpenseDto expenseDto, out int errorCode, out string errorMessage);
        bool ExistsById(string userId, int expenseId);
        bool Create(User user, ExpenseDto expenseDto, out int errorCode, out string errorMessage);
        bool Update(User user, ExpenseDto expenseDto, out int errorCode, out string errorMessage);
        bool Delete(Expense expense);
        bool tryGetExpensesWithCategoryId(User user, int categoryId, out ICollection<Expense> expenses, out int errorCode, out string errorMessage);
        bool TryDeleteExpense(User user, int expenseId, out int errorCode, out string errorMessage);
        bool TryRemoveCategories(User user, int expenseId, ICollection<int> categoryIds, out int errorCode, out string errorMessage);
    }
}
