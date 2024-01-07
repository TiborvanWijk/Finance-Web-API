using FinanceApi.Services.Interfaces;

namespace FinanceApi.Services
{
    public class FinancialService : IFinancialService
    {
        private readonly IExpenseService expenseService;
        private readonly IIncomeService incomeService;

        public FinancialService(IExpenseService expenseService, IIncomeService incomeService)
        {
            this.expenseService = expenseService;
            this.incomeService = incomeService;
        }

        public decimal GetSavingsRate(string userId)
        {
            var totalIncomeAmount = incomeService.GetAllOfUser(userId).Select(i => i.Amount).Sum();

            var totalExpenseAmount = expenseService.GetAllOfUser(userId).Select(e => e.Amount).Sum();

            var savingsRate = Math.Round(100 - (totalExpenseAmount / (totalIncomeAmount / 100)), 2);

            return savingsRate;
        }

        public decimal GetSavingsRateInTimePeriod(string userId, DateTime? startdate, DateTime? endDate)
        {
            var totalIncomeAmount = incomeService.GetAllOfUser(userId).Where(i => i.Date >= startdate && i.Date <= endDate).Select(i => i.Amount).Sum();

            var totalExpenseAmount = expenseService.GetAllOfUser(userId).Where(e => e.Date >= startdate && e.Date <= endDate).Select(e => e.Amount).Sum();

            
            if(totalIncomeAmount == 0 || totalExpenseAmount == 0)
            {
                return totalIncomeAmount == 0 ? 0 : 100;
            }

            var savingsRate = Math.Round(100 - (totalExpenseAmount / (totalIncomeAmount / 100)), 2);

            return savingsRate;
        }
    }
}
