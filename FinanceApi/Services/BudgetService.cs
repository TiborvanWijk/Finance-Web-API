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
            return budgetRepository.Create(budget);
        }

        public bool Delete(Budget budget)
        {
            return budgetRepository.Delete(budget);
        }

        public bool Exists(int budgetId)
        {
            return budgetRepository.Exists(budgetId);
        }

        public ICollection<Budget> GetAllOfUser(string userId)
        {
            return budgetRepository.GetAllOfUser(userId);
        }

        public Budget GetById(int budgetId)
        {
            return budgetRepository.GetById(budgetId);
        }

        public bool Update(Budget budget)
        {
            return budgetRepository.Update(budget);
        }
    }
}
