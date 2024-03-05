using FinanceApi.Data.Dtos;
using FinanceApi.Models;
using FinanceApi.Repositories;

namespace FinanceApi.Services.Interfaces
{
    public interface IIncomeService
    {
        Income GetById(int incomeId, bool tracking);
        public bool TryGetIncomesFilteredOrDefault(string userId, out ICollection<Income> incomes, DateTime? startDate, DateTime? endDate,  string? list_order_by, string? list_dir, int? categoryId, out int errorCode, out string errorMessage); 
        bool AddCategory(IncomeCategory incomeCategory);
        bool ExistsById(string userId, int incomeId);
        bool ValidateIncome(IncomeDto incomeDto, out int errorCode, out string errorMessage);
        bool Create(User user, IncomeDto incomeDto, out int errorCode, out string errorMessage);
        bool Update(User user, IncomeDto incomeDto, out int errorCode, out string errorMessage);
        bool Delete(Income income);
        bool AddCategories(string userId, int incomeId, ICollection<int> categoryIds, out string errorMessage, out int errorCode);
        bool TryDeleteIncome(User user, int incomeId, out int errorCode, out string errorMessage);
        bool TryRemoveCategories(User user, int incomeId, int categoryId, out int errorCode, out string errorMessage);
    }
}