using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;
using FinanceApi.Services.Interfaces;

namespace FinanceApi.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }
        public bool Create(Category category)
        {
            return categoryRepository.Create(category);
        }

        public bool Delete(Category category)
        {
            return categoryRepository.Delete(category);
        }

        public bool ExistsById(int id)
        {
            return categoryRepository.ExistsById(id);
        }

        public bool ExistsBytitle(string title)
        {
            return categoryRepository.ExistsBytitle(title);
        }

        public ICollection<Category> GetAllOfUser(string userId)
        {
            return categoryRepository.GetAllOfUser(userId);
        }

        public Category GetById(int categoryId)
        {
            return categoryRepository.GetById(categoryId);
        }

        public bool Update(Category category)
        {
            return categoryRepository.Update(category);
        }
    }
}
