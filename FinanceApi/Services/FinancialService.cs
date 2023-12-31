﻿using FinanceApi.Services.Interfaces;
using FinanceApi.Validators;
using Microsoft.IdentityModel.Tokens;

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

        public decimal AverageSpendingPerMonth(string userId, DateTime? startDate, DateTime? endDate)
        {
            throw new NotImplementedException();
        }

        public bool tryGetSavingsRate(string userId, out decimal savingsRate, out int errorCode, out string errorMessage)
        {

            errorCode = 0;
            errorMessage = string.Empty;
            savingsRate = 0;

            try
            {
                var totalIncomeAmount = incomeService.GetAllOfUser(userId).Select(i => i.Amount).Sum();

                var totalExpenseAmount = expenseService.GetAllOfUser(userId).Select(e => e.Amount).Sum();

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

        public bool TryGetSavingsRateInTimePeriod(string userId, DateTime? startDate, DateTime? endDate, out decimal savingsRate, out int errorCode, out string errorMessage)
        {

            errorCode = 0;
            errorMessage = string.Empty;
            savingsRate = 0;


            if(!Validator.ValidateTimePeriod(startDate, endDate, out errorCode, out errorMessage))
            {
                return false;
            }

            try
            {
                var totalIncomeAmount = incomeService.GetAllOfUser(userId).Where(i => i.Date >= startDate && i.Date <= endDate).Select(i => i.Amount).Sum();

                var totalExpenseAmount = expenseService.GetAllOfUser(userId).Where(e => e.Date >= startDate && e.Date <= endDate).Select(e => e.Amount).Sum();

            
                if(totalIncomeAmount == 0 || totalExpenseAmount == 0)
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
            if(!Validator.ValidateTimePeriod(startDate, endDate, out errorCode, out errorMessage))
            {
                return false;
            }

            try
            {
                var totalIncomeAmount = incomeService.GetAllOfUser(userId).Where(i => i.Date >= startDate && i.Date <= endDate).Select(i => i.Amount).Sum();

                var totalExpenseAmount = expenseService.GetAllOfUser(userId).Where(e => e.Date >= startDate && e.Date <= endDate).Select(e => e.Amount).Sum();

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
