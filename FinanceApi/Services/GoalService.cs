using FinanceApi.Data.Dtos;
using FinanceApi.Mapper;
using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;
using FinanceApi.Services.Interfaces;
using FinanceApi.Validators;

namespace FinanceApi.Services
{
    public class GoalService : IGoalService
    {
        private readonly IGoalRepository goalRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IIncomeRepository incomeRepository;

        public GoalService(IGoalRepository goalRepository, ICategoryRepository categoryRepository, IIncomeRepository incomeRepository)
        {
            this.goalRepository = goalRepository;
            this.categoryRepository = categoryRepository;
            this.incomeRepository = incomeRepository;
        }

        public bool Create(User user, GoalManageDto goalManageDto, out int errorCode, out string errorMessage)
        {
            
            errorCode = 0;
            errorMessage = string.Empty;

            if(!ValidateGoal(goalManageDto, out errorCode, out errorMessage))
            {
                return false;
            }

            if(goalRepository.ExistsByTitle(user.Id, goalManageDto.Title))
            {
                errorCode = 400;
                errorMessage = "Goal with this title already in use.";
                return false;
            }

            var goal = Map.ToGoalFromManageDto(goalManageDto);
            goal.Currency = goalManageDto.Currency.ToUpper();
            goal.User = user;

            if (!goalRepository.Create(goal))
            {
                errorCode = 500;
                errorMessage = "Something went wrong while creating goal.";
                return false;
            }

            return true;
        }

        public bool ValidateGoal(GoalManageDto goalDto, out int errorCode, out string errorMessage)
        {
            errorCode = 0;
            errorMessage = string.Empty;

            if(goalDto.Amount <= 0)
            {
                errorCode = 400;
                errorMessage = "Amount must be greater then '0'.";
                return false;
            }

            if (!Validator.IsValidCurrencyCode(goalDto.Currency))
            {
                errorCode = 400;
                errorMessage = "Currency ISOcode is not valid.";
                return false;
            }

            if(!Validator.ValidateTimePeriod(goalDto.StartDate, goalDto.EndDate, out errorCode, out errorMessage))
            {
                return false;
            }


            return true;
        }

        public bool AddCategories(string userId, int goalId, ICollection<int> categoryIds, out string errorMessage, out int errorCode)
        {
            errorMessage = string.Empty;
            errorCode = 0;
            if (!goalRepository.ExistsById(userId, goalId))
            {
                errorMessage = "Goal not found.";
                errorCode = 404;
                return false;
            }

            if (categoryIds == null || categoryIds.Count() <= 0)
            {
                errorMessage = "No category id's found.";
                errorCode = 400;
                return false;
            }


            var goal = goalRepository.GetById(goalId, true);

            var goalCategories = categoryRepository.GetGoalCategories(userId, goalId);

            foreach (var categoryId in categoryIds)
            {
                if (!categoryRepository.ExistsById(userId, categoryId))
                {
                    errorMessage = "Category not found.";
                    errorCode = 404;
                    return false;
                }
                else if (goalCategories.Any(gc => gc.CategoryId == categoryId))
                {
                    errorMessage = "Category already added.";
                    errorCode = 400;
                    return false;
                }
            }


            foreach (var categoryId in categoryIds)
            {

                var goalCategory = new GoalCategory()
                {
                    CategoryId = categoryId,
                    Category = categoryRepository.GetById(categoryId, true),
                    GoalId = goalId,
                    Goal = goal,
                };

                if (!goalRepository.AddCategory(goalCategory))
                {
                    errorMessage = "Something went wrong while adding category to goal.";
                    errorCode = 500;
                    return false;
                }
            }

            return true;
        }

        public bool Update(User user, GoalManageDto goalManageDto, out int errorCode, out string errorMessage)
        {
            errorCode = 0;
            errorMessage = string.Empty;
            if (!goalRepository.ExistsById(user.Id, goalManageDto.Id))
            {
                errorCode = 404;
                errorMessage = "Goal not found.";
                return false;
            }

            if(!ValidateGoal(goalManageDto, out errorCode, out errorMessage))
            {
                return false;
            }

            var titleInUse = goalRepository.GetAllOfUser(user.Id).Any(g => g.Id != goalManageDto.Id && g.Title.ToLower().Equals(goalManageDto.Title.ToLower()));

            if (titleInUse)
            {
                errorCode = 400;
                errorMessage = "Title is already in use.";
                return false;
            }

            var goal = Map.ToGoalFromGoalManageDto(goalManageDto);
            goal.Currency = goalManageDto.Currency.ToUpper();

            if (!goalRepository.Update(goal))
            {
                errorCode = 500;
                errorMessage = "Something went wrong while updating goal.";
                return false;
            }

            return true;
        }

