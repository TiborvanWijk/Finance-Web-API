using FinanceApi.Models;

namespace FinanceApi.Services.Interfaces
{
    public interface IFinancialService
    {
        Task<decimal?> tryGetSavingsRateAsync(string userId, bool validDateTime, DateTime? startDate, DateTime? endDate);
        bool TryGetNetIncomeInTimePeriod(string userId, DateTime? startDate, DateTime? endDate, out decimal netIncome, out int errorCode, out string errorMessage);
    }
}
