using FinanceApi.Data;
using FinanceApi.Data.Dtos;
using FinanceApi.Mapper;
using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;
using FinanceApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IExpenseRepository expenseRepository;

        public CategoryService(ICategoryRepository categoryRepository, IExpenseRepository expenseRepository)
        {
            this.categoryRepository = categoryRepository;
            this.expenseRepository = expenseRepository;
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

        public bool TryGetCategoriesSortedByExpenseAmount(User user, out ICollection<Category> categories, out int errorCode, out string errorMessage)
        {
            errorCode = 0;
            errorMessage = string.Empty;
            categories = new List<Category>();


            try
            {
                categories = categoryRepository.GetCategoriesIncludingExpenseCategoriesAndExpense(user.Id)
                    .OrderByDescending(c => c.ExpenseCategories.Sum(ec => ec.Expense != null ? ec.Expense.Amount : 0)).ToList();

            }
            catch (Exception ex)
            {
                errorCode = 500;
                errorMessage = ex.Message;
                return false;
            }


            return true;
        }

        public bool TryGetCategoryExpenseAmount(User user, int categoryId, out decimal expenseAmount, out int errorCode, out string errorMessage)
        {
            errorCode = 0;
            errorMessage = string.Empty;
            expenseAmount = 0;

            if(!categoryRepository.ExistsById(user.Id, categoryId))
            {
                errorCode = 404;
                errorMessage = "Category not found.";
                return false;
            }


            if (!categoryRepository.HasExpenses(categoryId))
            {
                errorCode = 400;
                errorMessage = "Category has no expenses.";
                return false;
            }

            expenseAmount = expenseRepository.GetAllOfUserByCategoryId(user.Id, categoryId).Sum(e => e.Amount);

            return true;
        }
    }
}
