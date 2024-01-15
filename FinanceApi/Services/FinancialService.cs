using FinanceApi.Repositories.Interfaces;
using FinanceApi.Services.Interfaces;
using FinanceApi.Validators;
using Microsoft.IdentityModel.Tokens;

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

        public bool TryAverageSpendingPerMonth(string userId, DateTime? startDate, DateTime? endDate, out int errorCode, out string errorMessage)
        {

            errorCode = 0;
            errorMessage = string.Empty;

            throw new NotImplementedException();
        }

        public bool tryGetSavingsRate(string userId, out decimal savingsRate, DateTime? startDate, DateTime? endDate, out int errorCode, out string errorMessage)
        {

            errorCode = 0;
            errorMessage = string.Empty;
            savingsRate = 0;

            try
            {

                decimal totalExpenseAmount;
                decimal totalIncomeAmount;

                if (startDate != null || endDate != null)
                {
                    if (!Validator.ValidateTimePeriod(startDate, endDate, out errorCode, out errorMessage))
                    {
                        return false;
                    }

                    totalIncomeAmount = incomeRepository.GetAllOfUser(userId).Where(i => i.Date >= startDate && i.Date <= endDate).Select(i => i.Amount).Sum();
                    totalExpenseAmount = expenseRepository.GetAllOfUser(userId).Where(e => e.Date >= startDate && e.Date <= endDate).Select(e => e.Amount).Sum();
                }
                else
                {

                    totalIncomeAmount = incomeRepository.GetAllOfUser(userId).Select(i => i.Amount).Sum();
                    totalExpenseAmount = expenseRepository.GetAllOfUser(userId).Select(e => e.Amount).Sum();
                }


                if (totalIncomeAmount == 0 || totalExpenseAmount == 0)
                {
                    savingsRate = totalIncomeAmount == 0 ? 0 : 100;
                    return true;
                }

                savingsRate = Math.Round(100 - (totalExpenseAmount / (totalIncomeAmount / 100)), 2);

                return true;
            }
            catch (Exception ex)
            {
                errorCode = 500;
                errorMessage = $"Error while calculating net income: {ex.Message}";
                return false;
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
