using FinanceApi.Data;
using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;

namespace FinanceApi.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataContext dataContext;

        public CategoryRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
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

        public bool Save()
        {
            var saved = dataContext.SaveChanges();
            return saved > 0;
        }

        public bool Update(Category category)
        {
            throw new NotImplementedException();
        }
    }
}
