using FinanceApi.Repositories.Interfaces;
using FinanceApi.Services.Interfaces;
using FinanceApi.Validators;

namespace FinanceApi.Services
{
    public class FinancialService : IFinancialService
    {
        private readonly IIncomeRepository incomeRepository;
        private readonly IExpenseRepository expenseRepository;
        private readonly IUserRepository userRepository;

        public FinancialService(IIncomeRepository incomeRepository, IExpenseRepository expenseRepository, IUserRepository userRepository)
        {
            this.incomeRepository = incomeRepository;
            this.expenseRepository = expenseRepository;
            this.userRepository = userRepository;
        }

        public async Task<decimal?> tryGetSavingsRateAsync(string userId, bool validDateTime, DateTime? startDate, DateTime? endDate)
        {

            try
            {

                decimal totalExpenseAmount;
                decimal totalIncomeAmount;


                if (validDateTime)
                {
                    var incomes = await incomeRepository.GetAllOfUserAsync(userId);

                    var expenses = await expenseRepository.GetAllOfUserAsync(userId);

                    totalIncomeAmount = incomes.Where(i => i.Date >= startDate && i.Date <= endDate).Select(i => i.Amount).Sum();
                    totalExpenseAmount = expenses.Where(e => e.Date >= startDate && e.Date <= endDate).Select(e => e.Amount).Sum();
                }
                else
                {
                    var incomes = await incomeRepository.GetAllOfUserAsync(userId);

                    var expenses = await expenseRepository.GetAllOfUserAsync(userId);

                    totalIncomeAmount = incomes.Select(i => i.Amount).Sum();
                    totalExpenseAmount = expenses.Select(e => e.Amount).Sum();
                }


                if (totalIncomeAmount == 0 || totalExpenseAmount == 0)
                {
                    return totalIncomeAmount == 0 ? 0 : 100;
                }

                return Math.Round(100 - (totalExpenseAmount / (totalIncomeAmount / 100)), 2);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool TryGetNetIncomeInTimePeriod(string userId, DateTime? startDate, DateTime? endDate, out decimal netIncome, out int errorCode, out string errorMessage)
        {

            errorCode = 0;
            errorMessage = string.Empty;
            netIncome = 0;
            if (!Validator.ValidateTimePeriod(startDate, endDate, out errorCode, out errorMessage))
            {
                return false;
            }

            try
            {
                var totalIncomeAmount = incomeRepository.GetAllOfUser(userId).Where(i => i.Date >= startDate && i.Date <= endDate).Select(i => i.Amount).Sum();

                var totalExpenseAmount = expenseRepository.GetAllOfUser(userId).Where(e => e.Date >= startDate && e.Date <= endDate).Select(e => e.Amount).Sum();

                netIncome = Math.Round(totalIncomeAmount - totalExpenseAmount, 2);

                return true;
            }
            catch (Exception ex)
            {
                errorCode = 500;
                errorMessage = $"Error while calculating net income: {ex.Message}";
                return false;
            }
        }
    }
}
