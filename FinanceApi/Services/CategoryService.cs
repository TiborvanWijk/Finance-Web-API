using FinanceApi.Data;
using FinanceApi.Data.Dtos;
using FinanceApi.Mapper;
using FinanceApi.Models;
using FinanceApi.Repositories;
using FinanceApi.Repositories.Interfaces;
using FinanceApi.Services.Interfaces;
using FinanceApi.Validators;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IExpenseRepository expenseRepository;
        private readonly IUserRepository userRepository;

        public CategoryService(ICategoryRepository categoryRepository, IExpenseRepository expenseRepository, IUserRepository userRepository)
        {
            this.categoryRepository = categoryRepository;
            this.expenseRepository = expenseRepository;
            this.userRepository = userRepository;
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

        public Category GetById(int categoryId, bool tracking)
        {
            return categoryRepository.GetById(categoryId, tracking);
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

        public bool TryDelete(User user, int categoryId, out int errorCode, out string errorMessage)
        {

            errorCode = 0;
            errorMessage = string.Empty;


            if(!categoryRepository.ExistsById(user.Id, categoryId))
            {
                errorCode = 404;
                errorMessage = "Category not found.";
                return false;
            }

            var category = categoryRepository.GetById(categoryId, true);

            if (!categoryRepository.Delete(category))
            {
                errorCode = 500;
                errorMessage = "Something went wrong while deleting category.";
                return false;
            }

            return true;
        }

        public bool TryGetCategoriesFilteredOrDefault(string userId, out ICollection<Category> categories, out int errorCode, out string errorMessage, string ListOrderBy, string listDir)
        {
            errorCode = 0;
            errorMessage = string.Empty;
            categories = new List<Category>();

            if (!userRepository.ExistsById(userId))
            {
                errorCode = 401;
                errorMessage = "Unauthorized.";
                return false;
            }

            try
            {

                var user = userRepository.GetById(userId, true);

                categories = categoryRepository.GetAllOfUser(userId);

                categories = listDir != null && listDir.Equals("desc") ?
                    (ListOrderBy switch
                    {
                        "title" => categories.OrderByDescending(c => c.Title),
                        "expense" => categoryRepository.GetCategoriesIncludingExpenseCategoriesAndExpense(user.Id)
                                .OrderByDescending(c => c.ExpenseCategories.Sum(ec => ec.Expense != null ? ec.Expense.Amount : 0)),
                                _ => categories.OrderByDescending(c => c.Id),
                                
                    }).ToList()
                    :
                    (ListOrderBy switch
                    {
                        "expense" => categories.OrderBy(c => c.Title),
                        "spending" => categoryRepository.GetCategoriesIncludingExpenseCategoriesAndExpense(user.Id)
                                .OrderBy(c => c.ExpenseCategories.Sum(ec => ec.Expense != null ? ec.Expense.Amount : 0)),
                        _ => categories.OrderBy(c => c.Id),

                    }).ToList();

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }



        }
    }
}
