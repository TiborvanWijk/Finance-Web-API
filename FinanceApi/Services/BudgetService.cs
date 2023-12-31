﻿using FinanceApi.Data.Dtos;
using FinanceApi.Enums;
using FinanceApi.Mapper;
using FinanceApi.Models;
using FinanceApi.Repositories;
using FinanceApi.Repositories.Interfaces;
using FinanceApi.Services.Interfaces;
using FinanceApi.Validators;
using System.Security.Claims;

namespace FinanceApi.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository budgetRepository;
        private readonly ICategoryRepository categoryRepository;

        public BudgetService(IBudgetRepository budgetRepository, ICategoryRepository categoryRepository)
        {
            this.budgetRepository = budgetRepository;
            this.categoryRepository = categoryRepository;
        }

        public bool Create (User user, BudgetDto budgetDto, out int errorCode, out string errorMessage)
        {
            errorCode = 0;
            errorMessage = string.Empty;

            if (ExistsByTitle(user.Id, budgetDto.Title))
            {
                errorCode = 400;
                errorMessage = "Budget with this title already exists.";
                return false;
            }

            if (!ValidateBudget(budgetDto, out errorCode, out errorMessage, user.Id))
            {
                return false;
            }

            var budget = Map.ToBudget(budgetDto);
            budget.Currency = budget.Currency.ToUpper();
            budget.User = user;

            if (!budgetRepository.Create(budget))
            {
                errorCode = 500;
                errorMessage = "Something went wrong while saving budget.";
                return false;
            }

            return true;
        }

        public bool ValidateBudget(BudgetDto budgetDto, out int errorCode, out string errorMessage, string userId)
        {
            errorCode = 0;
            errorMessage = string.Empty;

            if (budgetDto.StartDate >= budgetDto.EndDate)
            {
                errorCode = 400;
                errorMessage = "End date must be later then starting date.";
                return false;
            }

            if (budgetDto.LimitAmount <= 0)
            {
                errorCode = 400;
                errorMessage = "Limit amount must me greater then '0'.";
                return false;
            }

            if (!Validator.IsValidCurrencyCode(budgetDto.Currency))
            {
                errorCode = 400;
                errorMessage = "Currency ISOcode is not valid.";
                return false;
            }

            if (!Enum.IsDefined(typeof(Urgency), budgetDto.Urgency))
            {
                errorCode = 400;
                errorMessage = "Invalid urgency type.";
                return false;
            }

            return true;
        }

        public bool Delete(Budget budget)
        {
            return budgetRepository.Delete(budget);
        }

        public bool ExistsById(string userId, int budgetId)
        {
            return budgetRepository.ExistsById(userId, budgetId);
        }

        public bool ExistsByTitle(string userId, string title)
        {
            return budgetRepository.ExistsByTitle(userId, title);
        }

        public ICollection<Budget> GetAllOfUser(string userId)
        {
            return budgetRepository.GetAllOfUser(userId);
        }

        public Budget GetById(int budgetId, bool tracking)
        {
            return budgetRepository.GetById(budgetId, tracking);
        }

        public bool Update(User user, BudgetDto budgetDto, out int errorCode, out string errorMessage)
        {
            errorCode = 0;
            errorMessage = string.Empty;

            if(!budgetRepository.ExistsById(user.Id, budgetDto.Id))
            {
                errorCode = 404;
                errorMessage = "Budget not found.";
                return false;
            }

            bool titleInUse = budgetRepository.GetAllOfUser(user.Id).Any(b => b.Id != budgetDto.Id && b.Title.ToLower().Equals(budgetDto.Title.ToLower()));
            if (titleInUse)
            {
                errorCode = 400;
                errorMessage = "Budget with that title already exists.";
                return false;
            }

            if (!ValidateBudget(budgetDto, out errorCode, out errorMessage, user.Id))
            {
                return false;
            }

            var budget = Map.ToBudget(budgetDto);
            budget.Currency = budget.Currency.ToUpper();

            if (!budgetRepository.Update(budget))
            {
                errorCode = 500;
                errorMessage = "Something went wrong while updating budget.";
                return false;
            }

            return true;
        }

        public bool AddCategories(string userId, int budgetId, ICollection<int> categoryIds, out string errorMessage, out int errorCode)
        {

            errorMessage = string.Empty;
            errorCode = 0;
            if (!ExistsById(userId, budgetId))
            {
                errorMessage = "Budget not found.";
                errorCode = 404;
                return false;
            }

            if (categoryIds == null || categoryIds.Count() <= 0)
            {
                errorMessage = "No category id's found.";
                errorCode = 400;
                return false;
            }


            var budget = budgetRepository.GetById(budgetId, true);

            var budgetCategories = categoryRepository.GetBudgetCategories(userId, budgetId);

            foreach (var categoryId in categoryIds)
            {
                if (!categoryRepository.ExistsById(userId, categoryId))
                {
                    errorMessage = "Category not found.";
                    errorCode = 404;
                    return false;
                }
                else if (budgetCategories.Any(bc => bc.CategoryId == categoryId))
                {
                    errorMessage = "Category already added.";
                    errorCode = 400;
                    return false;
                }
            }


            foreach (var categoryId in categoryIds)
            {

                var budgetCategory = new BudgetCategory()
                {
                    CategoryId = categoryId,
                    Category = categoryRepository.GetById(categoryId, true),
                    BudgetId = budgetId,
                    Budget = budget,
                };

                if (!budgetRepository.AddCategory(budgetCategory))
                {
                    errorMessage = "Something went wrong while adding category to budget.";
                    errorCode = 500;
                    return false;
                }
            }

            return true;
        }

        public bool TryGetBudgetsByCategoryId(User user, int categoryId, out ICollection<Budget> budgets, out int errorCode, out string errorMessage)
        {

            errorCode = 0;
            errorMessage = string.Empty;
            budgets = new List<Budget>();


            if(!categoryRepository.ExistsById(user.Id, categoryId))
            {
                errorCode = 404;
                errorMessage = "Category not found.";
                return false;
            }

            budgets = budgetRepository.GetAllOfUser(user.Id).Where(b => categoryRepository.GetBudgetCategories(user.Id, b.Id).Any(bc => bc.CategoryId == categoryId)).ToList();


            return true;
        }
    }
}