        public bool TryDeleteGoal(User user, int goalId, out int errorCode, out string errorMessage)
        {

            errorCode = 0;
            errorMessage = string.Empty;

            if(!goalRepository.ExistsById(user.Id, goalId))
            {
                errorCode = 404;
                errorMessage = "Goal not found";
                return false;
            }

            var goal = goalRepository.GetById(goalId, true);

            if (!goalRepository.Delete(goal))
            {
                errorCode = 500;
                errorMessage = "Something went wrong while deleting goal.";
                return false;
            }

            return true;
        }

        public bool TryRemoveCategory(User user, int goalId, int categoryId, out int errorCode, out string errorMessage)
        {

            errorCode = 0;
            errorMessage = string.Empty;

            if (!goalRepository.ExistsById(user.Id, goalId))
            {
                errorCode = 404;
                errorMessage = "Goal not found.";
                return false;
            }

            var goalCategories = categoryRepository.GetGoalCategories(user.Id, goalId);

            if (goalCategories.Count <= 0)
            {
                errorCode = 400;
                errorMessage = "Goal does not have any categories";
                return false;
            }

            if (!categoryRepository.ExistsById(user.Id, categoryId))
            {
                errorCode = 404;
                errorMessage = "Category not found.";
                return false;
            }

            if (!goalCategories.Any(ic => ic.CategoryId == categoryId))
            {
                errorCode = 400;
                errorMessage = "Goal does not have this category";
                return false;
            }


            if (!goalRepository.DeleteGoalCategoryWithId(user.Id, categoryId, goalId))
            {
                errorCode = 500;
                errorMessage = "Something went wrong while deleting budget category.";
                return false;
            }

            return true;
        }

        public decimal GetProgressAmountOfGoal(string userId, int goalId)
        {
            decimal amount = 0;

            var goal = goalRepository.GetById(goalId, false);

            var incomes = incomeRepository.GetAllOfUserByGoalId(userId, goalId);

            foreach (var income in incomes)
            {
                if(income.Date <= DateTime.Now && income.Date >= goal.StartDate && income.Date <= goal.EndDate)
                {
                    amount += income.Amount;
                }
            }

            return amount;
        }

        public bool TryGetAllOrderedOrDefault(
            string userId,
            out ICollection<Goal> goals,
            out int errorCode,
            out string errorMessage,
            DateTime? startDate,
            DateTime? endDate,
            string? listOrderBy,
            string? listDir,
            int? categoryId)
        {
            errorCode = 0;
            errorMessage = string.Empty;
            goals = null;

            try
            {
                goals = categoryId == null
                    ? goalRepository.GetAllOfUser(userId)
                    : goalRepository.GetAllOfUserByCategoryId(userId, (int) categoryId);

                if (startDate != null || endDate != null)
                {
                    if (!Validator.ValidateTimePeriod(startDate, endDate, out errorCode, out errorMessage))
                    {
                        return false;
                    }
                    goals = goals.Where(g => g.StartDate >= startDate && g.EndDate <= endDate).ToList();
                }


                goals = listDir != null && listDir.Equals("desc") ?
                    (listOrderBy switch
                    {

                        "title" => goals.OrderByDescending(g => g.Title),
                        "amount" => goals.OrderByDescending(g => g.Amount),
                        "progress" => goals.OrderByDescending(g => GetProgressAmountOfGoal(userId, g.Id)),
                        _ => goals.OrderByDescending(g => g.EndDate),

                    }).ToList()
                    :
                    (listOrderBy switch
                    {

                        "title" => goals.OrderBy(g => g.Title),
                        "amount" => goals.OrderBy(g => g.Amount),
                        "progress" => goals.OrderBy(g => GetProgressAmountOfGoal(userId, g.Id)),
                        _ => goals.OrderBy(g => g.EndDate),

                    }).ToList();

                return true;
            }
            catch (Exception ex)
            {
                errorCode = 500;
                errorMessage = "Something unexpected happened";
                return false;
            }

        }
    }
}
