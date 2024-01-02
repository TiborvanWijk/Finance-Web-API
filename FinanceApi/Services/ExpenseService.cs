using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;
using FinanceApi.Services.Interfaces;

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

        public bool Create(Expense expense)
        {
            return expenseRepository.Create(expense);
        }

        public bool Delete(Expense expense)
        {
            return expenseRepository.Delete(expense);
        }

        public bool Exists(string userId, int expenseId)
        {
            return expenseRepository.Exists(userId, expenseId);
        }

        public ICollection<Expense> GetAllOfUser(string userId)
        {
            return expenseRepository.GetAllOfUser(userId);
        }

        public Expense GetById(int expenseId)
        {
            return expenseRepository.GetById(expenseId);
        }

        public bool Update(Expense expense)
        {
            return expenseRepository.Update(expense);
        }

        public bool AddCategories(string userId, int expenseId, ICollection<int> categoryIds, out string errorMessage, out int responseCode)
        {
            errorMessage = string.Empty;
            responseCode = 200;
            if (!Exists(userId, expenseId))
            {
                errorMessage = "Expense not found.";
                responseCode = 404;
                return false;
            }

            if (categoryIds == null || categoryIds.Count() <= 0)
            {
                errorMessage = "No category id's found.";
                responseCode = 400;
                return false;
            }


            var expense = GetById(expenseId);

            var expenseCategories = categoryRepository.GetExpenseCategories(userId, expenseId);

            foreach (var categoryId in categoryIds)
            {
                if (!categoryRepository.ExistsById(userId, categoryId))
                {
                    errorMessage = "Category not found.";
                    responseCode = 404;
                    return false;
                }
                else if(expenseCategories.Any(ec => ec.CategoryId == categoryId))
                {
                    errorMessage = "Category already added.";
                    responseCode = 400;
                    return false;
                }
            }


            foreach (var categoryId in categoryIds)
            {

                var expenseCategory = new ExpenseCategory()
                {
                    CategoryId = categoryId,
                    Category = categoryRepository.GetById(categoryId),
                    ExpenseId = expenseId,
                    Expense = expense,
                };

                if (!AddCategory(expenseCategory))
                {
                    errorMessage = "Something went wrong while adding category to expense.";
                    responseCode = 500;
                    return false;
                }
            }

            return true;
        }
    }
}
