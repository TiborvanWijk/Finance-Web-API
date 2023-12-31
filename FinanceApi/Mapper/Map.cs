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


    }
}
