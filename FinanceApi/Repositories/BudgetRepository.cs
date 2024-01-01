using FinanceApi.Data;
using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;

namespace FinanceApi.Repositories
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly DataContext dataContext;

        public BudgetRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
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

        public bool Exists(int budgetId)
        {
            return dataContext.Budgets.Any(b => b.Id == budgetId);
        }

        public ICollection<Budget> GetAllOfUser(string userId)
        {
            return dataContext.Budgets.Where(b => b.User.Id.Equals(userId)).ToList();
        }

        public Budget GetById(int budgetId)
        {
            return dataContext.Budgets.FirstOrDefault(b => b.Id == budgetId);
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
