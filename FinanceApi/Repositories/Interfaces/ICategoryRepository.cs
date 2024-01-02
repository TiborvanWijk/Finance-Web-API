using FinanceApi.Models;

namespace FinanceApi.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Category GetById(int categoryId);
        ICollection<Category> GetAllOfUser(string userId);
        ICollection<ExpenseCategory> GetExpenseCategories(string userId, int expenseId);
        bool ExistsById(string userId, int id);
        bool ExistsBytitle(string userId, string title);
        bool Create(Category category);
        bool Update(Category category);
        bool Delete(Category category);
        bool Save();
        ICollection<IncomeCategory> GetIncomeCategories(string userId, int incomeId);
    }
}
