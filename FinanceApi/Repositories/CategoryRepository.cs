using FinanceApi.Data;
using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataContext dataContext;

        public CategoryRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }
        public bool Create(Category category)
        {
            dataContext.Categories.Add(category);
            return Save();
        }

        public bool Delete(Category category)
        {
            dataContext.Categories.Remove(category);
            return Save();
        }

        public bool ExistsById(string userId, int id)
        {
            return dataContext.Categories.Any(c => c.Id == id && c.User.Id.Equals(userId));
        }

        public bool ExistsBytitle(string userId, string title)
        {
            return dataContext.Categories.Any(c => c.Title.ToLower().Equals(title.ToLower()) && c.User.Id.Equals(userId));
        }

        public ICollection<Category> GetAllOfUser(string userId)
        {
            return dataContext.Categories
                .AsNoTracking()
                .Where(c => c.User.Id.Equals(userId)).ToList();
        }

        public ICollection<BudgetCategory> GetBudgetCategories(string userId, int budgetId)
        {
            return dataContext.BudgetCategories.Where(bc => bc.BudgetId == budgetId && bc.Budget.User.Id.Equals(userId)).ToList();
        }

        public Category GetById(int categoryId, bool tracking)
        {
            if(tracking)
            {
                return dataContext.Categories.FirstOrDefault(c => c.Id == categoryId);
            }
            return dataContext.Categories.AsNoTracking().FirstOrDefault(c => c.Id == categoryId);
        }

        public ICollection<ExpenseCategory> GetExpenseCategories(string userId, int expenseId)
        {
            return dataContext.ExpenseCategories.Where(ec => ec.ExpenseId == expenseId && ec.Expense.User.Id.Equals(userId)).ToList();
        }

        public ICollection<ExpenseCategory> GetExpenseCategoriesByCategoryId(int categoryId)
        {
            return dataContext.ExpenseCategories.Where(ec => ec.CategoryId == categoryId).ToList();
        }

        public ICollection<GoalCategory> GetGoalCategories(string userId, int goalId)
        {
            return dataContext.GoalCategories.Where(gc => gc.Goal.Id == goalId && gc.Goal.User.Id.Equals(userId)).ToList();
        }

        public ICollection<IncomeCategory> GetIncomeCategories(string userId, int incomeId)
        {
            return dataContext.IncomeCategories.Where(ec => ec.IncomeId == incomeId && ec.Income.User.Id.Equals(userId)).ToList();
        }

        public bool Save()
        {
            var saved = dataContext.SaveChanges();
            return saved > 0;
        }

        public bool Update(Category category)
        {
            dataContext.Categories.Update(category);
            return Save();
        }
    }
}
