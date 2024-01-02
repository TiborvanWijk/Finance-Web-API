using FinanceApi.Models;

namespace FinanceApi.Repositories.Interfaces
{
    public interface IIncomeRepository
    {
        Income GetById(int incomeId);
        ICollection<Income> GetAllByUserId(string userId);
        bool Exists(string userId, int incomeId);
        bool Create(Income income);
        bool Update(Income income);
        bool Delete(Income income);
        bool Save();
    }
}
