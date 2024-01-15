using FinanceApi.Data.Dtos;
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

        public bool TryGetExpensesFilteredOrDefault(string userId,
            out ICollection<Expense> expenses,
            DateTime? startDate,
            DateTime? endDate,
            string? list_order_by,
            string? list_dir,
            out int errorCode,
            out string errorMessage)
        {

            errorCode = 0;
            errorMessage = string.Empty;

            expenses = expenseRepository.GetAllOfUser(userId).OrderByDescending(i => i.Date).ToList();


            if (startDate != null || endDate != null)
            {
                if (!Validator.ValidateTimePeriod(startDate, endDate, out errorCode, out errorMessage))
                {
                    return false;
                }

                expenses = expenses.Where(i => i.Date >= startDate && i.Date <= endDate).ToList();
            }


            expenses = (list_dir != null && list_dir.Equals("desc")) ?
                (list_order_by switch
                {
                    "title" => expenses.OrderByDescending(i => i.Title),
                    "amount" => expenses.OrderByDescending(i => i.Amount),
                    "urgency" => expenses.OrderByDescending(i => i.Urgency),
                    _ => expenses.OrderByDescending(i => i.Date),
                }).ToList()
                :
                (list_order_by switch
                {
                    "title" => expenses.OrderBy(i => i.Title),
                    "amount" => expenses.OrderBy(i => i.Amount),
                    "urgency" => expenses.OrderBy(i => i.Urgency),
                    _ => expenses.OrderBy(i => i.Date),
                }).ToList();

            return true;
        }

        public bool Update(User user, ExpenseDto expenseDto, out int errorCode, out string errorMessage)
        {

            errorCode = 0;
            errorMessage = string.Empty;

            if (!expenseRepository.ExistsById(user.Id, expenseDto.Id))
            {
                errorCode = 404;
                errorMessage = "Expense not found";
                return false;
            }

            if (!ValidateExpense(expenseDto, out errorCode, out errorMessage))
            {
                return false;
            }

            var expense = Map.ToExpense(expenseDto);
            expense.Currency = expense.Currency.ToUpper();

            if (!expenseRepository.Update(expense))
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
                else if (expenseCategories.Any(ec => ec.CategoryId == categoryId))
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

            expenses = expenseRepository.GetAllOfUserByCategoryId(user.Id, categoryId);

            return true;
        }

        public bool TryDeleteExpense(User user, int expenseId, out int errorCode, out string errorMessage)
        {
            errorCode = 0;
            errorMessage = string.Empty;

            if (!expenseRepository.ExistsById(user.Id, expenseId))
            {
                errorCode = 404;
                errorMessage = "Expense not found.";
                return false;
            }

            var expense = expenseRepository.GetById(expenseId, true);

            if (!expenseRepository.Delete(expense))
            {
                errorCode = 500;
                errorMessage = "Something went wrong while deleting expense.";
                return false;
            }

            return true;
        }


        public bool TryRemoveCategories(User user, int expenseId, ICollection<int> categoryIds, out int errorCode, out string errorMessage)
        {

            errorCode = 0;
            errorMessage = string.Empty;

            if (!expenseRepository.ExistsById(user.Id, expenseId))
            {
                errorCode = 404;
                errorMessage = "Expense not found.";
                return false;
            }


            var expenseCategories = categoryRepository.GetExpenseCategories(user.Id, expenseId);

            if (expenseCategories.Count <= 0)
            {
                errorCode = 400;
                errorMessage = "Expense does not have any categories";
                return false;
            }

            foreach (var categoryId in categoryIds)
            {
                if (!categoryRepository.ExistsById(user.Id, categoryId))
                {
                    errorCode = 404;
                    errorMessage = "Category not found.";
                    return false;
                }

                if (!expenseCategories.Any(ic => ic.CategoryId == categoryId))
                {
                    errorCode = 400;
                    errorMessage = "Expense does not have this category";
                    return false;
                }
            }

            foreach (var categoryId in categoryIds)
            {

                if (!expenseRepository.DeleteExpenseCategoryWithId(user.Id, categoryId, expenseId))
                {
                    errorCode = 500;
                    errorMessage = "Something went wrong while deleting expense category.";
                    return false;
                }
            }

            return true;
        }

    }
}
