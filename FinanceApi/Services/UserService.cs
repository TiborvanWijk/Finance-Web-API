using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;
using FinanceApi.Services.Interfaces;
using FinanceApi.Validators;

namespace FinanceApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IExpenseRepository expenseRepository;
        private readonly IIncomeRepository incomeRepository;

        public UserService(IUserRepository userRepository, IExpenseRepository expenseRepository, IIncomeRepository incomeRepository)
        {
            this.userRepository = userRepository;
            this.expenseRepository = expenseRepository;
            this.incomeRepository = incomeRepository;
        }

        public bool Create(User user)
        {
            return userRepository.Create(user);
        }

        public bool Delete(User user)
        {
            return userRepository.Delete(user);
        }

        public bool ExistsById(string userId)
        {
            return userRepository.ExistsById(userId);
        }

        public bool ExistsByUsername(string username)
        {
            return userRepository.ExistsByUsername(username);
        }

        public User GetById(string userId, bool tracking)
        {
            return userRepository.GetById(userId, tracking);
        }

        public User GetByUsername(string username)
        {
            return userRepository.GetByUsername(username);
        }

        public bool TryGetUserBalance(string userId, out decimal balance, out int errorCode, out string errorMessage)
        {

            errorCode = 0;
            errorMessage = string.Empty;
            balance = 0;

            if (!userRepository.ExistsById(userId))
            {
                errorCode = 404;
                errorMessage = "User not found.";
                return false;
            }

            var expenseTotal = expenseRepository.GetAllOfUser(userId)
                .Where(e => e.Date <= DateTime.Now)
                .Select(e => e.Amount)
                .Sum();

            var incomeTotal = incomeRepository.GetAllOfUser(userId)
                .Where(e => e.Date <= DateTime.Now)
                .Select(i => i.Amount)
                .Sum();

            try
            {
                balance = Math.Round(incomeTotal - expenseTotal,2);
            }
            catch (Exception ex)
            {
                errorCode = 500;
                errorMessage = $"Something unexpected happen with calculating:  {ex.Message}";
                return false;
            }
            return true;
        }

        public bool TryUpdateUserCurrency(string userId, string currency, out int errorCode, out string errorMessage)
        {

            errorCode = 0;
            errorMessage = string.Empty;

            if (!userRepository.ExistsById(userId))
            {
                errorCode = 404;
                errorMessage = "User not found.";
                return false;
            }

            if (!Validator.IsValidCurrencyCode(currency))
            {
                errorCode = 400;
                errorMessage = "Currency ISOcode is not valid.";
                return false;
            }

            var user = userRepository.GetById(userId, true);
            user.Currency = currency.ToUpper();


            if (!userRepository.Update(user))
            {
                errorCode = 500;
                errorMessage = "Something went wrong while updating currency of user.";
                return false;
            }

            return true;
        }

        public bool Update(User user)
        {
            return userRepository.Update(user);
        }
    }
}
