using FinanceApi.Models;

namespace FinanceApi.Repositories.Interfaces
{
    public interface IUserRepository
    {
        User GetById(int userId);
        User GetByUsername(string username);
        ICollection<User> GetAll();
        bool ExistsById(int userId);
        bool ExistsByUsername(string username);
        bool Create(User user);
        bool Update(User user);
        bool Delete(User user);
        bool Save();

    }
}
