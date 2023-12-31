using FinanceApi.Data.Dtos;
using FinanceApi.Models;

namespace FinanceApi.Mapper
{
    public abstract class Map
    {

        public static UserDto ToUserDto(User user)
        {
            var userDto = new UserDto()
            {
                Id = user.Id,
                Username = user.UserName,
                Balance = user.Balance,
                Currency = user.Currency,

            };

            return userDto;
        }

        internal static IncomeDto ToIncomeDto(Income income)
        {
            var incomeDto = new IncomeDto()
            {
                Id = income.Id,
                Title = income.Title,
                Description = income.Description,
                Currency = income.Currency,
                Amount = income.Amount,
                Status = income.Status,
                Date = income.Date,
            };
            return incomeDto;
        }
    }
}
