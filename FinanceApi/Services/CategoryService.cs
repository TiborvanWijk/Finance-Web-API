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

        public bool Update(User user, CategoryDto categoryDto, out int errorCode, out string errorMessage)
        {
            errorCode = 0;
            errorMessage = string.Empty;

            if (!categoryRepository.ExistsById(user.Id, categoryDto.Id))
            {
                errorCode = 404;
                errorMessage = "Category does not exist.";
                return false;
            }
            bool isNotUnique = categoryRepository.GetAllOfUser(user.Id).Any(c => c.Id != categoryDto.Id && c.Title.ToLower().Equals(categoryDto.Title.ToLower()));
            if (isNotUnique)
            {
                errorCode = 400;
                errorMessage = "Category with this title already exists.";
                return false;
            }

            var category = Map.ToCategory(categoryDto);

            if (!categoryRepository.Update(category))
            {
                errorCode = 500;
                errorMessage = "Something went wrong while updating.";
                return false;
            }

            return true;
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

        public Category GetById(int categoryId, bool tracking)
        {
            return categoryRepository.GetById(categoryId, tracking);
        }
    }
}
