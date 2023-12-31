using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;
using FinanceApi.Services.Interfaces;

namespace FinanceApi.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository budgetRepository;

        public BudgetService(IBudgetRepository budgetRepository)
        {
            this.budgetRepository = budgetRepository;
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

        public bool Update(Budget budget)
        {
            throw new NotImplementedException();
        }
    }
}
