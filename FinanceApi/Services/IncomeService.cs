using FinanceApi.Data.Dtos;
using FinanceApi.Enums;
using FinanceApi.Mapper;
using FinanceApi.Models;
using FinanceApi.Repositories;
using FinanceApi.Repositories.Interfaces;
using FinanceApi.Services.Interfaces;
using FinanceApi.Validators;

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

        public bool AddCategories(string userId, int incomeId, ICollection<int> categoryIds, out string errorMessage, out int errorCode)
        {
            errorMessage = string.Empty;
            errorCode = 0;
            if (!incomeRepository.ExistsById(userId, incomeId))
            {
                errorMessage = "Income not found.";
                errorCode = 404;
                return false;
            }

            if (categoryIds == null || categoryIds.Count() <= 0)
            {
                errorMessage = "No category id's found.";
                errorCode = 400;
                return false;
            }


            var income = incomeRepository.GetById(incomeId, true);

            var incomeCategories = categoryRepository.GetIncomeCategories(userId, incomeId);

            foreach (var categoryId in categoryIds)
            {
                if (!categoryRepository.ExistsById(userId, categoryId))
                {
                    errorMessage = "Category not found.";
                    errorCode = 404;
                    return false;
                }
                else if (incomeCategories.Any(ec => ec.CategoryId == categoryId))
                {
                    errorMessage = "Category already added.";
                    errorCode = 400;
                    return false;
                }
            }


            foreach (var categoryId in categoryIds)
            {

                var incomeCategory = new IncomeCategory()
                {
                    CategoryId = categoryId,
                    Category = categoryRepository.GetById(categoryId, true),
                    IncomeId = incomeId,
                    Income = income,
                };

                if (!incomeRepository.AddCategory(incomeCategory))
                {
                    errorMessage = "Something went wrong while adding category to income.";
                    errorCode = 500;
                    return false;
                }
            }

            return true;
        }

        public bool AddCategory(IncomeCategory incomeCategory)
        {
            return incomeRepository.AddCategory(incomeCategory);
        }

        public bool Delete(Income income)
        {
            return incomeRepository.Delete(income);
        }

        public bool ExistsById(string userId, int incomeId)
        {
            return incomeRepository.ExistsById(userId, incomeId);
        }

        public bool TryGetIncomesFilteredOrDefault(string userId,
            out ICollection<Income> incomes,
            DateTime? startDate,
            DateTime? endDate,
            string? list_order_by,
            string? list_dir,
            out int errorCode,
            out string errorMessage)
        {

            errorCode = 0;
            errorMessage = string.Empty;

            incomes = incomeRepository.GetAllOfUser(userId);


            if (startDate != null || endDate != null)
            {
                if (!Validator.ValidateTimePeriod(startDate, endDate, out errorCode, out errorMessage))
                {
                    return false;
                }

                incomes = incomes.Where(i => i.Date >= startDate && i.Date <= endDate).ToList();
            }

            incomes = (list_dir != null && list_dir.Equals("desc")) ?
                (list_order_by switch
                {
                    "title" => incomes.OrderByDescending(i => i.Title),
                    "amount" => incomes.OrderByDescending(i => i.Amount),
                    _ => incomes.OrderByDescending(i => i.Date),
                }).ToList()
                :
                (list_order_by switch
                {
                    "title" => incomes.OrderBy(i => i.Title),
                    "amount" => incomes.OrderBy(i => i.Amount),
                    _ => incomes.OrderBy(i => i.Date),
                }).ToList();



            return true;
        }

        public Income GetById(int incomeId, bool tracking)
        {
            return incomeRepository.GetById(incomeId, tracking);
        }

        public bool ValidateIncome(IncomeDto incomeDto, out int errorCode, out string errorMessage)
        {
            errorCode = 0;
            errorMessage = string.Empty;

            if (!Validator.IsValidCurrencyCode(incomeDto.Currency))
            {
                errorCode = 400;
                errorMessage = "Currency ISOcode is not valid.";
                return false;
            }

            if (incomeDto.Amount <= 0 || incomeDto.Amount > 100000000)
            {
                errorCode = 400;
                errorMessage = "Amount must be more then '0' and smaller then '100000000'.";
                return false;
            }

            return true;
        }

        public bool Create(User user, IncomeDto incomeDto, out int errorCode, out string errorMessage)
        {
            errorCode = 0;
            errorMessage = string.Empty;

            if (!ValidateIncome(incomeDto, out errorCode, out errorMessage))
            {
                return false;
            }

            var income = Map.ToIncome(incomeDto);
            income.Currency = income.Currency.ToUpper();
            income.User = user;

            if (!incomeRepository.Create(income))
            {
                errorCode = 500;
                errorMessage = "Something went wrong while creating income.";
                return false;
            }

            return true;
        }

        public bool Update(User user, IncomeDto incomeDto, out int errorCode, out string errorMessage)
        {
            errorCode = 0;
            errorMessage = string.Empty;

            if (!incomeRepository.ExistsById(user.Id, incomeDto.Id))
            {
                errorCode = 404;
                errorMessage = "Income not found.";
                return false;
            }

            if (!ValidateIncome(incomeDto, out errorCode, out errorMessage))
            {
                return false;
            }

            var income = Map.ToIncome(incomeDto);
            income.Currency = income.Currency.ToUpper();

            if (!incomeRepository.Update(income))
            {
                errorCode = 500;
                errorMessage = "Something went wrong while updating income.";
                return false;
            }

            return true;
        }

        public bool tryGetIncomesWithCategoryId(User user, int categoryId, out ICollection<Income> incomes, out int errorCode, out string errorMessage)
        {
            errorCode = 0;
            errorMessage = string.Empty;
            incomes = new List<Income>();



            if (!categoryRepository.ExistsById(user.Id, categoryId))
            {
                errorCode = 404;
                errorMessage = "Category not found.";
                return false;
            }

            incomes = incomeRepository.GetAllOfUserByCategoryId(user.Id, categoryId);

            return true;
        }

        public bool TryDeleteIncome(User user, int incomeId, out int errorCode, out string errorMessage)
        {

            errorCode = 0;
            errorMessage = string.Empty;

            if (!incomeRepository.ExistsById(user.Id, incomeId))
            {
                errorCode = 404;
                errorMessage = "Income not found.";
                return false;
            }

            var income = incomeRepository.GetById(incomeId, true);

            if (!incomeRepository.Delete(income))
            {
                errorCode = 500;
                errorMessage = "Something went wrong while deleting income.";
                return false;
            }

            return true;
        }

        public bool TryRemoveCategories(User user, int incomeId, ICollection<int> categoryIds, out int errorCode, out string errorMessage)
        {

            errorCode = 0;
            errorMessage = string.Empty;

            if (!incomeRepository.ExistsById(user.Id, incomeId))
            {
                errorCode = 404;
                errorMessage = "Income not found.";
                return false;
            }


            var incomeCategories = categoryRepository.GetIncomeCategories(user.Id, incomeId);

            if (incomeCategories.Count <= 0)
            {
                errorCode = 400;
                errorMessage = "Income does not have any categories";
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

                if (!incomeCategories.Any(ic => ic.CategoryId == categoryId))
                {
                    errorCode = 400;
                    errorMessage = "Income does not have this category";
                    return false;
                }
            }

            foreach (var categoryId in categoryIds)
            {

                if (!incomeRepository.DeleteIncomeCategoryWithId(user.Id, categoryId, incomeId))
                {
                    errorCode = 500;
                    errorMessage = "Something went wrong while deleting income category.";
                    return false;
                }
            }

            return true;
        }
    }
}
