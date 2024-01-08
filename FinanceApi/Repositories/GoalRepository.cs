using FinanceApi.Data;
using FinanceApi.Models;
using FinanceApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Repositories
{
    public class GoalRepository : IGoalRepository
    {
        private readonly DataContext dataContext;

        public GoalRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public bool AddCategory(GoalCategory goalCategory)
        {
            dataContext.GoalCategories.Add(goalCategory);
            return Save();
        }

        public bool Create(Goal goal)
        {
            dataContext.Goals.Add(goal);
            return Save();
        }

        public bool Delete(Goal goal)
        {
            dataContext.Goals.Remove(goal);
            return Save();
        }

        public bool ExistsById(string userId, int goalId)
        {
            return dataContext.Goals.Any(g => g.Id == goalId && g.User.Id.Equals(userId));
        }

        public bool ExistsByTitle(string userId, string title)
        {
            return dataContext.Goals.Any(g => g.Title.ToLower().Equals(title.ToLower()));
        }

        public ICollection<Goal> GetAllOfUser(string userId)
        {
            return dataContext.Goals.AsNoTracking().Where(g => g.User.Id.Equals(userId)).ToList();
        }

        public Goal GetById(int goalId, bool tracking)
        {
            if (tracking)
            {
                return dataContext.Goals.FirstOrDefault(g => g.Id == goalId);
            }
            return dataContext.Goals.AsNoTracking().FirstOrDefault(g => g.Id == goalId);
        }

        public bool HasGoals(string userId)
        {
            return dataContext.Goals.Any(g => g.User.Id.Equals(userId));
        }

        public bool Save()
        {
            var saved = dataContext.SaveChanges();
            return saved > 0;
        }

        public bool Update(Goal goal)
        {
            dataContext.Goals.Update(goal);
            return Save();
        }
    }
}
