using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;

namespace FinanceApi.Repositories
{
    public class GoalRepository : IGoalRepository
    {
        public bool Create(Goal goal)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Goal goal)
        {
            throw new NotImplementedException();
        }

        public bool Exists(int goalId)
        {
            throw new NotImplementedException();
        }

        public ICollection<Goal> GetAllOfUser(int userId)
        {
            throw new NotImplementedException();
        }

        public Goal GetById(int goalId)
        {
            throw new NotImplementedException();
        }

        public bool HasGoals(int userId)
        {
            throw new NotImplementedException();
        }

        public bool Update(Goal goal)
        {
            throw new NotImplementedException();
        }
    }
}
