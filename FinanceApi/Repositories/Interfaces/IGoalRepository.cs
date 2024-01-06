using FinanceApi.Models;

namespace FinanceApi.Repositories.Interfaces
{
    public interface IGoalRepository
    {
        Goal GetById(int goalId);
        ICollection<Goal> GetAllOfUser(string userId);
        bool HasGoals(string userId);
        bool ExistsById(string userId, int goalId);
        bool Create(Goal goal);
        bool Update(Goal goal);
        bool Delete(Goal goal);
        bool Save();
        bool ExistsByTitle(string userId, string title);
        bool AddCategory(GoalCategory goalCategory);
    }
}
