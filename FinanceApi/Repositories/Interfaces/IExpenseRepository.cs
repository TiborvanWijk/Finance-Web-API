using FinanceApi.Models;

namespace FinanceApi.Repositories.Interfaces
{
    public interface IExpenseRepository
    {
        Expense GetById(int expenseId, bool tracking);
        ICollection<Expense> GetAllOfUser(string userId);
        ICollection<Expense> GetAllOfUserByBudgetId(string userId, int budgetId);
        ICollection<Expense> GetAllOfUserByCategoryId(string userId, int categoryId);
        bool AddCategory(ExpenseCategory expenseCategory);
        bool ExistsById(string userId, int expenseId);
        bool Create(Expense expense);
        bool Update(Expense expense);
        bool Delete(Expense expense);
        bool Save();
        bool DeleteExpenseCategoryWithId(string userId, int categoryId, int expenseId);
    }
}
