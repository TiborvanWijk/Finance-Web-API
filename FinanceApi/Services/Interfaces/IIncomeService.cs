using FinanceApi.Data.Dtos;
using FinanceApi.Models;
using FinanceApi.Repositories;

namespace FinanceApi.Services.Interfaces
{
    public interface IIncomeService
    {
        Income GetById(int incomeId);
        ICollection<Income> GetAllByUserId(string userId);
        public bool AddCategory(IncomeCategory incomeCategory);
        bool ExistsById(string userId, int incomeId);
        bool ValidateIncome(IncomeDto incomeDto, out int errorCode, out string errorMessage);
        bool Create(User user, IncomeDto incomeDto, out int errorCode, out string errorMessage);
        bool Update(User user, IncomeDto incomeDto, out int errorCode, out string errorMessage, out decimal prevAmount);
        bool Delete(Income income);
        bool AddCategories(string userId, int incomeId, ICollection<int> categoryIds, out string errorMessage, out int errorCode);
    }
}