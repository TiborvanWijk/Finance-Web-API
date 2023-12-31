using FinanceApi.Models;

namespace FinanceApi.Services.Interfaces
{
    public interface IExpenseService
    {
        Expense GetById(int expenseId);
        ICollection<Expense> GetAllOfUser(int userId);
        bool Exists(int expenseId);
        bool Create(Expense expense);
        bool Update(Expense expense);
        bool Delete(Expense expense);
    }
}
