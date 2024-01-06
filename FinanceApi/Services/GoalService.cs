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

        public GoalService(IGoalRepository goalRepository)
        {
            this.goalRepository = goalRepository;
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

        public Goal GetById(int goalId)
        {
            return goalRepository.GetById(goalId);
        }

        public bool HasGoals(string userId)
        {
            return goalRepository.HasGoals(userId);
        }

        public bool Update(Goal goal)
        {
            return goalRepository.Update(goal);
        }
    }
}
