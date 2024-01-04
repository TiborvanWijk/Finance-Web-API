using FinanceApi.Data.Dtos;
using FinanceApi.Mapper;
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

        public bool Create(User user, CategoryDto categoryDto, out int errorCode, out string errorMessage)
        {
            errorMessage = string.Empty;
            errorCode = 0;

            if (categoryRepository.ExistsBytitle(user.Id, categoryDto.Title))
            {
                errorCode = 400;
                errorMessage = "Category with this title already exists.";
                return false;
            }

            var category = Map.ToCategory(categoryDto);
            category.User = user;

            if (!categoryRepository.Create(category))
            {
                errorCode = 500;
                errorMessage = "Something went wrong while creating category.";
                return false;
            }

            return true;
        }

        public bool Delete(Category category)
        {
            return categoryRepository.Delete(category);
        }

        public int ValidateCategoryUpdate(string userId, CategoryDto categoryDto)
        {
            var categories = categoryRepository.GetAllOfUser(userId);

            bool exists = categoryRepository.ExistsById(userId, categoryDto.Id);

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

        public bool ExistsById(string userId, int id)
        {
            return categoryRepository.ExistsById(userId, id);
        }

        public bool ExistsBytitle(string userId, string title)
        {
            return categoryRepository.ExistsBytitle(userId, title);
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
