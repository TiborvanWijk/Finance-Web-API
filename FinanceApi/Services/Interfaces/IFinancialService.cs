namespace FinanceApi.Services.Interfaces
{
    public interface IFinancialService
    {
        decimal GetSavingsRate(string userId);
        decimal GetSavingsRateInTimePeriod(string userId, DateTime? startdate, DateTime? endDate);
    }
}
