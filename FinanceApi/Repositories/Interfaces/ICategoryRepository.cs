using FinanceApi.Models;

namespace FinanceApi.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Category GetById(int categoryId);
        ICollection<Category> GetAllOfUser(int userId);
        bool ExistsById(int id);
        bool ExistsBytitle(string title);
        bool Create(Category category);
        bool Update(Category category);
        bool Delete(Category category);
        bool Save();
    }
}
