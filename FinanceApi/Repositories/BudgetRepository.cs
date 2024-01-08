using FinanceApi.Data;
using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Repositories
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly DataContext dataContext;

        public BudgetRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public bool AddCategory(BudgetCategory budgetCategory)
        {
            dataContext.BudgetCategories.Add(budgetCategory);
            return save();
        }

        public bool Create(Budget budget)
        {
            dataContext.Budgets.Add(budget);
            return save();
        }

        public bool Delete(Budget budget)
        {
            dataContext.Budgets.Remove(budget);
            return save();
        }

        public bool ExistsById(string userId, int budgetId)
        {
            return dataContext.Budgets.Any(b => b.Id == budgetId && b.User.Id.Equals(userId));
        }

        public bool ExistsByTitle(string userId, string title)
        {
            return dataContext.Budgets.Any(b => b.Title.ToLower().Equals(title.ToLower()) && b.User.Id.Equals(userId));
        }

        public ICollection<Budget> GetAllOfUser(string userId)
        {
            return dataContext.Budgets.AsNoTracking().Where(b => b.User.Id.Equals(userId)).ToList();
        }

        public Budget GetById(int budgetId, bool tracking)
        {
            if(tracking)
            {
                return dataContext.Budgets.FirstOrDefault(b => b.Id == budgetId);
            }
            return dataContext.Budgets.AsNoTracking().FirstOrDefault(b => b.Id == budgetId);
        }

        public bool save()
        {
            var saved = dataContext.SaveChanges();
            return saved > 0;
        }

        public bool Update(Budget budget)
        {
            dataContext.Budgets.Update(budget);
            return save();
        }
    }
}
