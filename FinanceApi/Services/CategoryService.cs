using FinanceApi.Currency;
using FinanceApi.Data.Dtos;
using FinanceApi.Mapper;
using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;
using FinanceApi.Services.Interfaces;

namespace FinanceApi.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IExpenseRepository expenseRepository;
        private readonly IUserRepository userRepository;
        private readonly IIncomeRepository incomeRepository;

        public CategoryService(ICategoryRepository categoryRepository, IExpenseRepository expenseRepository, IUserRepository userRepository, IIncomeRepository incomeRepository)
        {
            this.categoryRepository = categoryRepository;
            this.expenseRepository = expenseRepository;
            this.userRepository = userRepository;
            this.incomeRepository = incomeRepository;
        }

        public bool Create(User user, CategoryManageDto categoryManageDto, out int errorCode, out string errorMessage)
        {
            errorMessage = string.Empty;
            errorCode = 0;

            if (categoryRepository.ExistsBytitle(user.Id, categoryManageDto.Title))
            {
                errorCode = 400;
                errorMessage = "Category with this title already exists.";
                return false;
            }

            var category = Map.ToCategoryFromManageDto(categoryManageDto);
            category.User = user;

            if (!categoryRepository.Create(category))
            {
                errorCode = 500;
                errorMessage = "Something went wrong while creating category.";
                return false;
            }

            return true;
        }
        public bool Update(User user, CategoryManageDto categoryDto, out int errorCode, out string errorMessage)
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

            var category = Map.ToCategoryFromManageDto(categoryDto);

            if (!categoryRepository.Update(category))
            {
                errorCode = 500;
                errorMessage = "Something went wrong while updating.";
                return false;
            }

            return true;
        }

        private decimal GetExpenseAmountOfCategoryById(User user, int id)
        {

            var expenseAmount = expenseRepository.GetAllOfUserByCategoryId(user.Id, id)
                .Select(x =>
                {
                    var exchangeRate = CurrencyExchange.GetExchangeRate(x.Currency, user.Currency, x.Date);
                    return x.Amount * exchangeRate;
                }
                ).Sum();
            return expenseAmount;
        }
        private decimal GetIncomeAmountOfCategoryById(User user, int id)
        {
            var incomeAmount = incomeRepository.GetAllOfUserByCategoryId(user.Id, id)
                .Select(x =>
                {
                    var exchangeRate = CurrencyExchange.GetExchangeRate(x.Currency, user.Currency, x.Date);
                    return x.Amount * exchangeRate;
                }
                ).Sum();
            return incomeAmount;
        }

        public bool TryDelete(User user, int categoryId, out int errorCode, out string errorMessage)
        {

            errorCode = 0;
            errorMessage = string.Empty;


            if (!categoryRepository.ExistsById(user.Id, categoryId))
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

        public bool TryGetCategoryDtosOrderedOrDefault(string userId, out ICollection<CategoryDto> categories, out int errorCode, out string errorMessage, string ListOrderBy, string listDir)
        {
            errorCode = 0;
            errorMessage = string.Empty;
            categories = new List<CategoryDto>();

            if (!userRepository.ExistsById(userId))
            {
                errorCode = 401;
                errorMessage = "Unauthorized.";
                return false;
            }

            try
            {

                var user = userRepository.GetById(userId, true);

                categories = categoryRepository.GetAllOfUser(userId).Select(Map.ToCategoryDto).ToList();

                categories = categories.Select(x =>
                {
                    x.ExpenseAmount = GetExpenseAmountOfCategoryById(user, x.Id);
                    x.IncomeAmount = GetIncomeAmountOfCategoryById(user, x.Id);
                    return x;
                }).ToList();

                categories = listDir != null && listDir.Equals("desc") ?
                    (ListOrderBy switch
                    {
                        "title" => categories.OrderByDescending(c => c.Title),
                        "expense" => categories.OrderByDescending(c => c.ExpenseAmount),
                        "income" => categories.OrderByDescending(c => c.IncomeAmount),
                        _ => categories.OrderByDescending(c => c.Id),

                    }).ToList()
                    :
                    (ListOrderBy switch
                    {
                        "title" => categories.OrderBy(c => c.Title),
                        "expense" => categories.OrderBy(c => c.ExpenseAmount),
                        "income" => categories.OrderBy(c => c.IncomeAmount),
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
