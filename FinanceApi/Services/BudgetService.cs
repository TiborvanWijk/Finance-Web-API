using FinanceApi.Data.Dtos;
using FinanceApi.Enums;
using FinanceApi.Mapper;
using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;
using FinanceApi.Services.Interfaces;
using FinanceApi.Validators;
using System.Security.Claims;

namespace FinanceApi.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository budgetRepository;

        public BudgetService(IBudgetRepository budgetRepository)
        {
            this.budgetRepository = budgetRepository;
        }

        public bool Create (User user, BudgetDto budgetDto, out int errorCode, out string errorMessage)
        {
            errorCode = 0;
            errorMessage = string.Empty;


            if (budgetDto.StartDate >= budgetDto.EndDate)
            {
                errorCode = 400;
                errorMessage = "End date must be later then starting date.";
                return false;
            }

            if (budgetDto.LimitAmount <= 0)
            {
                errorCode = 400;
                errorMessage = "Limit amount must me greater then '0'.";
                return false;
            }

            if (!Validator.IsValidCurrencyCode(budgetDto.Currency))
            {
                errorCode = 400;
                errorMessage = "Currency ISOcode is not valid.";
                return false;
            }

            if (!Enum.IsDefined(typeof(Urgency), budgetDto.Urgency))
            {
                errorCode = 400;
                errorMessage = "Invalid urgency type.";
                return false;
            }

            if (ExistsByTitle(user.Id, budgetDto.Title))
            {
                errorCode = 400;
                errorMessage = "Budget with this title already exists.";
                return false;
            }

            var budget = Map.ToBudget(budgetDto);
            budget.Currency = budget.Currency.ToUpper();
            budget.User = user;

            if (!budgetRepository.Create(budget))
            {
                errorCode = 500;
                errorMessage = "Something went wrong while saving budget.";
                return false;
            }

            return true;
        }

        public bool Delete(Budget budget)
        {
            return budgetRepository.Delete(budget);
        }

        public bool ExistsById(string userId, int budgetId)
        {
            return budgetRepository.ExistsById(userId, budgetId);
        }

        public bool ExistsByTitle(string userId, string title)
        {
            return budgetRepository.ExistsByTitle(userId, title);
        }

        public ICollection<Budget> GetAllOfUser(string userId)
        {
            return budgetRepository.GetAllOfUser(userId);
        }

        public Budget GetById(int budgetId)
        {
            return budgetRepository.GetById(budgetId);
        }

        public bool Update(Budget budget)
        {
            return budgetRepository.Update(budget);
        }
    }
}
