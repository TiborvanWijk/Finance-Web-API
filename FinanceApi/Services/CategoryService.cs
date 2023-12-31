using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;
using FinanceApi.Services.Interfaces;

namespace FinanceApi.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IBudgetRepository budgetRepository;

        public CategoryService(IBudgetRepository budgetRepository)
        {
            this.budgetRepository = budgetRepository;
        }
        public bool Create(Category category)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Category category)
        {
            throw new NotImplementedException();
        }

        public bool ExistsById(int id)
        {
            throw new NotImplementedException();
        }

        public bool ExistsBytitle(string title)
        {
            throw new NotImplementedException();
        }

        public ICollection<Category> GetAllOfUser(int userId)
        {
            throw new NotImplementedException();
        }

        public Category GetById(int categoryId)
        {
            throw new NotImplementedException();
        }

        public bool Update(Category category)
        {
            throw new NotImplementedException();
        }
    }
}
