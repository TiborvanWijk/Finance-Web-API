using FinanceApi.Data.Dtos;
using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;
using FinanceApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

        public int ValidateCategoryUpdate(string userId, CategoryDto categoryDto)
        {
            var categories = categoryRepository.GetAllOfUser(userId);

            bool exists = categoryRepository.ExistsById(categoryDto.Id);

            bool isUsers = categories.Any(c => c.Id == categoryDto.Id);

            bool isNotUnique = categories.Any(c => c.Id != categoryDto.Id && c.Title.ToLower().Equals(categoryDto.Title.ToLower()));


            if (!exists || !isUsers)
            {
                return 404;
            }
            else if (isNotUnique)
            {
                return 400;
            }



            return 200;
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
