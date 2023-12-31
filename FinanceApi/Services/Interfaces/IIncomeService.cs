using FinanceApi.Models;

namespace FinanceApi.Services.Interfaces
{
    public interface IIncomeService
    {
        Income GetById(int incomeId);
        ICollection<Income> GetAll(int userId);
        bool Exists(int incomeId);
        bool Create(Income income);
        bool Update(Income income);
        bool Delete(Income income);
    }
}
