using FinanceApi.Models;
using FinanceApi.Repositories;
using FinanceApi.Repositories.Interfaces;
using FinanceApi.Services.Interfaces;

namespace FinanceApi.Services
{
    public class IncomeService : IIncomeService
    {
        private readonly IIncomeRepository incomeRepository;
        private readonly ICategoryRepository categoryRepository;

        public IncomeService(IIncomeRepository incomeRepository, ICategoryRepository categoryRepository)
        {
            this.incomeRepository = incomeRepository;
            this.categoryRepository = categoryRepository;
        }

        public bool AddCategories(string userId, int incomeId, ICollection<int> categoryIds, out string errorMessage, out int responseCode)
        {
            errorMessage = string.Empty;
            responseCode = 200;
            if (!Exists(userId, incomeId))
            {
                errorMessage = "Income not found.";
                responseCode = 404;
                return false;
            }

            if (categoryIds == null || categoryIds.Count() <= 0)
            {
                errorMessage = "No category id's found.";
                responseCode = 400;
                return false;
            }


            var income = GetById(incomeId);

            var incomeCategories = categoryRepository.GetIncomeCategories(userId, incomeId);

            foreach (var categoryId in categoryIds)
            {
                if (!categoryRepository.ExistsById(userId, categoryId))
                {
                    errorMessage = "Category not found.";
                    responseCode = 404;
                    return false;
                }
                else if (incomeCategories.Any(ec => ec.CategoryId == categoryId))
                {
                    errorMessage = "Category already added.";
                    responseCode = 400;
                    return false;
                }
            }


            foreach (var categoryId in categoryIds)
            {

                var incomeCategory = new IncomeCategory()
                {
                    CategoryId = categoryId,
                    Category = categoryRepository.GetById(categoryId),
                    IncomeId = incomeId,
                    Income = income,
                };

                if (!AddCategory(incomeCategory))
                {
                    errorMessage = "Something went wrong while adding category to income.";
                    responseCode = 500;
                    return false;
                }
            }

            return true;
        }

        public bool AddCategory(IncomeCategory incomeCategory)
        {
            return incomeRepository.AddCategory(incomeCategory);
        }

        public bool Create(Income income)
        {
            return incomeRepository.Create(income);
        }

        public bool Delete(Income income)
        {
            return incomeRepository.Delete(income);
        }

        public bool Exists(string userId, int incomeId)
        {
            return incomeRepository.Exists(userId, incomeId);
        }

        public ICollection<Income> GetAllByUserId(string userId)
        {
            return incomeRepository.GetAllByUserId(userId);
        }

        public Income GetById(int incomeId)
        {
            return incomeRepository.GetById(incomeId);
        }

        public bool Update(Income income)
        {
            return incomeRepository.Update(income);
        }
    }
}
