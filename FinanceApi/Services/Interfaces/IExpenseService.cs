using FinanceApi.Data.Dtos;
using FinanceApi.Models;

namespace FinanceApi.Services.Interfaces
{
    public interface IExpenseService
    {
        Expense GetById(int expenseId);
        ICollection<Expense> GetAllOfUser(string userId);
        bool AddCategory(ExpenseCategory expenseCategory);
        public bool AddCategories(string userId, int expenseId, ICollection<int> categoryIds, out string errorMessage, out int responseCode);
        bool Exists(string userId, int expenseId);
        bool Create(User user, ExpenseDto expenseDto, out int errorCode, out string errorMessage);
        bool Update(Expense expense);
        bool Delete(Expense expense);
    }
}
