using FinanceApi.Data;
using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;

namespace FinanceApi.Repositories
{
    public class IncomeRepository : IIncomeRepository
    {
        private readonly DataContext dataContext;

        public IncomeRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }
        public bool Create(Income income)
        {
            dataContext.Incomes.Add(income);
            return Save();
        }

        public bool Delete(Income income)
        {
            dataContext.Incomes.Remove(income);
            return Save();
        }

        public bool Exists(int incomeId)
        {
            return dataContext.Incomes.Any(i => i.Id == incomeId);
        }

        public ICollection<Income> GetAllByUserId(string userId)
        {
            return dataContext.Incomes.Where(i => i.User.Id == userId).ToList();
        }

        public Income GetById(int incomeId)
        {
            return dataContext.Incomes.FirstOrDefault(i => i.Id == incomeId);
        }

        public bool Save()
        {
            var saved = dataContext.SaveChanges();
            return saved > 0;
        }

        public bool Update(Income income)
        {
            dataContext.Incomes.Update(income);
            return Save();
        }
    }
}
