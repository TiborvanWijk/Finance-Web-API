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
            throw new NotImplementedException();
        }

        public bool Delete(Budget budget)
        {
            throw new NotImplementedException();
        }

        public bool Exists(int budgetId)
        {
            throw new NotImplementedException();
        }

        public ICollection<Budget> GetAllOfUser(int userId)
        {
            throw new NotImplementedException();
        }

        public Budget GetById(int budgetId)
        {
            throw new NotImplementedException();
        }

        public bool save()
        {
            var saved = dataContext.SaveChanges();
            return saved > 0;
        }

        public bool Update(Budget budget)
        {
            throw new NotImplementedException();
        }
    }
}
