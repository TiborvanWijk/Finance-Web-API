using FinanceApi.Data;
using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;

namespace FinanceApi.Repositories
{
    public class GoalRepository : IGoalRepository
    {
        private readonly DataContext dataContext;

        public GoalRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }
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

        public bool Save()
        {
            var saved = dataContext.SaveChanges();
            return saved > 0;
        }

        public bool Update(Goal goal)
        {
            throw new NotImplementedException();
        }
    }
}
