using FinanceApi.Data;
using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly DataContext dataContext;

        public ExpenseRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public bool AddCategory(ExpenseCategory expenseCategory)
        {
            dataContext.Add(expenseCategory);
            return Save();
        }

        public bool Create(Expense expense)
        {
            dataContext.Expenses.Add(expense);
            return Save();
        }

        public bool Delete(Expense expense)
        {
            dataContext.Expenses.Remove(expense);
            return Save();
        }

        public bool DeleteExpenseCategoryWithId(string userId, int categoryId, int expenseId)
        {
            dataContext.Remove(dataContext.ExpenseCategories.First(ec => ec.Expense.User.Id.Equals(userId) && ec.Expense.Id == expenseId && ec.CategoryId == categoryId));

            return Save();
        }

        public bool ExistsById(string userId, int expenseId)
        {
            return dataContext.Expenses.Any(e => e.Id == expenseId && e.User.Id.Equals(userId));
        }

        public ICollection<Expense> GetAllOfUser(string userId)
        {
            return dataContext.Expenses.Where(e => e.User.Id.Equals(userId)).ToList();
        }

        public async Task<ICollection<Expense>> GetAllOfUserAsync(string userId)
        {
            return await dataContext.Expenses.Where(e => e.User.Id.Equals(userId)).ToListAsync();
        }

        public ICollection<Expense> GetAllOfUserByBudgetId(string userId, int budgetId)
        {
            return dataContext.Expenses
                .Where(e => e.User.Id.Equals(userId) && e.ExpenseCategories
                .Any(ec => ec.Category.BudgetCategories.Any(bc => bc.BudgetId == budgetId)))
                .ToList();
        }

        public ICollection<Expense> GetAllOfUserByCategoryId(string userId, int categoryId)
        {
            return dataContext.Expenses.Where(e => e.User.Id.Equals(userId) && e.ExpenseCategories.Any(ec => ec.CategoryId == categoryId)).ToList();
        }

        public Expense GetById(int expenseId, bool tracking)
        {
            if(tracking)
            {
                return dataContext.Expenses.FirstOrDefault(e => e.Id == expenseId);
            }
            return dataContext.Expenses.AsNoTracking().FirstOrDefault(e => e.Id == expenseId);
        }

        public bool Save()
        {
            var saved = dataContext.SaveChanges();
            return saved > 0;
        }

        public bool Update(Expense expense)
        {
            dataContext.Expenses.Update(expense);
            return Save();
        }
    }
}
