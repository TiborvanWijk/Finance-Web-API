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
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository expenseRepository;
        private readonly ICategoryRepository categoryRepository;

        public ExpenseService(IExpenseRepository expenseRepository, ICategoryRepository categoryRepository)
        {
            this.expenseRepository = expenseRepository;
            this.categoryRepository = categoryRepository;
        }

        public bool AddCategory(ExpenseCategory expenseCategory)
        {
            return expenseRepository.AddCategory(expenseCategory);
        }

        public bool Create(User user, ExpenseDto expenseDto, out int errorCode, out string errorMessage)
        {
            errorCode = 0;
            errorMessage = string.Empty;

            if (!ValidateExpense(expenseDto, out errorCode, out errorMessage))
            {
                return false;
            }

            var expense = Map.ToExpense(expenseDto);
            expense.Currency = expense.Currency.ToUpper();
            expense.User = user;

            if (!expenseRepository.Create(expense))
            {
                errorCode = 500;
                errorMessage = "Something went wrong while creating expense.";
                return false;
            }

            return true;
        }

        public bool ValidateExpense(ExpenseDto expenseDto, out int errorCode, out string errorMessage)
        {
            errorCode = 0;
            errorMessage = string.Empty;

            if (!Validator.IsValidCurrencyCode(expenseDto.Currency))
            {
                errorCode = 400;
                errorMessage = "Currency ISOcode is not valid.";
                return false;
            }

            if (expenseDto.Amount <= 0 || expenseDto.Amount > 100000000)
            {
                errorCode = 400;
                errorMessage = "Amount must be more then '0' and smaller then '100000000'.";
                return false;
            }

            if (!Enum.IsDefined(typeof(Urgency), expenseDto.Urgency))
            {
                errorCode = 400;
                errorMessage = "Invalid urgency type.";
                return false;
            }

            return true;
        }

        public bool Delete(Expense expense)
        {
            return expenseRepository.Delete(expense);
        }

        public bool ExistsById(string userId, int expenseId)
        {
            return expenseRepository.ExistsById(userId, expenseId);
        }

        public ICollection<Expense> GetAllOfUser(string userId)
        {
            return expenseRepository.GetAllOfUser(userId);
        }

        public bool Update(User user, ExpenseDto expenseDto, out int errorCode, out string errorMessage, out decimal prevAmount)
        {

            errorCode = 0;
            errorMessage = string.Empty;
            prevAmount = 0;

            if(!expenseRepository.ExistsById(user.Id, expenseDto.Id))
            {
                errorCode = 404;
                errorMessage = "Expense not found";
                return false;
            }

            var expense = expenseRepository.GetById(expenseDto.Id, false);

            if (expense.IsPaid)
            {
                prevAmount = expense.Amount;
            }

            if (!ValidateExpense(expenseDto, out errorCode, out errorMessage))
            {
                return false;
            }

            //expense.Amount = expenseDto.Amount;
            //expense.Date = expenseDto.Date;
            //expense.Description = expenseDto.Description;
            //expense.Title = expenseDto.Title;
            //expense.Currency = expenseDto.Currency;

            var expenseNew = Map.ToExpense(expenseDto);
            expense.Currency = expense.Currency.ToUpper();
            expense = null;

            if (!expenseRepository.Update(expenseNew))
            {
                errorCode = 500;
                errorMessage = "Something went wrong while updating income.";
                return false;
            }

            return true;
        }

        public bool AddCategories(string userId, int expenseId, ICollection<int> categoryIds, out string errorMessage, out int errorCode)
        {
            errorMessage = string.Empty;
            errorCode = 0;
            if (!ExistsById(userId, expenseId))
            {
                errorMessage = "Expense not found.";
                errorCode = 404;
                return false;
            }

            if (categoryIds == null || categoryIds.Count() <= 0)
            {
                errorMessage = "No category id's found.";
                errorCode = 400;
                return false;
            }


            var expense = expenseRepository.GetById(expenseId, true);

            var expenseCategories = categoryRepository.GetExpenseCategories(userId, expenseId);

            foreach (var categoryId in categoryIds)
            {
                if (!categoryRepository.ExistsById(userId, categoryId))
                {
                    errorMessage = "Category not found.";
                    errorCode = 404;
                    return false;
                }
                else if(expenseCategories.Any(ec => ec.CategoryId == categoryId))
                {
                    errorMessage = "Category already added.";
                    errorCode = 400;
                    return false;
                }
            }


            foreach (var categoryId in categoryIds)
            {

                var expenseCategory = new ExpenseCategory()
                {
                    CategoryId = categoryId,
                    Category = categoryRepository.GetById(categoryId, true),
                    ExpenseId = expenseId,
                    Expense = expense,
                };

                if (!expenseRepository.AddCategory(expenseCategory))
                {
                    errorMessage = "Something went wrong while adding category to expense.";
                    errorCode = 500;
                    return false;
                }
            }

            return true;
        }

        public Expense GetById(int expenseId, bool tracking)
        {
            return expenseRepository.GetById(expenseId, tracking);
        }

        public bool tryGetExpensesWithCategoryId(User user, int categoryId, out ICollection<Expense> expenses, out int errorCode, out string errorMessage)
        {
            errorCode = 0;
            errorMessage = string.Empty;
            expenses = new List<Expense>();



            if (!categoryRepository.ExistsById(user.Id, categoryId))
            {
                errorCode = 404;
                errorMessage = "Category not found.";
                return false;
            }

            expenses = expenseRepository.GetAllOfUser(user.Id).Where(e => categoryRepository.GetIncomeCategories(user.Id, e.Id).Any(ec => ec.CategoryId == categoryId)).ToList();

            return true;
        }
    }
}
