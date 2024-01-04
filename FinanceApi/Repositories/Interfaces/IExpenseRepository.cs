using FinanceApi.Models;

namespace FinanceApi.Repositories.Interfaces
{
    public interface IExpenseRepository
    {
        Expense GetById(int expenseId);
        ICollection<Expense> GetAllOfUser(string userId);
        bool AddCategory(ExpenseCategory expenseCategory);
        bool ExistsById(string userId, int expenseId);
        bool Create(Expense expense);
        bool Update(Expense expense);
        bool Delete(Expense expense);
        bool Save();
    }
}
