using FinanceApi.Models;

namespace FinanceApi.Services.Interfaces
{
    public interface ICategoryService
    {
        Category GetById(int categoryId);
        ICollection<Category> GetAllOfUser(int userId);
        bool ExistsById(int id);
        bool ExistsBytitle(string title);
        bool Create(Category category);
        bool Update(Category category);
        bool Delete(Category category);
    }
}
