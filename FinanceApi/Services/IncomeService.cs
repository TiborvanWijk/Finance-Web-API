using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;
using FinanceApi.Services.Interfaces;

namespace FinanceApi.Services
{
    public class IncomeService : IIncomeService
    {
        private readonly IIncomeRepository incomeRepository;

        public IncomeService(IIncomeRepository incomeRepository)
        {
            this.incomeRepository = incomeRepository;
        }
        public bool Create(Income income)
        {
            return incomeRepository.Create(income);
        }

        public bool Delete(Income income)
        {
            return incomeRepository.Delete(income);
        }

        public bool Exists(int incomeId)
        {
            return incomeRepository.Exists(incomeId);
        }

        public ICollection<Income> GetAllByUserId(string userId)
        {
            return incomeRepository.GetAllByUserId(userId);
        }

        public Income GetById(int incomeId)
        {
            return incomeRepository.GetById(incomeId);
        }

        public bool Update(Income income)
        {
            return incomeRepository.Update(income);
        }
    }
}
