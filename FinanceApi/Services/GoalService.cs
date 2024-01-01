using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;
using FinanceApi.Services.Interfaces;

namespace FinanceApi.Services
{
    public class GoalService : IGoalService
    {
        private readonly IGoalRepository goalRepository;

        public GoalService(IGoalRepository goalRepository)
        {
            this.goalRepository = goalRepository;
        }
        public bool Create(Goal goal)
        {
            return goalRepository.Create(goal);
        }

        public bool Delete(Goal goal)
        {
            return goalRepository.Delete(goal);
        }

        public bool Exists(int goalId)
        {
            return goalRepository.Exists(goalId);
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
