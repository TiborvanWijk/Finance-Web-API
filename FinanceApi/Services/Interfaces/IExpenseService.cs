using FinanceApi.Data.Dtos;
using FinanceApi.Models;

namespace FinanceApi.Services.Interfaces
{
    public interface IExpenseService
    {
        Expense GetById(int expenseId);
        ICollection<Expense> GetAllOfUser(string userId);
        bool AddCategory(ExpenseCategory expenseCategory);
        public bool AddCategories(string userId, int expenseId, ICollection<int> categoryIds, out string errorMessage, out int errorCode);
        bool ValidateExpense(ExpenseDto expenseDto, out int errorCode, out string errorMessage);
        bool ExistsById(string userId, int expenseId);
        bool Create(User user, ExpenseDto expenseDto, out int errorCode, out string errorMessage);
        bool Update(User user, ExpenseDto expenseDto, out int errorCode, out string errorMessage, out decimal prevAmount);
        bool Delete(Expense expense);
    }
}
