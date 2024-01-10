using FinanceApi.Data.Dtos;
using FinanceApi.Mapper;
using FinanceApi.Models;
using FinanceApi.Repositories;
using FinanceApi.Repositories.Interfaces;
using FinanceApi.Services.Interfaces;
using FinanceApi.Validators;

namespace FinanceApi.Services
{
    public class GoalService : IGoalService
    {
        private readonly IGoalRepository goalRepository;
        private readonly ICategoryRepository categoryRepository;

        public GoalService(IGoalRepository goalRepository, ICategoryRepository categoryRepository)
        {
            this.goalRepository = goalRepository;
            this.categoryRepository = categoryRepository;
        }

        public bool Create(User user, GoalManageDto goalManageDto, out int errorCode, out string errorMessage)
        {
            
            errorCode = 0;
            errorMessage = string.Empty;

            if(!ValidateGoal(goalManageDto, out errorCode, out errorMessage))
            {
                return false;
            }

            if(ExistsByTitle(user.Id, goalManageDto.Title))
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

            if(goalDto.TargetDate <= DateTime.Today)
            {
                errorCode = 400;
                errorMessage = "Target date must be later then today.";
                return false;
            }

            return true;
        }

        public bool Delete(Goal goal)
        {
            return goalRepository.Delete(goal);
        }

        public bool ExistsById(string userId, int goalId)
        {
            return goalRepository.ExistsById(userId, goalId);
        }

        public bool ExistsByTitle(string userId, string title)
        {
            return goalRepository.ExistsByTitle(userId, title);
        }

        public ICollection<Goal> GetAllOfUser(string userId)
        {
            return goalRepository.GetAllOfUser(userId);
        }

        public Goal GetById(int goalId, bool tracking)
        {
            return goalRepository.GetById(goalId, tracking);
        }

        public bool HasGoals(string userId)
        {
            return goalRepository.HasGoals(userId);
        }

        public bool Update(Goal goal)
        {
            return goalRepository.Update(goal);
        }

        public bool AddCategories(string userId, int goalId, ICollection<int> categoryIds, out string errorMessage, out int errorCode)
        {
            errorMessage = string.Empty;
            errorCode = 0;
            if (!ExistsById(userId, goalId))
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


            var goal = GetById(goalId, true);

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

                if (!AddCategory(goalCategory))
                {
                    errorMessage = "Something went wrong while adding category to goal.";
                    errorCode = 500;
                    return false;
                }
            }

            return true;
        }

        public bool AddCategory(GoalCategory goalCategory)
        {
            return goalRepository.AddCategory(goalCategory);
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

            var goal = goalRepository.GetById(goalManageDto.Id, true);

            goal.Title = goalManageDto.Title;
            goal.Description = goalManageDto.Description;
            goal.Amount = goalManageDto.Amount;
            goal.Currency = goalManageDto.Currency.ToUpper();
            goal.TargetDate = goalManageDto.TargetDate;

            if (!goalRepository.Update(goal))
            {
                errorCode = 500;
                errorMessage = "Something went wrong while updating goal.";
                return false;
            }

            return true;
        }

        public bool TryGetGoalsById(User user, int categoryId, out ICollection<Goal> goals, out int errorCode, out string errorMessage)
        {

            errorCode = 0;
            errorMessage = string.Empty;
            goals = new List<Goal>();

            if(!categoryRepository.ExistsById(user.Id, categoryId))
            {
                errorCode = 404;
                errorMessage = "Category not found.";
                return false;
            }

            goals = goalRepository.GetAllOfUserByCategoryId(user.Id, categoryId);


            return true;
        }
    }
}
